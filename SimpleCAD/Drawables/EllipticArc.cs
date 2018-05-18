using SimpleCAD.Geometry;
using SimpleCAD.Graphics;
using System;
using System.ComponentModel;

namespace SimpleCAD.Drawables
{
    public class EllipticArc : Curve
    {
        private Point2D center;

        public Point2D Center { get => center; set { center = value; UpdatePolyline(); NotifyPropertyChanged(); } }

        [Browsable(false)]
        public float X { get { return Center.X; } }
        [Browsable(false)]
        public float Y { get { return Center.Y; } }

        private float semiMajorAxis;
        private float semiMinorAxis;
        private float rotation;

        public float SemiMajorAxis { get => semiMajorAxis; set { semiMajorAxis = value; UpdatePolyline(); NotifyPropertyChanged(); } }
        public float SemiMinorAxis { get => semiMinorAxis; set { semiMinorAxis = value; UpdatePolyline(); NotifyPropertyChanged(); } }
        public float Rotation { get => rotation; set { rotation = value; UpdatePolyline(); NotifyPropertyChanged(); } }

        private float startAngle;
        private float endAngle;

        public float StartAngle { get => startAngle; set { startAngle = value; UpdatePolyline(); NotifyPropertyChanged(); } }
        public float EndAngle { get => endAngle; set { endAngle = value; UpdatePolyline(); NotifyPropertyChanged(); } }

        private Polyline poly;
        private float curveLength = 4;
        private float cpSize = 0;

        public EllipticArc() { }

        public EllipticArc(Point2D center, float semiMajor, float semiMinor, float startAngle, float endAngle, float rotation = 0)
        {
            Center = center;
            SemiMajorAxis = semiMajor;
            SemiMinorAxis = semiMinor;
            StartAngle = startAngle;
            EndAngle = endAngle;
            Rotation = rotation;
            UpdatePolyline();
        }

        public EllipticArc(float x, float y, float semiMajor, float semiMinor, float startAngle, float endAngle, float rotation = 0)
            : this(new Point2D(x, y), semiMajor, semiMinor, startAngle, endAngle, rotation)
        {
            ;
        }

        private void UpdatePolyline()
        {
            poly = new Polyline();
            // Represent curved features by at most 4 pixels
            float sweep = EndAngle - StartAngle;
            while (sweep < 0) sweep += 2 * MathF.PI;
            while (sweep > 2 * MathF.PI) sweep -= 2 * MathF.PI;
            int n = (int)Math.Min(MaxCurveSegments, Math.Max(MinCurveSegments, curveLength / 4));
            float a = StartAngle;
            float da = sweep / n;
            for (int i = 0; i < n + 1; i++)
            {
                float dx = MathF.Cos(a) * SemiMinorAxis;
                float dy = MathF.Sin(a) * SemiMajorAxis;
                float t = MathF.Atan2(dy, dx);

                float x = SemiMajorAxis * MathF.Cos(t);
                float y = SemiMinorAxis * MathF.Sin(t);
                poly.Points.Add(x, y);
                a += da;
            }
            poly.Closed = false;
            poly.TransformBy(Matrix2D.Rotation(Rotation));
            poly.TransformBy(Matrix2D.Translation(Center.X, Center.Y));
        }

        public override void Draw(Renderer renderer)
        {
            cpSize = 2 * renderer.View.ScreenToWorld(new Vector2D(renderer.View.Document.Settings.ControlPointSize, 0)).X;
            // Approximate perimeter
            float p = 2 * MathF.PI * (3 * (SemiMajorAxis + SemiMinorAxis) - MathF.Sqrt((3 * SemiMajorAxis + SemiMinorAxis) * (SemiMajorAxis + 3 * SemiMinorAxis)));
            float newCurveLength = renderer.View.WorldToScreen(new Vector2D(p, 0)).X;
            if (!MathF.IsEqual(newCurveLength, curveLength))
            {
                curveLength = newCurveLength;
                UpdatePolyline();
            }

            poly.Style = Style.ApplyLayer(Layer);
            renderer.Draw(poly);
        }

        public override Extents2D GetExtents()
        {
            return poly.GetExtents();
        }

        public override void TransformBy(Matrix2D transformation)
        {
            Center = Center.Transform(transformation);
            Rotation = Vector2D.FromAngle(Rotation).Transform(transformation).Angle;
            SemiMajorAxis = (Vector2D.XAxis * SemiMajorAxis).Transform(transformation).Length;
            SemiMinorAxis = (Vector2D.XAxis * SemiMinorAxis).Transform(transformation).Length;
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            return poly.Contains(pt, pickBoxSize);
        }

        public override ControlPoint[] GetControlPoints()
        {
            return new[]
            {
                new ControlPoint("Center point", Center),
                new ControlPoint("Semi major axis", ControlPointType.Distance, Center, Center + SemiMajorAxis * Vector2D.FromAngle(Rotation)),
                new ControlPoint("Semi minor axis", ControlPointType.Distance, Center, Center + SemiMinorAxis * Vector2D.FromAngle(Rotation).Perpendicular),
                new ControlPoint("Rotation", ControlPointType.Angle, Center, Center + (SemiMajorAxis + cpSize) * Vector2D.FromAngle(Rotation)),
            };
        }

        public override SnapPoint[] GetSnapPoints()
        {
            float ts = MathF.Atan2(MathF.Sin(StartAngle) * SemiMajorAxis, MathF.Cos(StartAngle) * SemiMinorAxis);
            float xs = SemiMajorAxis * MathF.Cos(ts);
            float ys = SemiMinorAxis * MathF.Sin(ts);
            float te = MathF.Atan2(MathF.Sin(EndAngle) * SemiMajorAxis, MathF.Cos(EndAngle) * SemiMinorAxis);
            float xe = SemiMajorAxis * MathF.Cos(ts);
            float ye = SemiMinorAxis * MathF.Sin(ts);

            return new[]
            {
                new SnapPoint("Center point", SnapPointType.Center, Center),
                new SnapPoint("Start point", new Point2D(xs, ys)),
                new SnapPoint("End point", new Point2D(xe, ye)),
            };
        }

        public override void TransformControlPoints(int[] indices, Matrix2D transformation)
        {
            foreach (int index in indices)
            {
                if (index == 0)
                    Center = Center.Transform(transformation);
                else if (index == 1)
                    SemiMajorAxis = Vector2D.XAxis.Transform(transformation).Length * SemiMajorAxis;
                else if (index == 2)
                    SemiMinorAxis = Vector2D.XAxis.Transform(transformation).Length * SemiMinorAxis;
                else if (index == 3)
                    Rotation = Vector2D.FromAngle(Rotation).Transform(transformation).Angle;
            }
        }

        public override void Load(DocumentReader reader)
        {
            base.Load(reader);
            Center = reader.ReadPoint2D();
            SemiMajorAxis = reader.ReadFloat();
            SemiMinorAxis = reader.ReadFloat();
            Rotation = reader.ReadFloat();
            StartAngle = reader.ReadFloat();
            EndAngle = reader.ReadFloat();
            UpdatePolyline();
        }

        public override void Save(DocumentWriter writer)
        {
            base.Save(writer);
            writer.Write(Center);
            writer.Write(SemiMajorAxis);
            writer.Write(SemiMinorAxis);
            writer.Write(Rotation);
            writer.Write(StartAngle);
            writer.Write(EndAngle);
        }

        public override float StartParam => StartAngle;
        public override float EndParam => EndAngle;

        public override float Area => (Math.Abs(EndAngle - StartAngle) - MathF.Sin(Math.Abs(EndAngle - StartAngle))) / 2 * SemiMajorAxis * SemiMinorAxis;

        [Browsable(false)]
        public override bool Closed => false;

        public override float GetDistAtParam(float param)
        {
            param = MathF.Clamp(param, StartParam, EndParam);
            float dist = 0;
            float dt = param / (float)MaxCurveSegments;
            float t = dt;
            Point2D lastPt = GetPointAtParam(StartParam);
            for (int i = 1; i < MaxCurveSegments; i++)
            {
                Point2D pt = GetPointAtParam(t);
                dist += (pt - lastPt).Length;
                lastPt = pt;
                t += dt;
            }
            return dist;
        }

        public override Point2D GetPointAtParam(float param)
        {
            param = MathF.Clamp(param, StartParam, EndParam);
            float x = SemiMajorAxis * MathF.Cos(param);
            float y = SemiMinorAxis * MathF.Sin(param);
            return Center + new Vector2D(x, y).Transform(Matrix2D.Rotation(Rotation));
        }

        public override Vector2D GetNormalAtParam(float param)
        {
            Point2D pt = GetPointAtParam(param);
            return new Vector2D(2 * pt.X / (SemiMajorAxis * SemiMajorAxis), 2 * pt.Y / (SemiMinorAxis * SemiMinorAxis)).Transform(Matrix2D.Rotation(Rotation));
        }

        public override float GetParamAtPoint(Point2D pt)
        {
            Vector2D dir = (pt - Center).Transform(Matrix2D.Rotation(-Rotation));
            float param = MathF.Atan2(dir.Y / SemiMinorAxis, dir.X / SemiMajorAxis);
            return MathF.Clamp(param, StartParam, EndParam);
        }

        public override void Reverse()
        {
            MathF.Swap(ref startAngle, ref endAngle);
        }

        public override bool Split(float[] @params, out Curve[] subCurves)
        {
            @params = ValidateParams(@params);
            if (@params.Length == 0)
            {
                subCurves = new Curve[0];
                return false;
            }

            subCurves = new Curve[@params.Length + 1];
            for (int i = 0; i < @params.Length + 1; i++)
            {
                float sp = (i == 0 ? StartParam : @params[i - 1]);
                float ep = (i == @params.Length ? EndParam : @params[i]);
                subCurves[i] = new EllipticArc(Center, SemiMajorAxis, SemiMinorAxis, sp, ep, Rotation);
            }

            return true;
        }
    }
}

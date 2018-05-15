using SimpleCAD.Geometry;
using SimpleCAD.Graphics;
using System;
using System.ComponentModel;

namespace SimpleCAD.Drawables
{
    public class Ellipse : Curve
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

        private Polyline poly;
        private float curveLength = 4;
        private float cpSize = 0;

        public Ellipse() { }

        public Ellipse(Point2D center, float semiMajor, float semiMinor, float rotation = 0)
        {
            Center = center;
            SemiMajorAxis = semiMajor;
            SemiMinorAxis = semiMinor;
            Rotation = rotation;
            UpdatePolyline();
        }

        public Ellipse(float x, float y, float semiMajor, float semiMinor, float rotation = 0)
            : this(new Point2D(x, y), semiMajor, semiMinor, rotation)
        {
            ;
        }

        private void UpdatePolyline()
        {
            poly = new Polyline();
            // Represent curved features by at most 4 pixels
            int n = (int)Math.Min(MaxCurveSegments, Math.Max(MinCurveSegments, curveLength / 4));
            float da = 2 * MathF.PI / n;
            float a = 0;
            for (int i = 0; i < n; i++)
            {
                float x = SemiMajorAxis * MathF.Cos(a);
                float y = SemiMinorAxis * MathF.Sin(a);
                poly.Points.Add(x, y);
                a += da;
            }
            poly.Closed = true;
            poly.TransformBy(Matrix2D.Rotation(Rotation));
            poly.TransformBy(Matrix2D.Translation(Center.X, Center.Y));
        }

        public override void Draw(Renderer renderer)
        {
            cpSize = 2 * renderer.View.ScreenToWorld(new Vector2D(renderer.View.Document.Settings.ControlPointSize, 0)).X;
            // Approximate perimeter (Ramanujan)
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
            return new[]
            {
                new SnapPoint("Center point", SnapPointType.Center, Center),
                new SnapPoint("East quadrant", SnapPointType.Quadrant, Center + SemiMajorAxis * Vector2D.FromAngle(Rotation)),
                new SnapPoint("North quadrant", SnapPointType.Quadrant, Center + SemiMinorAxis * Vector2D.FromAngle(Rotation).Perpendicular),
                new SnapPoint("West quadrant", SnapPointType.Quadrant, Center - SemiMajorAxis * Vector2D.FromAngle(Rotation)),
                new SnapPoint("South quadrant", SnapPointType.Quadrant, Center - SemiMinorAxis * Vector2D.FromAngle(Rotation).Perpendicular),
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
            UpdatePolyline();
        }

        public override void Save(DocumentWriter writer)
        {
            base.Save(writer);
            writer.Write(Center);
            writer.Write(SemiMajorAxis);
            writer.Write(SemiMinorAxis);
            writer.Write(Rotation);
        }

        public override float StartParam => 0;
        public override float EndParam => 2 * MathF.PI;

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
    }
}

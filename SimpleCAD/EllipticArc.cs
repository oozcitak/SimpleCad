using System;
using System.ComponentModel;
using System.Drawing;

namespace SimpleCAD
{
    public class EllipticArc : Drawable
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
        float curveLength = 4;

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
            int n = (int)Math.Max(4, curveLength / 4);
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
            poly.TransformBy(TransformationMatrix2D.Rotation(Rotation));
            poly.TransformBy(TransformationMatrix2D.Translation(Center.X, Center.Y));
        }

        public override void Draw(DrawParams param)
        {
            // Approximate perimeter (Ramanujan)
            float p = 2 * MathF.PI * (3 * (SemiMajorAxis + SemiMinorAxis) - MathF.Sqrt((3 * SemiMajorAxis + SemiMinorAxis) * (SemiMajorAxis + 3 * SemiMinorAxis)));
            float newCurveLength = param.ModelToView(p);
            if (!MathF.IsEqual(newCurveLength, curveLength))
            {
                curveLength = newCurveLength;
                UpdatePolyline();
            }
            poly.Outline = Outline;
            poly.Draw(param);
        }

        public override Extents2D GetExtents()
        {
            return poly.GetExtents();
        }

        public override void TransformBy(TransformationMatrix2D transformation)
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

        public override ControlPoint[] GetControlPoints(float size)
        {
            return new[]
            {
                new ControlPoint("Center"),
                new ControlPoint("SemiMajorAxis", ControlPoint.ControlPointType.Distance, Center, Center + SemiMajorAxis * Vector2D.FromAngle(Rotation)),
                new ControlPoint("SemiMinorAxis", ControlPoint.ControlPointType.Distance, Center, Center + SemiMinorAxis * Vector2D.FromAngle(Rotation).Perpendicular()),
                new ControlPoint("Rotation", ControlPoint.ControlPointType.Angle, Center, Center + (SemiMajorAxis + size) * Vector2D.FromAngle(Rotation)),
            };
        }
    }
}

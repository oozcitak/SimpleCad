using System;
using System.ComponentModel;
using System.Drawing;

namespace SimpleCAD
{
    public class EllipticArc : Drawable
    {
        private Point2D center;

        public Point2D Center { get => center; set { center = value; NotifyPropertyChanged(); } }

        [Browsable(false)]
        public float X { get { return Center.X; } }
        [Browsable(false)]
        public float Y { get { return Center.Y; } }

        private Vector2D dir;

        private float semiMajorAxis;
        private float semiMinorAxis;

        public float SemiMajorAxis { get => semiMajorAxis; set { semiMajorAxis = value; NotifyPropertyChanged(); } }
        public float SemiMinorAxis { get => semiMinorAxis; set { semiMinorAxis = value; NotifyPropertyChanged(); } }

        private float startAngle;
        private float endAngle;

        public float StartAngle { get => startAngle; set { startAngle = value; NotifyPropertyChanged(); } }
        public float EndAngle { get => endAngle; set { endAngle = value; NotifyPropertyChanged(); } }

        public EllipticArc(Point2D center, float semiMajor, float semiMinor, float startAngle, float endAngle)
        {
            Center = center;
            SemiMajorAxis = semiMajor;
            SemiMinorAxis = semiMinor;
            dir = Vector2D.XAxis;
            StartAngle = startAngle;
            EndAngle = endAngle;
        }

        public EllipticArc(float x, float y, float semiMajor, float semiMinor, float startAngle, float endAngle)
            : this(new Point2D(x, y), semiMajor, semiMinor, startAngle, endAngle)
        {
            ;
        }

        public override void Draw(DrawParams param)
        {
            // Approximate perimeter (Ramanujan)
            float p = 2 * MathF.PI * (3 * (SemiMajorAxis + SemiMinorAxis) - MathF.Sqrt((3 * SemiMajorAxis + SemiMinorAxis) * (SemiMajorAxis + 3 * SemiMinorAxis)));
            // Represent curved features by at most 4 pixels
            float sweep = EndAngle - StartAngle;
            while (sweep < 0) sweep += 2 * MathF.PI;
            while (sweep > 2 * MathF.PI) sweep -= 2 * MathF.PI;
            float curveLength = param.ModelToView(sweep / (2 * MathF.PI) * p);
            int n = (int)Math.Max(4, curveLength / 4);
            float a = StartAngle;
            float da = sweep / n;
            Point2DCollection pts = new Point2DCollection();
            for (int i = 0; i < n + 1; i++)
            {
                float dx = MathF.Cos(a) * SemiMinorAxis;
                float dy = MathF.Sin(a) * SemiMajorAxis;
                float t = MathF.Atan2(dy, dx);

                float x = SemiMajorAxis * MathF.Cos(t);
                float y = SemiMinorAxis * MathF.Sin(t);
                pts.Add(x, y);
                a += da;
            }
            pts.TransformBy(TransformationMatrix2D.Rotation(dir.Angle));
            pts.TransformBy(TransformationMatrix2D.Translation(Center.X, Center.Y));
            PointF[] ptfs = pts.ToPointF();
            using (Pen pen = OutlineStyle.CreatePen(param))
            {
                param.Graphics.DrawLines(pen, ptfs);
            }
        }

        public override Extents GetExtents()
        {
            Extents extents = new Extents();
            extents.Add(X - SemiMajorAxis, Y - SemiMinorAxis);
            extents.Add(X + SemiMajorAxis, Y + SemiMinorAxis);
            return extents;
        }

        public override void TransformBy(TransformationMatrix2D transformation)
        {
            Point2D p = Center;
            p.TransformBy(transformation);
            Center = p;

            dir.TransformBy(transformation);

            Vector2D unit = Vector2D.XAxis;
            unit.TransformBy(transformation);
            SemiMajorAxis = dir.Length * SemiMajorAxis;
            SemiMinorAxis = dir.Length * SemiMinorAxis;
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            Vector2D ptDir = pt - Center;
            float a1 = SemiMajorAxis - pickBoxSize / 2;
            float a2 = SemiMajorAxis + pickBoxSize / 2;
            float b1 = SemiMinorAxis - pickBoxSize / 2;
            float b2 = SemiMinorAxis + pickBoxSize / 2;
            float rot = dir.Angle;
            float xx = (pt.X - X) * MathF.Cos(rot) + (pt.Y - Y) * MathF.Sin(rot);
            float yy = (pt.X - X) * MathF.Sin(rot) - (pt.Y - Y) * MathF.Cos(rot);
            return (xx * xx / a1 / a1 + yy * yy / b1 / b1 >= 1) && (xx * xx / a2 / a2 + yy * yy / b2 / b2 <= 1) &&
                ptDir.IsBetween(Vector2D.FromAngle(StartAngle + rot), Vector2D.FromAngle(EndAngle + rot));
        }
    }
}

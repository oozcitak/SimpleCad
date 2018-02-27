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
            float p = 2 * (float)Math.PI * (3 * (SemiMajorAxis + SemiMinorAxis) - (float)Math.Sqrt((3 * SemiMajorAxis + SemiMinorAxis) * (SemiMajorAxis + 3 * SemiMinorAxis)));
            // Represent curved features by at most 4 pixels
            float sweep = EndAngle - StartAngle;
            while (sweep < 0) sweep += 2 * (float)Math.PI;
            while (sweep > 2 * (float)Math.PI) sweep -= 2 * (float)Math.PI;
            float curveLength = param.ModelToView(sweep / (2 * (float)Math.PI) * p);
            int n = (int)Math.Max(4, curveLength / 4);
            float a = StartAngle;
            float da = sweep / (float)n;
            Point2DCollection pts = new Point2DCollection();
            for (int i = 0; i < n + 1; i++)
            {
                float dx = (float)Math.Cos(a) * SemiMinorAxis;
                float dy = (float)Math.Sin(a) * SemiMajorAxis;
                float t = (float)Math.Atan2(dy, dx);

                float x = SemiMajorAxis * (float)Math.Cos(t);
                float y = SemiMinorAxis * (float)Math.Sin(t);
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
            return;


            System.Drawing.Drawing2D.Matrix orgTr = param.Graphics.Transform;
            param.Graphics.RotateTransform(dir.Angle * 180 / (float)Math.PI, System.Drawing.Drawing2D.MatrixOrder.Append);
            using (Pen pen = OutlineStyle.CreatePen(param))
            {
                param.Graphics.DrawArc(pen, X - SemiMajorAxis, Y - SemiMinorAxis, 2 * SemiMajorAxis, 2 * SemiMinorAxis,
                    StartAngle * 180f / (float)Math.PI, (EndAngle - StartAngle) * 180f / (float)Math.PI);
            }
            param.Graphics.Transform = orgTr;
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
            float xx = (pt.X - X) * (float)Math.Cos(rot) + (pt.Y - Y) * (float)Math.Sin(rot);
            float yy = (pt.X - X) * (float)Math.Sin(rot) - (pt.Y - Y) * (float)Math.Cos(rot);
            return (xx * xx / a1 / a1 + yy * yy / b1 / b1 >= 1) && (xx * xx / a2 / a2 + yy * yy / b2 / b2 <= 1) &&
                ptDir.IsBetween(Vector2D.FromAngle(StartAngle + rot), Vector2D.FromAngle(EndAngle + rot));
        }
    }
}

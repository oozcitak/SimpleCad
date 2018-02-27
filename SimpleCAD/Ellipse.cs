using System;
using System.ComponentModel;
using System.Drawing;

namespace SimpleCAD
{
    public class Ellipse : Drawable
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

        public Ellipse(Point2D center, float semiMajor, float semiMinor)
        {
            Center = center;
            SemiMajorAxis = semiMajor;
            SemiMinorAxis = semiMinor;
            dir = Vector2D.XAxis;
        }

        public Ellipse(float x, float y, float semiMajor, float semiMinor)
            : this(new Point2D(x, y), semiMajor, semiMinor)
        {
            ;
        }

        public override void Draw(DrawParams param)
        {
            // Approximate perimeter (Ramanujan)
            float p = 2 * (float)Math.PI * (3 * (SemiMajorAxis + SemiMinorAxis) - (float)Math.Sqrt((3 * SemiMajorAxis + SemiMinorAxis) * (SemiMajorAxis + 3 * SemiMinorAxis)));
            // Represent curved features by at most 4 pixels
            float curveLength = param.ModelToView(p);
            int n = (int)Math.Max(4, curveLength / 4);
            float da = 2 * (float)Math.PI / (float)n;
            float a = 0;
            Point2DCollection pts = new Point2DCollection();
            for (int i = 0; i < n; i++)
            {
                float x = SemiMajorAxis * (float)Math.Cos(a);
                float y = SemiMinorAxis * (float)Math.Sin(a);
                pts.Add(x, y);
                a += da;
            }
            pts.TransformBy(TransformationMatrix2D.Rotation(dir.Angle));
            pts.TransformBy(TransformationMatrix2D.Translation(Center.X, Center.Y));
            PointF[] ptfs = pts.ToPointF();
            using (Brush brush = FillStyle.CreateBrush(param))
            {
                param.Graphics.FillPolygon(brush, ptfs);
            }
            using (Pen pen = OutlineStyle.CreatePen(param))
            {
                param.Graphics.DrawPolygon(pen, ptfs);
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
            SemiMajorAxis = unit.Length * SemiMajorAxis;
            SemiMinorAxis = unit.Length * SemiMinorAxis;
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            float a1 = SemiMajorAxis - pickBoxSize / 2;
            float a2 = SemiMajorAxis + pickBoxSize / 2;
            float b1 = SemiMinorAxis - pickBoxSize / 2;
            float b2 = SemiMinorAxis + pickBoxSize / 2;
            float rot = dir.Angle;
            float xx = (pt.X - X) * (float)Math.Cos(rot) + (pt.Y - Y) * (float)Math.Sin(rot);
            float yy = (pt.X - X) * (float)Math.Sin(rot) - (pt.Y - Y) * (float)Math.Cos(rot);
            return (xx * xx / a1 / a1 + yy * yy / b1 / b1 >= 1) && (xx * xx / a2 / a2 + yy * yy / b2 / b2 <= 1);
        }
    }
}

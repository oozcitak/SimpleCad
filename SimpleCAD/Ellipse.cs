using System;
using System.Drawing;

namespace SimpleCAD
{
    public class Ellipse : Drawable
    {
        public Point2D Center { get; set; }

        public float X { get { return Center.X; } }
        public float Y { get { return Center.Y; } }

        private Vector2D dir;

        public float SemiMajorAxis { get; set; }
        public float SemiMinorAxis { get; set; }

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
            System.Drawing.Drawing2D.Matrix orgTr = param.Graphics.Transform;
            param.Graphics.RotateTransform(dir.Angle * 180 / (float)Math.PI, System.Drawing.Drawing2D.MatrixOrder.Append);
            using (Brush brush = FillStyle.CreateBrush(param))
            {
                param.Graphics.FillEllipse(brush, X - SemiMajorAxis, Y - SemiMinorAxis, 2 * SemiMajorAxis, 2 * SemiMinorAxis);
            }
            using (Pen pen = OutlineStyle.CreatePen(param))
            {
                param.Graphics.DrawEllipse(pen, X - SemiMajorAxis, Y - SemiMinorAxis, 2 * SemiMajorAxis, 2 * SemiMinorAxis);
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
    }
}

using System;
using System.Drawing;

namespace SimpleCAD
{
    public class EllipticArc : Drawable
    {
        public Point2D P1 { get; set; }
        public Point2D P2 { get; set; }
        public float StartAngle { get; set; }
        public float EndAngle { get; set; }

        public float X1 { get { return P1.X; } }
        public float Y1 { get { return P1.Y; } }
        public float X2 { get { return P2.X; } }
        public float Y2 { get { return P2.Y; } }
        public float Width { get { return Math.Abs(X2 - X1); } }
        public float Height { get { return Math.Abs(Y2 - Y1); } }

        public EllipticArc(Point2D p1, Point2D p2, float startAngle, float endAngle)
        {
            P1 = p1;
            P2 = p2;
            StartAngle = startAngle;
            EndAngle = endAngle;
        }

        public EllipticArc(float x1, float y1, float x2, float y2, float startAngle, float endAngle)
            : this(new Point2D(x1, y1), new Point2D(x2, y2), startAngle, endAngle)
        {
            ;
        }

        public override void Draw(DrawParams param)
        {
            using (Pen pen = OutlineStyle.CreatePen(param))
            {
                param.Graphics.DrawArc(pen, X1, Y1, Width, Height, StartAngle * 180f / (float)Math.PI, (EndAngle - StartAngle) * 180f / (float)Math.PI);
            }
        }

        public override Extents GetExtents()
        {
            Extents extents = new Extents();
            extents.Add(X1, Y1);
            extents.Add(X2, Y2);
            return extents;
        }

        public override void TransformBy(TransformationMatrix2D transformation)
        {
            Point2D p1 = P1;
            Point2D p2 = P2;
            p1.TransformBy(transformation);
            p2.TransformBy(transformation);
            P1 = p1;
            P2 = p2;

            Vector2D a1 = Vector2D.FromAngle(StartAngle);
            Vector2D a2 = Vector2D.FromAngle(EndAngle);
            a1.TransformBy(transformation);
            a2.TransformBy(transformation);
            StartAngle = a1.Angle;
            EndAngle = a2.Angle;
        }
    }
}

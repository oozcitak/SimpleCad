using System;
using System.Drawing;

namespace SimpleCAD
{
    public class Triangle : Drawable
    {
        public Point2D P1 { get; set; }
        public Point2D P2 { get; set; }
        public Point2D P3 { get; set; }

        public float X1 { get { return P1.X; } }
        public float Y1 { get { return P1.Y; } }
        public float X2 { get { return P2.X; } }
        public float Y2 { get { return P2.Y; } }
        public float X3 { get { return P3.X; } }
        public float Y3 { get { return P3.Y; } }

        public Triangle(Point2D p1, Point2D p2, Point2D p3)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }

        public Triangle(float x1, float y1, float x2, float y2, float x3, float y3)
            : this(new Point2D(x1, y1), new Point2D(x2, y2), new Point2D(x3, y3))
        {
            ;
        }

        public override void Draw(DrawParams param)
        {
            PointF[] points = new PointF[3];
            points[0] = new PointF(X1, Y1);
            points[1] = new PointF(X2, Y2);
            points[2] = new PointF(X3, Y3);
            using (Brush brush = FillStyle.CreateBrush(param))
            {
                param.Graphics.FillPolygon(brush, points);
            }
            using (Pen pen = OutlineStyle.CreatePen(param))
            {
                param.Graphics.DrawPolygon(pen, points);
            }
        }

        public override Extents GetExtents()
        {
            Extents extents = new Extents();
            extents.Add(X1, Y1);
            extents.Add(X2, Y2);
            extents.Add(X3, Y3);
            return extents;
        }

        public override void TransformBy(Matrix2D transformation)
        {
            Point2D p1 = P1;
            Point2D p2 = P2;
            Point2D p3 = P3;
            p1.TransformBy(transformation);
            p2.TransformBy(transformation);
            p3.TransformBy(transformation);
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }
    }
}

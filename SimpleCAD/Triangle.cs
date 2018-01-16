using System;
using System.Drawing;

namespace SimpleCAD
{
    public class Triangle : Drawable
    {
        private Point2DCollection points;

        public Point2D P1 { get { return points[0]; } set { points[0] = value; } }
        public Point2D P2 { get { return points[1]; } set { points[1] = value; } }
        public Point2D P3 { get { return points[2]; } set { points[2] = value; } }

        public float X1 { get { return P1.X; } }
        public float Y1 { get { return P1.Y; } }
        public float X2 { get { return P2.X; } }
        public float Y2 { get { return P2.Y; } }
        public float X3 { get { return P3.X; } }
        public float Y3 { get { return P3.Y; } }

        public Triangle(Point2D p1, Point2D p2, Point2D p3)
        {
            points = new Point2DCollection();
            points.Add(p1);
            points.Add(p2);
            points.Add(p3);
        }

        public Triangle(float x1, float y1, float x2, float y2, float x3, float y3)
            : this(new Point2D(x1, y1), new Point2D(x2, y2), new Point2D(x3, y3))
        {
            ;
        }

        public override void Draw(DrawParams param)
        {
            PointF[] ptf = points.ToPointF();
            using (Brush brush = FillStyle.CreateBrush(param))
            {
                param.Graphics.FillPolygon(brush, ptf);
            }
            using (Pen pen = OutlineStyle.CreatePen(param))
            {
                param.Graphics.DrawPolygon(pen, ptf);
            }
        }

        public override Extents GetExtents()
        {
            return points.GetExtents();
        }

        public override void TransformBy(Matrix2D transformation)
        {
            points.TransformBy(transformation);
        }
    }
}

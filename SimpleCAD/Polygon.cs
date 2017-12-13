using System;
using System.Drawing;

namespace SimpleCAD
{
    public class Polygon : Drawable
    {
        public Point2D[] Points { get; set; }

        public Polygon(Point2D[] points)
        {
            Points = points;
        }

        public Polygon(System.Drawing.PointF[] points)
        {
            Points = new Point2D[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                Points[i] = new Point2D(points[i].X, points[i].Y);
            }
        }

        public override void Draw(DrawParams param)
        {
            PointF[] points = new PointF[Points.Length];
            for (int i = 0; i < Points.Length; i++)
            {
                points[i] = new PointF(Points[i].X, Points[i].Y);
            }

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
            foreach (Point2D point in Points)
            {
                extents.Add(point.X, point.Y);
            }
            return extents;
        }

        public override void TransformBy(Matrix2D transformation)
        {
            for (int i = 0; i < Points.Length; i++)
            {
                Point2D p = Points[i];
                p.TransformBy(transformation);
                Points[i] = p;
            }
        }
    }
}

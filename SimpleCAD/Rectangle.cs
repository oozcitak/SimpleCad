using System;
using System.ComponentModel;
using System.Drawing;

namespace SimpleCAD
{
    public class Rectangle : Drawable
    {
        private Point2DCollection points;

        public Point2D P1 { get { return points[0]; } set { points[0] = value; NotifyPropertyChanged(); } }
        public Point2D P2 { get { return points[1]; } set { points[1] = value; NotifyPropertyChanged(); } }

        [Browsable(false)]
        public float X1 { get { return P1.X; } }
        [Browsable(false)]
        public float Y1 { get { return P1.Y; } }
        [Browsable(false)]
        public float X2 { get { return P2.X; } }
        [Browsable(false)]
        public float Y2 { get { return P2.Y; } }

        [Browsable(false)]
        public float Width { get { return Math.Abs(X2 - X1); } }
        [Browsable(false)]
        public float Height { get { return Math.Abs(Y2 - Y1); } }

        public Rectangle(Point2D p1, Point2D p2)
        {
            points = new Point2DCollection();
            points.Add(p1);
            points.Add(p2.X, p1.Y);
            points.Add(p2);
            points.Add(p1.X, p2.Y);
        }

        public Rectangle(float x1, float y1, float x2, float y2)
            : this(new Point2D(x1, y1), new Point2D(x2, y2))
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

        public override void TransformBy(TransformationMatrix2D transformation)
        {
            points.TransformBy(transformation);
        }
    }
}

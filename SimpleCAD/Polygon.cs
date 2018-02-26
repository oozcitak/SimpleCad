using System;
using System.Drawing;

namespace SimpleCAD
{
    public class Polygon : Drawable
    {
        public Point2DCollection Points { get; private set; }

        public Polygon()
        {
            Points = new Point2DCollection();
            Points.CollectionChanged += Points_CollectionChanged;
        }

        public Polygon(Point2D[] pts)
        {
            Points = new Point2DCollection(pts);
            Points.CollectionChanged += Points_CollectionChanged;
        }

        public Polygon(PointF[] pts)
        {
            Points = new Point2DCollection(pts);
            Points.CollectionChanged += Points_CollectionChanged;
        }

        private void Points_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("Points");
        }

        public override void Draw(DrawParams param)
        {
            if (Points.Count > 0)
            {
                PointF[] pts = Points.ToPointF();
                using (Brush brush = FillStyle.CreateBrush(param))
                {
                    param.Graphics.FillPolygon(brush, pts);
                }

                using (Pen pen = OutlineStyle.CreatePen(param))
                {
                    param.Graphics.DrawPolygon(pen, pts);
                }
            }
        }

        public override Extents GetExtents()
        {
            return Points.GetExtents();
        }

        public override void TransformBy(TransformationMatrix2D transformation)
        {
            Points.TransformBy(transformation);
        }
    }
}

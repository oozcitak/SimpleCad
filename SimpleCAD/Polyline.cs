using System;
using System.Drawing;

namespace SimpleCAD
{
    public class Polyline : Drawable
    {
        private bool closed;

        public Point2DCollection Points { get; private set; }
        public bool Closed { get { return closed; } set { closed = value; NotifyPropertyChanged(); } }

        public Polyline()
        {
            Points = new Point2DCollection();
            Points.CollectionChanged += Points_CollectionChanged;
        }

        public Polyline(Point2DCollection pts)
        {
            Points = new Point2DCollection(pts);
            Points.CollectionChanged += Points_CollectionChanged;
        }

        public Polyline(Point2D[] pts)
        {
            Points = new Point2DCollection(pts);
            Points.CollectionChanged += Points_CollectionChanged;
        }

        public Polyline(PointF[] pts)
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
                using (Pen pen = OutlineStyle.CreatePen(param))
                {
                    if (Closed)
                        param.Graphics.DrawPolygon(pen, pts);
                    else
                        param.Graphics.DrawLines(pen, pts);
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

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            int iend = (Closed ? Points.Count - 1 : Points.Count - 2);
            for (int i = 0; i <= iend; i++)
            {
                int j = (i == Points.Count - 1 ? 0 : i + 1);
                Line line = new Line(Points[i], Points[j]);
                if (line.Contains(pt, pickBoxSize))
                    return true;
            }
            return false;
        }

        public override Drawable Clone()
        {
            Polyline newPolyline = (Polyline)base.Clone();
            newPolyline.Points = new Point2DCollection(Points);
            return newPolyline;
        }
    }
}

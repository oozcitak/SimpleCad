using SimpleCAD.Geometry;
using System.Drawing;
using System.IO;

namespace SimpleCAD.Drawables
{
    public class Polyline : Drawable
    {
        private bool closed;

        public Point2DCollection Points { get; private set; }
        public virtual bool Closed { get { return closed; } set { closed = value; NotifyPropertyChanged(); } }

        public float Length
        {
            get
            {
                float len = 0;
                for (int i = 0; i < (Closed ? Points.Count : Points.Count - 1); i++)
                {
                    int j = (i == Points.Count - 1 ? 0 : i + 1);
                    len += (Points[j] - Points[i]).Length;
                }
                return len;
            }
        }

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

        public Polyline(params Point2D[] pts)
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
                using (Pen pen = Style.CreatePen(param))
                {
                    if (Closed)
                        param.Graphics.DrawPolygon(pen, pts);
                    else
                        param.Graphics.DrawLines(pen, pts);
                }
            }
        }

        public override Extents2D GetExtents()
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

        public override ControlPoint[] GetControlPoints()
        {
            ControlPoint[] cp = new ControlPoint[Points.Count];
            for (int i = 0; i < Points.Count; i++)
            {
                cp[i] = new ControlPoint("Points", i);
            }
            return cp;
        }

        public Polyline(BinaryReader reader) : base(reader)
        {
            Points = new Point2DCollection(reader);
            Points.CollectionChanged += Points_CollectionChanged;
        }

        public override void Save(BinaryWriter writer)
        {
            base.Save(writer);
            Points.Save(writer);
        }
    }
}

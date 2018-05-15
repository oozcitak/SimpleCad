using SimpleCAD.Geometry;
using SimpleCAD.Graphics;
using System.Drawing;

namespace SimpleCAD.Drawables
{
    public class Polyline : Curve
    {
        private bool closed;

        public Point2DCollection Points { get; private set; }
        public virtual bool Closed { get { return closed; } set { closed = value; NotifyPropertyChanged(); } }

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

        public override void Draw(Renderer renderer)
        {
            renderer.DrawPolyline(Style.ApplyLayer(Layer), Points, Closed);
        }

        public override Extents2D GetExtents()
        {
            return Points.GetExtents();
        }

        public override void TransformBy(Matrix2D transformation)
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
                cp[i] = new ControlPoint("Vertex (" + (i + 1).ToString() + ")", Points[i]);
            }
            return cp;
        }

        public override SnapPoint[] GetSnapPoints()
        {
            SnapPoint[] cp = new SnapPoint[Points.Count];
            for (int i = 0; i < Points.Count; i++)
            {
                cp[i] = new SnapPoint("Vertex (" + (i + 1).ToString() + ")", Points[i]);
            }
            return cp;
        }

        public override void TransformControlPoints(int[] indices, Matrix2D transformation)
        {
            foreach (int index in indices)
            {
                Points[index] = Points[index].Transform(transformation);
            }
        }

        public override void Load(DocumentReader reader)
        {
            base.Load(reader);
            Points = reader.ReadPoint2DCollection();
            Points.CollectionChanged += Points_CollectionChanged;
        }

        public override void Save(DocumentWriter writer)
        {
            base.Save(writer);
            writer.Write(Points);
        }

        public override float StartParam => 0;
        public override float EndParam => Points.Count - 1;

        public override float GetDistAtParam(float param)
        {
            param = MathF.Clamp(param, StartParam, EndParam);
            float dist = 0;
            var lastPt = Points[0];
            for (int i = 1; i < Points.Count; i++)
            {
                if (param >= i)
                {
                    dist += (Points[i] - lastPt).Length;
                }
                else
                {
                    dist += (param - (i - 1)) * (Points[i] - lastPt).Length;
                    return dist;
                }
                lastPt = Points[i];
            }
            return dist;
        }

        public override Point2D GetPointAtParam(float param)
        {
            param = MathF.Clamp(param, StartParam, EndParam);
            for (int i = 0; i < Points.Count; i++)
            {
                if (MathF.IsEqual(param, i))
                    return Points[i];
                else if (param < i)
                    return Points[i] + (param - (i - 1)) * (Points[i] - Points[i - 1]);
            }
            return Points[Points.Count - 1];
        }

        public override Vector2D GetNormalAtParam(float param)
        {
            param = MathF.Clamp(param, StartParam, EndParam);
            for (int i = 1; i < Points.Count; i++)
            {
                if (param <= i)
                    return (Points[i] - Points[i - 1]).Perpendicular;
            }
            return (Points[Points.Count - 1] - Points[Points.Count - 2]).Perpendicular;
        }
    }
}

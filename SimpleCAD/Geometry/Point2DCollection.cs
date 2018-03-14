using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace SimpleCAD.Geometry
{
    public class Point2DCollection : IList<Point2D>, INotifyCollectionChanged, IPersistable
    {
        private List<Point2D> items;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public Point2DCollection()
        {
            items = new List<Point2D>();
        }

        public Point2DCollection(IEnumerable<Point2D> elements)
        {
            items = new List<Point2D>(elements);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items));
        }

        public Point2DCollection(IEnumerable<System.Drawing.PointF> elements)
        {
            items = new List<Point2D>();
            foreach (System.Drawing.PointF item in elements)
            {
                items.Add(new Point2D(item));
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items));
        }

        public Extents2D GetExtents()
        {
            Extents2D ex = new Extents2D();
            foreach (Point2D item in items)
            {
                ex.Add(item);
            }
            return ex;
        }

        public Point2D this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items[index]));
                items[index] = value;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items[index]));
            }
        }

        public void Add(float x, float y)
        {
            Point2D item = new Point2D(x, y);
            items.Add(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void Add(Point2D item)
        {
            items.Add(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void AddRange(IEnumerable<Point2D> list)
        {
            items.AddRange(items);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items));
        }

        public void Clear()
        {
            items.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(Point2D item)
        {
            return items.Contains(item);
        }

        public void CopyTo(Point2D[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Point2D item)
        {
            bool check = items.Remove(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return check;
        }

        public IEnumerator<Point2D> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public System.Drawing.PointF[] ToPointF()
        {
            System.Drawing.PointF[] points = new System.Drawing.PointF[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                points[i] = (System.Drawing.PointF)items[i];
            }
            return points;
        }

        public void TransformBy(Matrix2D transformation)
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i] = items[i].Transform(transformation);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        public int IndexOf(Point2D item)
        {
            return items.IndexOf(item);
        }

        public void Insert(int index, Point2D item)
        {
            items.Insert(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void RemoveAt(int index)
        {
            items.RemoveAt(index);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items[index]));
        }

        public Point2DCollection(BinaryReader reader) : this()
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                Point2D point = new Point2D(reader);
                items.Add(point);
            }
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(items.Count);
            foreach (Point2D pt in items)
            {
                pt.Save(writer);
            }
        }
    }
}

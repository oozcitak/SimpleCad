using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace SimpleCAD
{
    public class Point2DCollection : ICollection<Point2D>, INotifyCollectionChanged
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
                points[i] = items[i].ToPointF();
            }
            return points;
        }

        public void TransformBy(TransformationMatrix2D transformation)
        {
            for (int i = 0; i < items.Count; i++)
            {
                Point2D pt = items[i];
                pt.TransformBy(transformation);
                items[i] = pt;
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }
    }
}

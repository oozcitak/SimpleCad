using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleCAD
{
    public class Point2DCollection : ICollection<Point2D>
    {
        private List<Point2D> items;

        public Point2DCollection()
        {
            items = new List<Point2D>();
        }

        public Point2DCollection(IEnumerable<Point2D> elements)
        {
            items = new List<Point2D>(elements);
        }

        public Point2DCollection(IEnumerable<System.Drawing.PointF> elements)
        {
            items = new List<Point2D>();
            foreach (System.Drawing.PointF item in elements)
            {
                items.Add(new Point2D(item));
            }
        }

        public Extents GetExtents()
        {
            Extents ex = new Extents();
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
                items[index] = value;
            }
        }

        public void Add(float x, float y)
        {
            items.Add(new Point2D(x, y));
        }

        public void Add(Point2D item)
        {
            items.Add(item);
        }

        public void AddRange(IEnumerable<Point2D> list)
        {
            items.AddRange(items);
        }

        public void Clear()
        {
            items.Clear();
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
            return items.Remove(item);
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

        public void TransformBy(Matrix2D transformation)
        {
            for (int i = 0; i < items.Count; i++)
            {
                Point2D pt = items[i];
                pt.TransformBy(transformation);
                items[i] = pt;
            }
        }
    }
}

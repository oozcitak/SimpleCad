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

        public static implicit operator System.Drawing.PointF[] (Point2DCollection coll)
        {
            System.Drawing.PointF[] items = new System.Drawing.PointF[coll.items.Count];
            for (int i = 0; i < coll.items.Count; i++)
            {
                items[i] = coll.items[i];
            }
            return items;
        }

        public static implicit operator Point2D[] (Point2DCollection coll)
        {
            return coll.items.ToArray();
        }

        public static implicit operator Point2DCollection(Point2D[] items)
        {
            return new Point2DCollection(items);
        }

        public static implicit operator Point2DCollection(System.Drawing.PointF[] items)
        {
            Point2DCollection coll = new Point2DCollection();
            foreach (System.Drawing.PointF item in items)
            {
                coll.Add(item);
            }
            return coll;
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

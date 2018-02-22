using System;
using System.Collections.Generic;
using System.Drawing;

namespace SimpleCAD
{
    public class Composite : Drawable, ICollection<Drawable>
    {
        List<Drawable> items = new List<Drawable>();

        public Composite()
        {
        }

        public override void Draw(DrawParams param)
        {
            foreach (Drawable item in items)
            {
                if (item.Visible) item.Draw(param);
            }
        }

        public override Extents GetExtents()
        {
            Extents extents = new Extents();
            foreach (Drawable item in items)
            {
                if (item.Visible) extents.Add(item.GetExtents());
            }
            return extents;
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            foreach (Drawable d in items)
            {
                if (d.Contains(pt, pickBoxSize)) return true;
            }
            return false;
        }

        public override void TransformBy(TransformationMatrix2D transformation)
        {
            foreach (Drawable item in items)
            {
                item.TransformBy(transformation);
            }
        }

        public void Add(Drawable item)
        {
            items.Add(item);
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(Drawable item)
        {
            return items.Contains(item);
        }

        public void CopyTo(Drawable[] array, int arrayIndex)
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

        public bool Remove(Drawable item)
        {
            return items.Remove(item);
        }

        public IEnumerator<Drawable> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

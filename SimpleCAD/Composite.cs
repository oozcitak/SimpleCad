using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace SimpleCAD
{
    public class Composite : Drawable, ICollection<Drawable>, INotifyCollectionChanged
    {
        List<Drawable> items = new List<Drawable>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public Composite()
        {
            ;
        }

        public override void Draw(DrawParams param)
        {
            foreach (Drawable item in items)
            {
                if (item.Visible)
                {
                    item.Draw(param);
                }
            }
        }

        public override Extents2D GetExtents()
        {
            Extents2D extents = new Extents2D();
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

        public override Drawable Clone()
        {
            Composite newComposite = (Composite)base.Clone();
            foreach(Drawable d in items)
            {
                newComposite.Add(d.Clone());
            }
            return newComposite;
        }

        public void CopyStyleToChildren()
        {
            foreach (Drawable item in items)
            {
                item.Outline = Outline;
                if (item is Composite comp)
                {
                    comp.CopyStyleToChildren();
                }
            }
        }

        public void Add(Drawable item)
        {
            items.Add(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void Clear()
        {
            items.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
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
            bool check = items.Remove(item);
            if (check)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            }
            return check;
        }

        public IEnumerator<Drawable> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Drawable item in e.NewItems)
                    item.PropertyChanged += Drawable_PropertyChanged;
            }
            if (e.OldItems != null)
            {
                foreach (Drawable item in e.OldItems)
                    item.PropertyChanged -= Drawable_PropertyChanged;
            }

            CollectionChanged?.Invoke(this, e);
        }

        void Drawable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender));
        }
    }
}

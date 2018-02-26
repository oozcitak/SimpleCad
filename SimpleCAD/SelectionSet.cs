using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SimpleCAD
{
    public class SelectionSet : ISet<Drawable>, INotifyCollectionChanged
    {
        HashSet<Drawable> items = new HashSet<Drawable>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public SelectionSet()
        {
            ;
        }

        public bool Add(Drawable item)
        {
            bool check = items.Add(item);
            if (check)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            }
            return check;
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

        public bool IsReadOnly => throw new NotImplementedException();

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
            CollectionChanged?.Invoke(this, e);
        }

        public void UnionWith(IEnumerable<Drawable> other)
        {
            items.UnionWith(other);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, other));
        }

        public void IntersectWith(IEnumerable<Drawable> other)
        {
            items.IntersectWith(other);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace));
        }

        public void ExceptWith(IEnumerable<Drawable> other)
        {
            items.ExceptWith(other);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
        }

        public void SymmetricExceptWith(IEnumerable<Drawable> other)
        {
            items.SymmetricExceptWith(other);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace));
        }

        public bool IsSubsetOf(IEnumerable<Drawable> other)
        {
            return items.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<Drawable> other)
        {
            return items.IsSupersetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<Drawable> other)
        {
            return items.IsProperSupersetOf(other);
        }

        public bool IsProperSubsetOf(IEnumerable<Drawable> other)
        {
            return items.IsProperSubsetOf(other);
        }

        public bool Overlaps(IEnumerable<Drawable> other)
        {
            return items.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<Drawable> other)
        {
            return items.SetEquals(other);
        }

        void ICollection<Drawable>.Add(Drawable item)
        {
            ((ISet<Drawable>)items).Add(item);
        }
    }
}

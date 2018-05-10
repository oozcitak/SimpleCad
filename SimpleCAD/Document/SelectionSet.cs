using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace SimpleCAD
{
    public sealed class SelectionSet : ISet<Drawable>, INotifyCollectionChanged
    {
        HashSet<Drawable> items = new HashSet<Drawable>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public static SelectionSet Empty => new SelectionSet();

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

        public int Count { get { return items.Count; } }

        public bool IsReadOnly { get { return false; } }

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

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        public void UnionWith(IEnumerable<Drawable> other)
        {
            foreach(var item in other)
            {
                Add(item);
            }
        }

        #region Not Implemented
        void ISet<Drawable>.IntersectWith(IEnumerable<Drawable> other)
        {
            throw new NotImplementedException();
        }

        void ISet<Drawable>.ExceptWith(IEnumerable<Drawable> other)
        {
            throw new NotImplementedException();
        }

        void ISet<Drawable>.SymmetricExceptWith(IEnumerable<Drawable> other)
        {
            throw new NotImplementedException();
        }

        bool ISet<Drawable>.IsSubsetOf(IEnumerable<Drawable> other)
        {
            throw new NotImplementedException();
        }

        bool ISet<Drawable>.IsSupersetOf(IEnumerable<Drawable> other)
        {
            throw new NotImplementedException();
        }

        bool ISet<Drawable>.IsProperSupersetOf(IEnumerable<Drawable> other)
        {
            throw new NotImplementedException();
        }

        bool ISet<Drawable>.IsProperSubsetOf(IEnumerable<Drawable> other)
        {
            throw new NotImplementedException();
        }

        bool ISet<Drawable>.Overlaps(IEnumerable<Drawable> other)
        {
            throw new NotImplementedException();
        }

        bool ISet<Drawable>.SetEquals(IEnumerable<Drawable> other)
        {
            throw new NotImplementedException();
        }

        void ICollection<Drawable>.Add(Drawable item)
        {
            ((ISet<Drawable>)this).Add(item);
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleCAD
{
    public class CADModel : IList<Drawable>
    {
        private List<Drawable> items;

        public CADModel()
        {
            items = new List<Drawable>();
        }

        public int IndexOf(Drawable item)
        {
            return items.IndexOf(item);
        }

        public void Insert(int index, Drawable item)
        {
            items.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            items.RemoveAt(index);
        }

        public Drawable this[int index]
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

        public void Add(Drawable item)
        {
            items.Add(item);
        }

        public void AddRange(IEnumerable<Drawable> item)
        {
            items.AddRange(item);
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

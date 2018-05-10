using System.Collections.Generic;
using System.Linq;

namespace SimpleCAD
{
    public sealed class CPSelectionSet
    {
        Dictionary<Drawable, HashSet<int>> items = new Dictionary<Drawable, HashSet<int>>();

        public static CPSelectionSet Empty => new CPSelectionSet();

        public int[] this[Drawable item] => items[item].ToArray();

        public CPSelectionSet()
        {
            ;
        }

        public bool Add(Drawable item, int index)
        {
            if (items.TryGetValue(item, out var indices))
            {
                return indices.Add(index);
            }
            else
            {
                items.Add(item, new HashSet<int>() { index });
                return true;
            }
        }

        public void Add(Drawable item, IEnumerable<int> indices)
        {
            foreach (int index in indices)
            {
                Add(item, index);
            }
        }

        public void Clear()
        {
            items.Clear();
        }

        public int Count { get { return items.Count; } }

        public IEnumerator<KeyValuePair<Drawable, HashSet<int>>> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public void UnionWith(CPSelectionSet other)
        {
            foreach (var item in other)
            {
                foreach (int index in item.Value)
                {
                    Add(item.Key, index);
                }
            }
        }

        public SelectionSet ToSelectionSet()
        {
            SelectionSet ss = new SelectionSet();
            foreach (var pair in items)
            {
                ss.Add(pair.Key);
            }
            return ss;
        }
    }
}

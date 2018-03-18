using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SimpleCAD.Geometry;

namespace SimpleCAD.Drawables
{
    public class DrawableDictionary : Drawable, IDictionary<string, Drawable>
    {
        Dictionary<string, Drawable> items = new Dictionary<string, Drawable>();

        public Drawable this[string key] { get => items[key]; set => items[key] = value; }
        public ICollection<string> Keys => items.Keys;
        public ICollection<Drawable> Values => items.Values;
        public int Count => items.Count;
        public bool IsReadOnly => false;

        public DrawableDictionary()
        {
            ;
        }

        public DrawableDictionary(BinaryReader reader) : base(reader)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string key = reader.ReadString();
                string name = reader.ReadString();
                Type itemType = Type.GetType(name);
                Drawable item = (Drawable)Activator.CreateInstance(itemType, reader);
                items.Add(key, item);
            }
        }

        public override void Save(BinaryWriter writer)
        {
            base.Save(writer);
            writer.Write(items.Count);
            foreach (KeyValuePair<string, Drawable> item in items)
            {
                writer.Write(item.Key);
                writer.Write(item.Value.GetType().FullName);
                item.Value.Save(writer);
            }
        }

        public void Add(string key, Drawable value)
        {
            items.Add(key, value);
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool ContainsKey(string key)
        {
            return items.ContainsKey(key);
        }

        public override void Draw(Renderer renderer)
        {
            foreach (Drawable item in items.Values)
            {
                if (item.Visible)
                {
                    renderer.Draw(item);
                }
            }
        }

        public override Extents2D GetExtents()
        {
            Extents2D extents = new Extents2D();
            foreach (Drawable item in items.Values)
            {
                if (item.Visible) extents.Add(item.GetExtents());
            }
            return extents;
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            foreach (Drawable d in items.Values)
            {
                if (d.Contains(pt, pickBoxSize)) return true;
            }
            return false;
        }

        public override void TransformBy(Matrix2D transformation)
        {
            foreach (Drawable item in items.Values)
            {
                item.TransformBy(transformation);
            }
        }

        public override Drawable Clone()
        {
            DrawableDictionary newDict = (DrawableDictionary)base.Clone();
            foreach (KeyValuePair<string, Drawable> item in items)
            {
                newDict.Add(item.Key, item.Value.Clone());
            }
            return newDict;
        }

        public IEnumerator<KeyValuePair<string, Drawable>> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return items.Remove(key);
        }

        public bool TryGetValue(string key, out Drawable value)
        {
            return items.TryGetValue(key, out value);
        }

        void ICollection<KeyValuePair<string, Drawable>>.Add(KeyValuePair<string, Drawable> item)
        {
            items.Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<string, Drawable>>.Contains(KeyValuePair<string, Drawable> item)
        {
            return items.Contains(item);
        }

        void ICollection<KeyValuePair<string, Drawable>>.CopyTo(KeyValuePair<string, Drawable>[] array, int arrayIndex)
        {
            items.ToArray().CopyTo(array, arrayIndex);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool ICollection<KeyValuePair<string, Drawable>>.Remove(KeyValuePair<string, Drawable> item)
        {
            if (items.Contains(item))
                return items.Remove(item.Key);
            else
                return false;
        }
    }
}

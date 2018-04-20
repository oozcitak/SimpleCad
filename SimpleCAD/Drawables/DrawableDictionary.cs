using SimpleCAD.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCAD.Drawables
{
    public class DrawableDictionary : Drawable, IDict<Drawable>
    {
        Dictionary<string, Drawable> items = new Dictionary<string, Drawable>();

        public Drawable this[string key] { get => items[key]; set => items[key] = value; }
        public ICollection<string> Keys => items.Keys;
        public ICollection<Drawable> Values => items.Values;
        public int Count => items.Count;
        public bool IsReadOnly => false;

        public DrawableDictionary() { }

        public override void Load(DocumentReader reader)
        {
            base.Load(reader);
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                string key = reader.ReadString();
                string name = reader.ReadString();
                Type itemType = Type.GetType(name);
                Drawable item = (Drawable)Activator.CreateInstance(itemType, reader);
                items.Add(key, item);
            }
        }

        public override void Save(DocumentWriter writer)
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
                if (item.Visible && (item.Layer == null || item.Layer.Visible))
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
                if (item.Visible && item.Layer.Visible)
                    extents.Add(item.GetExtents());
            }
            return extents;
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            foreach (Drawable d in items.Values)
            {
                if (d.Visible && d.Layer.Visible && d.Contains(pt, pickBoxSize)) return true;
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

        public IEnumerator<Drawable> GetEnumerator()
        {
            foreach (Drawable item in items.Values)
                yield return item;
        }

        public bool Remove(string key)
        {
            return items.Remove(key);
        }

        public bool TryGetValue(string key, out Drawable value)
        {
            return items.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

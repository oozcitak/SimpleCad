using SimpleCAD.Geometry;
using SimpleCAD.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SimpleCAD.Drawables
{
    public class DrawableList : Drawable, IList<Drawable>
    {
        List<Drawable> items = new List<Drawable>();

        public Drawable this[int index] { get => items[index]; set => items[index] = value; }
        public int Count => items.Count;
        public bool IsReadOnly => false;

        public DrawableList() { }

        public override void Load(DocumentReader reader)
        {
            base.Load(reader);
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                string name = reader.ReadString();
                Type itemType = Type.GetType(name);
                Drawable item = (Drawable)Activator.CreateInstance(itemType, reader);
                items.Add(item);
            }
        }

        public override void Save(DocumentWriter writer)
        {
            base.Save(writer);
            writer.Write(items.Count);
            foreach (Drawable item in items)
            {
                writer.Write(item.GetType().FullName);
                item.Save(writer);
            }
        }

        public void Add(Drawable value)
        {
            items.Add(value);
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(Drawable item)
        {
            return items.Contains(item);
        }

        public override void Draw(Renderer renderer)
        {
            foreach (Drawable item in items)
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
            foreach (Drawable item in items)
            {
                if (item.Visible && item.Layer.Visible)
                    extents.Add(item.GetExtents());
            }
            return extents;
        }

        public override bool Contains(Point2D pt, float pickBoxSize)
        {
            foreach (Drawable d in items)
            {
                if (d.Visible && d.Layer.Visible && d.Contains(pt, pickBoxSize)) return true;
            }
            return false;
        }

        public override void TransformBy(Matrix2D transformation)
        {
            foreach (Drawable item in items)
            {
                item.TransformBy(transformation);
            }
        }

        public override Drawable Clone()
        {
            DrawableList newList = (DrawableList)base.Clone();
            foreach (Drawable item in items)
            {
                newList.Add(item.Clone());
            }
            return newList;
        }

        public IEnumerator<Drawable> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public bool Remove(Drawable item)
        {
            return items.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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

        public void CopyTo(Drawable[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }
    }
}

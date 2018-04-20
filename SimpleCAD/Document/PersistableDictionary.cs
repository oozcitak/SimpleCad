using System.Collections;
using System.Collections.Generic;

namespace SimpleCAD
{
    public abstract class PersistableDictionary<TValue> : IPersistable, IDict<TValue> where TValue : IPersistable, new()
    {
        Dictionary<string, TValue> dict;

        public virtual TValue this[string key] { get => dict[key]; set => dict[key] = value; }

        public virtual int Count => dict.Count;

        public PersistableDictionary()
        {
            dict = new Dictionary<string, TValue>();
        }

        public virtual void Load(DocumentReader reader)
        {
            dict = new Dictionary<string, TValue>();
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                string key = reader.ReadString();
                TValue value = new TValue();
                value.Load(reader);
                dict.Add(key, value);
            }
        }

        public virtual void Save(DocumentWriter writer)
        {
            writer.Write(dict.Count);
            foreach (var pair in dict)
            {
                writer.Write(pair.Key);
                pair.Value.Save(writer);
            }
        }

        public virtual void Add(string key, TValue value)
        {
            dict.Add(key, value);
        }

        public virtual void Clear()
        {
            dict.Clear();
        }

        public virtual bool ContainsKey(string key)
        {
            return dict.ContainsKey(key);
        }

        public virtual IEnumerator<TValue> GetEnumerator()
        {
            foreach (TValue value in dict.Values)
                yield return value;
        }

        public virtual bool Remove(string key)
        {
            return dict.Remove(key);
        }

        public virtual bool TryGetValue(string key, out TValue value)
        {
            return dict.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

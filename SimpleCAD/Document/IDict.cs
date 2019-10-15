using System.Collections.Generic;

namespace SimpleCAD
{
    public interface IDict<TValue> : IEnumerable<TValue>
    {
        int Count { get; }
        TValue this[string key] { get; set; }

        void Add(string key, TValue value);
        void Clear();
        bool ContainsKey(string key);
        bool Remove(string key);
        bool TryGetValue(string key, out TValue value);
    }
}

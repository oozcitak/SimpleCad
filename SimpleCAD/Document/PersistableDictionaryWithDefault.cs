using System;

namespace SimpleCAD
{
    public abstract class PersistableDictionaryWithDefault<TValue> : PersistableDictionary<TValue> where TValue : IPersistable, new()
    {
        public string DefaultKey { get; private set; }
        public TValue Default { get => dict[DefaultKey]; }

        public PersistableDictionaryWithDefault(string defaultKey, TValue defaultValue) : base()
        {
            DefaultKey = defaultKey;
            dict.Add(defaultKey, defaultValue);
        }

        public TValue GetOrDefault(string key)
        {
            if (dict.TryGetValue(key, out var value))
                return value;
            else
                return Default;
        }

        public override void Clear()
        {
            TValue defaultValue = dict[DefaultKey];
            dict.Clear();
            dict.Add(DefaultKey, defaultValue);
        }

        public override bool Remove(string key)
        {
            if(dict.Comparer.Equals(key, DefaultKey))
                return false;
            else
                return dict.Remove(key);
        }
    }
}

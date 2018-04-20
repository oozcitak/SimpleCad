using System;

namespace SimpleCAD
{
    public abstract class PersistableDictionaryWithDefault<TValue> : PersistableDictionary<TValue> where TValue : IPersistable, new()
    {
        public string DefaultKey { get; private set; }
        public TValue Default { get => this[DefaultKey]; }

        public PersistableDictionaryWithDefault(string defaultKey, TValue defaultValue) : base()
        {
            DefaultKey = defaultKey;
            Add(defaultKey, defaultValue);
        }

        public TValue GetOrDefault(string key)
        {
            if (TryGetValue(key, out var value))
                return value;
            else
                return Default;
        }

        public override void Clear()
        {
            TValue defaultValue = this[DefaultKey];
            base.Clear();
            Add(DefaultKey, defaultValue);
        }

        public override bool Remove(string key)
        {
            if (string.Equals(key, DefaultKey, StringComparison.OrdinalIgnoreCase))
                return false;

            return base.Remove(key);
        }
    }
}

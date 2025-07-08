namespace LogParser.Utility
{
    internal class ObservableDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TKey : notnull
    {
        public event Action<TKey, TValue>? ItemAdded;
        public event Action<TKey, TValue>? ItemRemoved;

        // Default constructor
        public ObservableDictionary() : base() { }

        // Constructor accepting Dictionary
        public ObservableDictionary(Dictionary<TKey, TValue> dictionary) : base(dictionary) { }

        // Constructor accepting IDictionary (more flexible)
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }

        // Constructor with capacity
        public ObservableDictionary(int capacity) : base(capacity) { }

        // Constructor with comparer
        public ObservableDictionary(IEqualityComparer<TKey> comparer) : base(comparer) { }

        // Constructor with dictionary and comparer
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
            : base(dictionary, comparer) { }

        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
            ItemAdded?.Invoke(key, value);
        }

        public new TValue this[TKey key]
        {
            get => base[key];
            set
            {
                bool isNew = !ContainsKey(key);
                base[key] = value;
                if (isNew)
                    ItemAdded?.Invoke(key, value);
            }
        }

        public new bool Remove(TKey key)
        {
            if (TryGetValue(key, out TValue? value))
            {
                bool removed = base.Remove(key);
                if (removed)
                    ItemRemoved?.Invoke(key, value);
                return removed;
            }
            return false;
        }
    }
}

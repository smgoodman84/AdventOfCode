using System.Collections.Generic;

namespace AdventOfCode.Shared.DataStructures;

public class Dictionary2D<TKey1, TKey2, TValue>
{
    private Dictionary<TKey1, Dictionary<TKey2, TValue>> _dictionary = new Dictionary<TKey1, Dictionary<TKey2, TValue>>();

    public IEnumerable<TKey1> Keys => _dictionary.Keys;

    public void Add(TKey1 key1, TKey2 key2, TValue value)
    {
        EnsureKey1Exits(key1);
        _dictionary[key1].Add(key2, value);
    }

    public void Set(TKey1 key1, TKey2 key2, TValue value)
    {
        EnsureKey1Exits(key1);
        _dictionary[key1][key2] = value;
    }

    public bool TryGetValue(TKey1 key1, TKey2 key2, out TValue value)
    {
        value = default;
        return _dictionary.TryGetValue(key1, out var dictionary2)
            && dictionary2.TryGetValue(key2, out value);
    }

    public bool ContainsKey(TKey1 key1, TKey2 key2)
    {
        return _dictionary.ContainsKey(key1)
               && _dictionary[key1].ContainsKey(key2);
    }

    public Dictionary<TKey2, TValue> this[TKey1 key]
    {
        get { EnsureKey1Exits(key); return _dictionary[key]; }
        set { _dictionary[key] = value; }
    }

    private void EnsureKey1Exits(TKey1 key1)
    {
        if (!_dictionary.ContainsKey(key1))
        {
            _dictionary.Add(key1, new Dictionary<TKey2, TValue>());
        }
    }
}
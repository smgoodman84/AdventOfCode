using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Shared.DataStructures;

public class SimplePriorityQueue
{
    private List<string>[] _items;
    private Dictionary<string, int> _itemValues = new Dictionary<string, int>();

    public SimplePriorityQueue(int maxPriority)
    {
        _items = Enumerable.Range(0, maxPriority + 1)
            .Select(x => new List<string>())
            .ToArray();
    }

    public void SetPriority(string key, int value)
    {
        if (_itemValues.ContainsKey(key))
        {
            Remove(key);
        }

        if (value >= _items.Length)
        {
            // var stop = true;
        }

        if (value < 0)
        {
            // var stop = true;
        }

        _itemValues.Add(key, value);
        _items[value].Add(key);
    }

    public void Remove(string key)
    {
        var value = _itemValues[key];
        _itemValues.Remove(key);
        _items[value].Remove(key);
    }

    public string Pop()
    {
        foreach (var itemList in _items)
        {
            if (itemList.Any())
            {
                var result = itemList.First();
                Remove(result);
                return result;
            }
        }

        return null;
    }

    public string PopReverse()
    {
        throw new System.Exception("Not sure this is useful or meaningful");
        /*
        for (var i = _items.Length - 1; i >= 0; i -= 1)
        {
            var itemList = _items[i];
            if (itemList.Any())
            {
                var result = itemList.Last();
                Remove(result);
                return result;
            }
        }

        return null;*/
    }
}
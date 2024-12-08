using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Shared.PrimitiveExtensions;

public static class IEnumerableExtensions
{
    public static List<List<T>> GroupsOfSize<T>(
        this IEnumerable<T> items,
        int groupSize
    )
    {
        var groups = new List<List<T>>();
        var currentGroup = new List<T>();
        foreach (var item in items)
        {
            currentGroup.Add(item);
            if (currentGroup.Count == groupSize)
            {
                groups.Add(currentGroup);
                currentGroup = new List<T>();
            }
        }

        if (currentGroup.Any())
        {
            groups.Add(currentGroup);
        }

        return groups;
    }

    public static List<(T, T)> AllPairs<T>(
        this IEnumerable<T> items
    )
    {
        var array = items.ToArray();
        var pairs = new List<(T, T)>();

        for (var a = 0; a < array.Length; a += 1)
        {
            for (var b = a + 1; b < array.Length; b += 1)
            {
                pairs.Add((array[a], array[b]));
            }
        }

        return pairs;
    }
}
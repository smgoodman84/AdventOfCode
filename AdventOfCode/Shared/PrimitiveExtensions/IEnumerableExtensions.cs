using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Shared.PrimitiveExtensions
{
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
    }
}

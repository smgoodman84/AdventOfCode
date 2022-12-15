using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Shared.Numbers
{
    public static class NumberRangeMerger
    {
        public static IEnumerable<NumberRange> Merge(IEnumerable<NumberRange> ranges)
        {
            return Merge(ranges.ToList());
        }

        private static List<NumberRange> Merge(List<NumberRange> ranges)
        {
            var initialCount = ranges.Count;
            var reduced = PartialMerge(ranges);
            var reducedCount = reduced.Count;

            if (reducedCount == initialCount)
            {
                return ranges;
            }

            return Merge(reduced);
        }

        private static List<NumberRange> PartialMerge(List<NumberRange> ranges)
        {
            var merged = new List<NumberRange>();

            foreach (var range in ranges.OrderBy(r => r.Start))
            {
                if (!merged.Any())
                {
                    merged.Add(range);
                }
                else
                {
                    var mergedIn = false;
                    for (var i = 0; i < merged.Count && !mergedIn; i++)
                    {
                        var mergedRange = merged[i];
                        if (mergedRange.Contains(range))
                        {
                            mergedIn = true;
                        }
                        else if (mergedRange.CanAdd(range))
                        {
                            merged[i] = mergedRange.Add(range);
                            mergedIn = true;
                        }
                    }

                    if (!mergedIn)
                    {
                        merged.Add(range);
                    }
                }
            }

            return merged;
        }
    }
}


using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Shared.Algorithms
{
    public class Permutations
	{
		public static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> elements)
		{
			foreach (var element in elements)
			{
				var otherElements = elements.Where(e => e.Equals(element));
				foreach (var permutationsOfOthers in GetPermutations(otherElements))
				{
                    yield return new[] { element }.Concat(permutationsOfOthers);
                }
			}
		}
    }
}


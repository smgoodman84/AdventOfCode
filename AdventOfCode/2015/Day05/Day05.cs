using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2015.Day05
{
    public class Day05 : Day
    {
        public Day05() : base(2015, 5, @"Day05/input.txt", "238", "")
        {
        }

        public override string Part1()
        {
            var niceStrings = InputLines.Where(IsNice).ToList();

            return niceStrings.Count.ToString();
        }

        public override string Part2() => "";

        private bool IsNice(string input)
        {
            char? previous = null;
            var vowelCount = 0;
            var containsDoubleLetter = false;
            var containsInvalidPair = false;

            var vowels = "aeiou".ToCharArray();
            var invalidPairs = new string[] { "ab", "cd", "pq", "xy" };


            foreach(var c in input)
            {
                if (vowels.Contains(c))
                {
                    vowelCount += 1;
                }

                if (previous != null)
                {
                    if (c == previous)
                    {
                        containsDoubleLetter = true;
                    }

                    var pair = $"{previous}{c}";
                    if (invalidPairs.Contains(pair))
                    {
                        containsInvalidPair = true;
                    }
                }

                previous = c;
            }

            return vowelCount >= 3
                && containsDoubleLetter
                && !containsInvalidPair;
        }
    }
}
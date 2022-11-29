using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2015.Day05
{
    public class Day05 : Day
    {
        public Day05() : base(2015, 5, @"Day05/input_2015_05.txt", "238", "")
        {
        }

        public override string Part1()
        {
            var niceStrings = InputLines.Where(IsNice).ToList();

            return niceStrings.Count.ToString();
        }

        public override string Part2()
        {
            var niceStrings = InputLines.Where(IsNice2).ToList();

            return niceStrings.Count.ToString();
        }

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

        private bool IsNice2(string input)
        {
            char? previous = null;
            char? previousPrevious = null;
            var containsSandwich = false;
            var letterPairs = new List<LetterPair>();

            var index = 0;
            foreach (var c in input)
            {
                if (previousPrevious != null)
                {
                    if (previousPrevious == c && previous != c)
                    {
                        containsSandwich = true;
                    }
                }

                if (previous != null)
                {
                    letterPairs.Add(new LetterPair
                    {
                        Index = index,
                        Letters = $"{previous}{c}"
                    });
                }

                previousPrevious = previous;
                previous = c;
                index += 1;
            }

            var containsDuplicatePair = false;
            var pairGroups = letterPairs
                .GroupBy(lr => lr.Letters)
                .Where(g => g.Count() >= 2);

            foreach (var group in pairGroups)
            {
                if (pairGroups.Count() > 2)
                {
                    containsDuplicatePair = true;
                }
                else
                {
                    if (group.Skip(1).First().Index - group.First().Index >= 2)
                    {
                        containsDuplicatePair = true;
                    }
                }
            }

            var result = containsDuplicatePair && containsSandwich;

            System.Console.WriteLine($"{input} {containsDuplicatePair} {containsSandwich} {result}");

            return result;
        }

        private class LetterPair
        {
            public string Letters { get; set; }
            public int Index { get; set; }
        }
    }
}
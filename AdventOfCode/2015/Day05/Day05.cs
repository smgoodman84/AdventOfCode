using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2015.Day05
{
    public class Day05 : Day
    {
        public Day05() : base(2015, 5, "Day05/input_2015_05.txt", "238", "69")
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
            TraceLine();
            var containsLetterSandwich = ContainsLetterSandwich(input);
            var containsNonOverlappingLetterPair = ContainsNonOverlappingLetterPair(input);

            TraceLine($"{input}: Sandwich {containsLetterSandwich}, Pair: {containsNonOverlappingLetterPair}");

            return containsNonOverlappingLetterPair
                && containsLetterSandwich;
        }

        private bool ContainsNonOverlappingLetterPair(string input)
        {
            var letterPairs = new List<LetterPair>();
            for (var index = 0; index < input.Length - 1; index++)
            {
                letterPairs.Add(new LetterPair
                {
                    Letters = input.Substring(index, 2),
                    StartIndex = index,
                    EndIndex = index + 1
                });
            }

            var groupedLetterPairs = letterPairs
                .GroupBy(lp => lp.Letters)
                .Where(g => g.Count() > 1);

            foreach (var groupedLetterPair in groupedLetterPairs)
            {
                var cleanedList = new List<LetterPair>();

                foreach (var letterPair in groupedLetterPair)
                {
                    if (!cleanedList.Any(x => Overlaps(x, letterPair)))
                    {
                        if (cleanedList.Any())
                        {
                            var otherLetterPair = cleanedList.Single();
                            var highlighted1 = $"{new string('_', otherLetterPair.StartIndex)}{otherLetterPair.Letters}{new string('_', input.Length - otherLetterPair.EndIndex - 1)}";
                            var highlighted2 = $"{new string('_', letterPair.StartIndex)}{letterPair.Letters}{new string('_', input.Length - letterPair.EndIndex - 1)}";

                            TraceLine($"{highlighted1} LetterPair {letterPair.Letters}");
                            TraceLine($"{highlighted2} LetterPair {letterPair.Letters}");
                            return true;
                        }

                        cleanedList.Add(letterPair);
                    }
                }
            }

            return false;
        }

        private static bool Overlaps(LetterPair letterPairOne, LetterPair letterPairTwo)
        {
            return letterPairOne.StartIndex == letterPairTwo.StartIndex
                || letterPairOne.StartIndex == letterPairTwo.EndIndex
                || letterPairOne.EndIndex == letterPairTwo.StartIndex
                || letterPairOne.EndIndex == letterPairTwo.EndIndex;
        }

        private bool ContainsLetterSandwich(string input)
        {
            for (var index = 1; index < input.Length - 1; index++)
            {
                if (input[index -1 ] == input[index + 1])
                {
                    var highlighted = $"{new string('_', index)}{input[index]}{new string('_', input.Length - index - 1)}";
                    TraceLine($"{highlighted} Sandwich");
                    return true;
                }
            }

            return false;
        }

        private class LetterPair
        {
            public string Letters { get; set; }
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }
        }
    }
}
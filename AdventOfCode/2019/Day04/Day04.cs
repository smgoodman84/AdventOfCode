using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2019.Day04
{
    class Day04 : Day
    {
        public Day04() : base(2019, 4, "Day04/input_2019_04.txt", "1764", "1196")
        {

        }

        private string _passwordRange;
        public override void Initialise()
        {
            _passwordRange = InputLines.Single();
        }

        public override string Part1()
        {
            return GetPossiblePasswords().ToString();
        }

        public override string Part2()
        {
            return GetPossiblePasswordsWithDigitPair().ToString();
        }

        public int GetPossiblePasswords()
        {
            return GetPossiblePasswords(_passwordRange, AdjacentDigitsMatch);
        }

        public int GetPossiblePasswordsWithDigitPair()
        {
            return GetPossiblePasswords(_passwordRange, ContainsDigitPair);
        }

        public static int GetPossiblePasswords(string passwordRange, Func<int[], bool> adjacentDigitsFunc)
        {
            var range = passwordRange
                .Split('-')
                .Select(x => int.Parse(x.Trim()))
                .ToArray();

            var min = range[0];
            var max = range[1];

            var possiblePasswords = Enumerable
                .Range(min, max - min + 1)
                .Select(GetChars)
                .Where(adjacentDigitsFunc)
                .Where(DigitsIncrease)
                .Count();

            return possiblePasswords;
        }

        private static bool AdjacentDigitsMatch(int[] input)
        {
            var lastC = -1;
            foreach(var c in input)
            {
                if (c == lastC)
                {
                    return true;
                }

                lastC = c;
            }

            return false;
        }

        private static bool ContainsDigitPair(int[] input)
        {
            var runs = new Dictionary<int, int>();

            foreach(var c in input)
            {
                if (!runs.ContainsKey(c))
                {
                    runs.Add(c, 1);
                }
                else
                {
                    runs[c] +=  1;
                }
            }

            return runs.Any(x => x.Value == 2);
        }

        private static bool DigitsIncrease(int[] input)
        {
            var lastC = -1;
            foreach (var c in input)
            {
                if (c < lastC)
                {
                    return false;
                }

                lastC = c;
            }

            return true;
        }

        private static int[] GetChars(int input)
        {
            return input.ToString()
                .ToCharArray()
                .Select(c => int.Parse(c.ToString()))
                .ToArray();
        }
    }
}

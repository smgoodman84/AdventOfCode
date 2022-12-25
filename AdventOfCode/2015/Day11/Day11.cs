using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Algorithms;

namespace AdventOfCode._2015.Day11
{
    public class Day11 : Day
    {
        public Day11() : base(2015, 11, "Day11/input_2015_11.txt", "vzbxxyzz", "vzcaabcc")
        {
        }

        public override string Part1()
        {
            var currentPassword = InputLines.Single();

            return IncrementPassword(currentPassword);
        }

        public override string Part2()
        {
            var currentPassword = InputLines.Single();

            return IncrementPassword(currentPassword, 2);
        }

        private string IncrementPassword(string password, int count = 1)
        {
            while (count > 0)
            {
                password = GetNextPassword(password);
                count -= 1;
            }

            return password;
        }

        private string GetNextPassword(string password)
        {
            var current = Increment(password);

            while (!IsValidPassword(current))
            {
                current = Increment(current);
            }

            return current;
        }

        public bool IsValidPassword(string input)
        {
            if (ContainsInvalidLetters(input))
            {
                return false;
            }

            var runLengths = RunLength<char>.GetRunLengths(input);
            var runsLongerThanTwo = runLengths.Where(rl => rl.Length > 1);
            if (runsLongerThanTwo.GroupBy(x => x.Element).Count() < 2)
            {
                return false;
            }

            return IncrementingRuns.Any(x => input.Contains(x));
        }

        private static List<string> IncrementingRuns = Enumerable.Range(0, 24)
                .Select(x => $"{(char)('a' + x)}{(char)('b' + x)}{(char)('c' + x)}")
                .ToList();

        private static char[] InvalidLetters = new[] { 'i', 'o', 'l' };
        public bool ContainsInvalidLetters(string input)
        {
            if (input.Any(c => InvalidLetters.Contains(c)))
            {
                return true;
            }

            return false;
        }

        public string Increment(string input)
        {
            var array = input.ToCharArray();

            for (var i = array.Length - 1; i >= 0; i--)
            {
                var newChar = Increment(array[i]);
                array[i] = newChar;
                if (newChar != 'a')
                {
                    return string.Join("", array);
                }
            }

            return "a" + string.Join("", array);
        }

        private char Increment(char c)
        {
            if (c == 'z')
            {
                return 'a';
            }

            return (char)(c + 1);
        }
    }
}
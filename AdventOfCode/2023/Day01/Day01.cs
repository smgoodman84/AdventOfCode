using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;

namespace AdventOfCode._2023.Day01
{
    public class Day01 : Day
    {
        public Day01() : base(2023, 1, "Day01/input_2023_01.txt", "55538", "54875")
        {

        }

        public override void Initialise()
        {
        }

        public override string Part1()
        {
            var value = InputLines
                .Select(l => GetNumber(l))
                .Sum();

            return value.ToString();
        }

        private static int GetNumber(string line)
        {
            var firstDigit = line.First(c => IsNumber(c));
            var lastDigit = line.Last(c => IsNumber(c));
            var number = int.Parse($"{firstDigit}{lastDigit}");
            return number;
        }

        private static bool IsNumber(char c)
        {
            return c >= '0' && c <= '9';
        }

        public override string Part2()
        {
            var value = InputLines
                .Select(l => GetNumberString(l))
                .Sum();

            return value.ToString();
        }

        private static int GetNumberString(string line)
        {
            var digitNames = FindDigitNames(line).OrderBy(d => d.Position);
            var first = digitNames.First();
            var last = digitNames.Last();
            var number = int.Parse($"{first.Value}{last.Value}");
            return number;
        }

        private static List<(int Value, int Position)> FindDigitNames(string line)
        {
            var result = new List<(int Value, int Position)>();
            for (var index = 0; index < line.Length; index += 1)
            {
                if (IsNumber(line[index]))
                {
                    result.Add((int.Parse($"{line[index]}"), index));
                }
                else
                {
                    var digitValue = 0;
                    foreach (var digitName in DigitNames)
                    {
                        if (line.Substring(index).StartsWith(digitName))
                        {
                            result.Add((digitValue, index));
                        }
                        digitValue += 1;
                    }
                }
            }
            return result;
        }

        private static string[] DigitNames = new[] {
            "zero",
            "one",
            "two",
            "three",
            "four",
            "five",
            "six",
            "seven",
            "eight",
            "nine",
        };
    }
}


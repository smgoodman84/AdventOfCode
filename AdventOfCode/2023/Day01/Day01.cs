using AdventOfCode.Shared;

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
            return GetCalibrationSum(Digits).ToString();
        }

        public override string Part2()
        {
            var lookup = Digits.Concat(DigitNames)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return GetCalibrationSum(lookup).ToString();
        }

        private int GetCalibrationSum(Dictionary<string, int> digitLookup)
        {
            var value = InputLines
                .Select(l => GetCalibrationValue(l, digitLookup))
                .Sum();

            return value;
        }

        private static int GetCalibrationValue(string line, Dictionary<string, int> digitLookup)
        {
            var digitNames = FindDigits(line, digitLookup);
            var first = digitNames.First().Value;
            var last = digitNames.Last().Value;
            return first * 10 + last;
        }

        private static List<(int Position, int Value)> FindDigits(string line, Dictionary<string, int> digitLookup)
        {
            var span = line.AsSpan();
            var result = new List<(int Value, int Position)>();
            for (var position = 0; position < line.Length; position += 1)
            {
                foreach (var digit in digitLookup)
                {
                    if (span.Slice(position).StartsWith(digit.Key))
                    {
                        result.Add((position, digit.Value));
                    }
                }
            }
            return result;
        }

        private static Dictionary<string, int> Digits = new Dictionary<string, int> {
            { "0", 0 },
            { "1", 1 },
            { "2", 2 },
            { "3", 3 },
            { "4", 4 },
            { "5", 5 },
            { "6", 6 },
            { "7", 7 },
            { "8", 8 },
            { "9", 9 },
        };

        private static Dictionary<string, int> DigitNames = new Dictionary<string, int> {
            { "zero", 0 },
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 },
        };
    }
}


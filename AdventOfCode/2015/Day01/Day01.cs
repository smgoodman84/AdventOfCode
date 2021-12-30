using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2015.Day01
{
    public class Day01 : Day
    {
        public Day01() : base(2015, 1, @"Day01/input.txt", "232", "")
        {
        }

        public override void Initialise()
        {
        }

        public override string Part1()
        {
            var line = InputLines.First();

            var openingBrackets = line.Count(c => c == '(');
            var closingBrackets = line.Count(c => c == ')');

            var result = openingBrackets - closingBrackets;

            return result.ToString();
        }

        public override string Part2()
        {
            return "";
        }
    }
}
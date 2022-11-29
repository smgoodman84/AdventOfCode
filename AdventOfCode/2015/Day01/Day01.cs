using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2015.Day01
{
    public class Day01 : Day
    {
        public Day01() : base(2015, 1, "Day01/input_2015_01.txt", "232", "1783")
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
            var line = InputLines.First();

            var position = 0;
            var floor = 0;

            foreach (var c in line)
            {
                if (c == '(')
                {
                    floor += 1;
                }

                if (c == ')')
                {
                    floor -= 1;
                }

                position += 1;

                if (floor == -1)
                {
                    return position.ToString();
                }
            }

            return "";
        }
    }
}
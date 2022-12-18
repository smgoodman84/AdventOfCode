using System.Linq;
using System.Text;
using AdventOfCode.Shared;

namespace AdventOfCode._2015.Day10
{
    public class Day10 : Day
    {
        public Day10() : base(2015, 10, "Day10/input_2015_10.txt", "492982", "")
        {
        }

        public override string Part1()
        {
            var input = InputLines.Single();
            var output = Iterate(input, 40);

            return output.Length.ToString();
        }

        public override string Part2()
        {
            var input = InputLines.Single();
            var output = Iterate(input, 50);

            return output.Length.ToString();
        }

        private string Iterate(string value, int count)
        {
            for (var i = 0; i < count; i++)
            {
                value = Iterate(value);
                // Console.WriteLine(value);
            }

            return value;
        }

        private string Iterate(string value)
        {
            var index = 0;
            var currentChar = ' ';
            int runLength = 0;
            var output = new StringBuilder();

            foreach(var c in value)
            {
                if (c == currentChar)
                {
                    runLength += 1;
                }
                else
                {
                    if (currentChar != ' ')
                    {
                        output.Append($"{runLength}{currentChar}");
                    }

                    currentChar = c;
                    runLength = 1;
                }
            }

            output.Append($"{runLength}{currentChar}");
            return output.ToString();
        }
    }
}
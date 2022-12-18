using System.Linq;
using AdventOfCode._2019.Intcode;
using AdventOfCode.Shared;

namespace AdventOfCode._2019.Day02
{
    public class Day09 : Day
    {
        public Day09() : base(2019, 9, "Day09/input_2019_09.txt", "2351176124", "73110")
        {

        }

        public override string Part1()
        {
            var output = new OutputCapture();

            IntcodeMachine.Load(InputLines)
                .SetInput(new PreparedInput(1))
                .SetOutput(output)
                .Execute()
                .GetAwaiter()
                .GetResult();

            return output.OutputValues.Single().ToString();
        }

        public override string Part2()
        {
            var output = new OutputCapture();

            IntcodeMachine.Load(InputLines)
                .SetInput(new PreparedInput(2))
                .SetOutput(output)
                .Execute()
                .GetAwaiter()
                .GetResult();

            return output.OutputValues.Single().ToString();
        }
    }
}

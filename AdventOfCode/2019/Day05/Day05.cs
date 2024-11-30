using System.Linq;
using AdventOfCode._2019.Intcode;
using AdventOfCode.Shared;

namespace AdventOfCode._2019.Day05;

public class Day05 : Day
{
    public Day05() : base(2019, 5, "Day05/input_2019_05.txt", "8332629", "8805067")
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

        return output.OutputValues
            .Where(o => o != 0)
            .Single()
            .ToString();
    }

    public override string Part2()
    {
        var output = new OutputCapture();

        IntcodeMachine.Load(InputLines)
            .SetInput(new PreparedInput(5))
            .SetOutput(output)
            .Execute()
            .GetAwaiter()
            .GetResult();

        return output.OutputValues
            .Single()
            .ToString();
    }
}
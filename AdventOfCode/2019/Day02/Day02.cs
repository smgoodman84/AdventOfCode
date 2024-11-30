using AdventOfCode._2019.Intcode;
using AdventOfCode.Shared;

namespace AdventOfCode._2019.Day02;

public class Day02 : Day
{
    public Day02() : base(2019, 2, "Day02/input_2019_02.txt", "4570637", "5485")
    {

    }

    public override string Part1()
    {
        return IntcodeMachine.Load(InputLines)
            .Repair(1, 12)
            .Repair(2, 2)
            .Execute()
            .GetAwaiter()
            .GetResult()
            .ReadMemory(0)
            .ToString();
    }

    public override string Part2()
    {
        return IntcodeMachine.Load(InputLines)
            .FindNounAndVerb(19690720)
            .ToString();
    }
}
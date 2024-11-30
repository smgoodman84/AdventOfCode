using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2021.Day01;

public class Day01 : Day
{
    private List<int> _depths;

    public Day01() : base(2021, 1, "Day01/input_2021_01.txt", "1215", "1150")
    {
    }

    public override void Initialise()
    {
        _depths = InputLines
            .Select(int.Parse)
            .ToList();
    }

    public override string Part1()
    {
        int increaseCount = 0;
        int? previousValue = null;
        foreach (var depth in _depths)
        {
            if (previousValue.HasValue && previousValue.Value < depth)
            {
                increaseCount += 1;
            }
            previousValue = depth;
        }

        return increaseCount.ToString();
    }

    public override string Part2()
    {
        var increaseCount = 0;
        var depthArray = _depths.ToArray();

        for (var i = 0; i < depthArray.Length - 3; i++)
        {
            if (depthArray[i] < depthArray[i+3])
            {
                increaseCount += 1;
            }
        }

        return increaseCount.ToString();
    }
}
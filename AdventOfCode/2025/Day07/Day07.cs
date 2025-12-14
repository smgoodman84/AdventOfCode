using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.Numbers;

namespace AdventOfCode._2025.Day07;

public class Day07 : Day
{
    public Day07() : base(2025, 7, "Day07/input_2025_07.txt", "1678", "", true)
    {

    }

    private Grid2D<char> _map;
    private Coordinate2D _start;
    public override void Initialise()
    {
        _map = Grid2D<char>.CreateWithCartesianCoordinates(
            InputLines,
            (c, x) => x);

        _start = _map.FindCoordinates(c => c == 'S').Single();
    }

    public override string Part1()
    {
        return GetSplitCount(_start.Y, [_start.X]).ToString();
    }

    private int GetSplitCount(long y, HashSet<long> xs)
    {
        if (y < 0)
        {
            return 0;
        }

        var newXs = new HashSet<long>();
        var splitCount = 0;
        foreach (var x in xs)
        {
            var val = _map.Read(x, y);
            if (val == '^')
            {
                splitCount += 1;
                newXs.Add(x - 1);
                newXs.Add(x + 1);
            }
            else
            {
                newXs.Add(x);
            }
        }

        return splitCount + GetSplitCount(y - 1, newXs);
    }

    public override string Part2()
    {
        return string.Empty;
    }
}
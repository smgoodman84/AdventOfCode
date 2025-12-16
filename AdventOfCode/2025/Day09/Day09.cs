using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.Numbers;

namespace AdventOfCode._2025.Day09;

public class Day09 : Day
{
    public Day09() : base(2025, 9, "Day09/input_2025_09.txt", "4782151432", "", false)
    {

    }

    private List<Coordinate2D> _redTiles;
    public override void Initialise()
    {
        _redTiles = InputLines
            .Select(l => new Coordinate2D(l))
            .ToList();
    }

    public override string Part1()
    {
        long maxArea = 0;
        foreach (var start in _redTiles)
        {
            foreach (var end in _redTiles)
            {
                maxArea = Math.Max(maxArea, Area(start, end));
            }
        }

        return maxArea.ToString();
    }

    private long Area(Coordinate2D start, Coordinate2D end)
    {
        var width = Math.Abs(start.X - end.X) + 1;
        var height = Math.Abs(start.Y - end.Y) + 1;

        return width * height;
    }

    public override string Part2()
    {
        return string.Empty;
    }
}
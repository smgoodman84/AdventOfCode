using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.Numbers;

namespace AdventOfCode._2025.Day09;

public class Day09 : Day
{
    public Day09() : base(2025, 9, "Day09/input_2025_09.txt", "4782151432", "", true)
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
        var polygon = new Polygon(_redTiles);
        var areas = new List<(Coordinate2D Start, Coordinate2D End, long Area)>();
        long maxArea = 0;
        for (var i=0; i < _redTiles.Count; i++)
        {
            for (var j=0; j < i; j++)
            {
                var start = _redTiles[i];
                var end = _redTiles[j];
                areas.Add((start, end, Area(start, end)));
                if (InsidePolygon(polygon, start, end))
                {
                    maxArea = Math.Max(maxArea, Area(start, end));
                }
            }
        }

        var maxInsideArea = areas
            .OrderByDescending(x => x.Area)
            .First(x => InsidePolygon(polygon, x.Start, x.End));

        return maxInsideArea.Area.ToString();
    }

    private bool InsidePolygon(Polygon polygon, Coordinate2D start, Coordinate2D end)
    {
        var minX = Math.Min(start.X, end.X);
        var minY = Math.Min(start.Y, end.Y);
        var maxX = Math.Max(start.X, end.X);
        var maxY = Math.Max(start.Y, end.Y);
        
        TraceLine($"Checking {minX},{minY} to {maxX},{maxY}");

        for (var x = minX; x <= maxX; x++)
        {
            
            TraceLine($"    Checking x = {x}");
            if (!InsidePolygon(polygon, x, minY) || !InsidePolygon(polygon, x, maxY))
            {
                return false;
            }
        }
        
        for (var y = minY; y <= maxY; y++)
        {
            TraceLine($"    Checking y = {y}");
            if (!InsidePolygon(polygon, minX, y) || !InsidePolygon(polygon, maxX, y))
            {
                return false;
            }
        }

        return true;
    }

    private bool InsidePolygon(Polygon polygon, long x, long y)
    {
        return polygon.ContainsUsingContext(new Coordinate2D(x, y));
    }
}
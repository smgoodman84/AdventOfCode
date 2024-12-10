using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2024.Day10;

public class Day10 : Day
{
    public Day10() : base(2024, 10, "Day10/input_2024_10.txt", "789", "", true)
    {
    }


    private Grid2D<int> _heights;
    private List<Coordinate2D> _trailheads = new List<Coordinate2D>();
    public override void Initialise()
    {
        var mapHeight = InputLines.Count;
        var mapWidth = InputLines[0].Length;
        _heights = new Grid2D<int>(mapWidth, mapHeight);

        var y = 0;
        foreach (var line in InputLines)
        {
            var x = 0;
            foreach (var c in line)
            {
                var height = c - '0';
                _heights.Write(x, y, height);

                if (height == 0)
                {
                    _trailheads.Add(new Coordinate2D(x, y));
                }
                x += 1;
            }

            y += 1;
        }
    }

    private HashSet<Coordinate2D> GetReachableSummits(
        Coordinate2D position,
        List<Coordinate2D> visited,
        Dictionary<Coordinate2D, HashSet<Coordinate2D>> cache)
    {
        if (cache.TryGetValue(position, out var cachedSummits))
        {
            TraceLine($"{position}: Using cache value");
            return cachedSummits;
        }
            
        // TraceLine($"{position}: Calculating {position}");

        var newVisited = visited.ToList();
        newVisited.Add(position);

        var currentHeight = _heights.Read(position);

        var result = new HashSet<Coordinate2D>();
        if (currentHeight == 9)
        {
            TraceLine($"{position}: Height is 9, adding summit.");
            var path = string.Join(" ", newVisited);
            TraceLine($"{position}: Path - {path}");
            result.Add(position);
        }

        foreach(var neighbour in position.Neighbours())
        {
            if (!_heights.IsInGrid(neighbour))
            {
                continue;
            }

            if (visited.Contains(neighbour))
            {
                continue;
            }

            var neighbourHeight = _heights.Read(neighbour);
            var heightDifference = neighbourHeight - currentHeight;
            if (heightDifference != 1)
            {
                continue;
            }

            var neighbourSummits = GetReachableSummits(neighbour, newVisited, cache);
            foreach (var neighbourSummit in neighbourSummits)
            {
                result.Add(neighbourSummit);
            }
        }

        // cache.Add(position, result);

        return result;
    }

    public override string Part1()
    {
        var result = 0;
        var scoreCache = new Dictionary<Coordinate2D, HashSet<Coordinate2D>>();

        foreach (var trailHead in _trailheads)
        {
            TraceLine($"Checking Trailhead {trailHead}");

            var reachableSummits = GetReachableSummits(
                trailHead,
                new List<Coordinate2D>(),
                scoreCache);

            result += reachableSummits.Count;

            TraceLine($"Trailhead {trailHead} had {reachableSummits.Count} reachable summits");
        }

        return result.ToString();
    }

    public override string Part2()
    {
        return string.Empty;
    }
}
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.Graphs;

namespace AdventOfCode._2024.Day18;

public class Day18 : Day
{
    public Day18() : base(2024, 18, "Day18/input_2024_18.txt", "270", "51,40", false)
    {
    }

    private List<Coordinate2D> _corruptionLocations;
    public override void Initialise()
    {
        _corruptionLocations = InputLines
            .Select(ParseLine)
            .ToList();
    }

    private Coordinate2D ParseLine(string line)
    {
        var split = line.Split(',').Select(int.Parse).ToArray();
        return new Coordinate2D(split[0], split[1], CoordinateSystem.Screen);
    }

    public override string Part1()
    {
        return GetShortestPath(71, _corruptionLocations.Take(1024).ToList()).ToString();
    }

    public override string Part2()
    {
        var min = 0;
        var max = _corruptionLocations.Count;

        while (min < max - 1)
        {
            var middle = (min + max) / 2;
            var locationsToCorrupt = _corruptionLocations.Take(middle).ToList();
            var lastLocation = locationsToCorrupt.Last();
            var shortestPath = GetShortestPath(71, locationsToCorrupt);
            
            if (shortestPath == -1)
            {
                TraceLine($"[{min},{middle},{max}] up to and including {lastLocation} - blocked");
                max = middle;
            }
            else
            {
                TraceLine($"[{min},{middle},{max}] up to and including {lastLocation} - passable");
                min = middle;
            }
        }

        var blockingLocation = _corruptionLocations.Take(max).Last();

        return blockingLocation.ToString();
    }

    public int GetShortestPath(int size, List<Coordinate2D> corruptedLocations)
    {
        var memory = new Grid2D<GraphNode<Memory>>(size, size);
        foreach(var coordinate in memory.AllCoordinates())
        {
            memory.Write(coordinate, new GraphNode<Memory>(new Memory
            {
                Location = coordinate,
                IsCorrupted = false
            }));
        }

        foreach(var coordinate in corruptedLocations)
        {
            var memoryLocation = memory.Read(coordinate);
            memoryLocation.Data.IsCorrupted = true;
        }

        var graph = new Graph<Memory>();
        
        foreach(var coordinate in memory.AllCoordinates())
        {
            var node = memory.Read(coordinate);
            graph.AddNode(node);
        }

        foreach(var coordinate in memory.AllCoordinates())
        {
            var nodeData = memory.Read(coordinate);
            if (nodeData.Data.IsCorrupted)
            {
                continue;
            }

            var neighbourCoordinates = nodeData.Data.Location.Neighbours()
                .Where(memory.IsInGrid)
                .ToList();

            foreach (var neighbourCoordinate in neighbourCoordinates)
            {
                var neighbour = memory.Read(neighbourCoordinate);
                if (!neighbour.Data.IsCorrupted)
                {
                    graph.AddEdge(new GraphEdge<Memory>
                    {
                        Source = nodeData,
                        Destination = neighbour,
                        Distance = 1
                    });
                }
            }
        }

        var start = memory.Read(0, 0);
        var end = memory.Read(memory.MaxX, memory.MaxY);
        try
        {
            return graph.GetShortestPathDistance(start, end, 10000);
        }
        catch (Exception)
        {
            return -1;
        }
    }

    private class Memory : IGraphNodeData
    {
        public Coordinate2D Location { get; set; }
        public bool IsCorrupted { get; set; }

	    public string GetIdentifier() => Location.ToString();
    }
}
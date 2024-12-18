using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.Graphs;

namespace AdventOfCode._2024.Day18;

public class Day18 : Day
{
    public Day18() : base(2024, 18, "Day18/input_2024_18.txt", "270", "", true)
    {
    }

    private Grid2D<GraphNode<Memory>> _memory;
    private List<Coordinate2D> _corruptionLocations;
    public override void Initialise()
    {
        _memory = new Grid2D<GraphNode<Memory>>(71, 71);
        foreach(var coordinate in _memory.AllCoordinates())
        {
            _memory.Write(coordinate, new GraphNode<Memory>(new Memory
            {
                Location = coordinate,
                IsCorrupted = false
            }));
        }
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
        foreach(var coordinate in _corruptionLocations.Take(1024))
        {
            var memory = _memory.Read(coordinate);
            memory.Data.IsCorrupted = true;
        }

        var graph = new Graph<Memory>();
        
        foreach(var coordinate in _memory.AllCoordinates())
        {
            var node = _memory.Read(coordinate);
            graph.AddNode(node);
        }

        foreach(var coordinate in _memory.AllCoordinates())
        {
            var nodeData = _memory.Read(coordinate);
            if (nodeData.Data.IsCorrupted)
            {
                continue;
            }

            var neighbourCoordinates = nodeData.Data.Location.Neighbours()
                .Where(_memory.IsInGrid)
                .ToList();

            foreach (var neighbourCoordinate in neighbourCoordinates)
            {
                var neighbour = _memory.Read(neighbourCoordinate);
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

        var start = _memory.Read(0, 0);
        var end = _memory.Read(_memory.MaxX, _memory.MaxY);
        var shortestPath = graph.GetShortestPathDistance(start, end, 10000);
        return shortestPath.ToString();
    }

    public override string Part2()
    {
        return string.Empty;
    }

    private class Memory : IGraphNodeData
    {
        public Coordinate2D Location { get; set; }
        public bool IsCorrupted { get; set; }

	    public string GetIdentifier() => Location.ToString();
    }
}
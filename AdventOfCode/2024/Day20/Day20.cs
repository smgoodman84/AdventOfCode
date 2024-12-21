using System.Drawing.Text;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.Graphs;

namespace AdventOfCode._2024.Day20;

public class Day20 : Day
{
    public Day20() : base(2024, 20, "Day20/input_2024_20.txt", "", "", true)
    {
    }

    private Grid2D<GraphNode<Location>> _map;
    private GraphNode<Location> _start;
    private GraphNode<Location> _end;
    private Graph<Location> _graph;
    public override void Initialise()
    {
        _map = Grid2D<GraphNode<Location>>.CreateWithScreenCoordinates(
            InputLines,
            (coord, c) => new GraphNode<Location>(new Location(coord, c))
        );

        _graph = new Graph<Location>();
        foreach(var location in _map.ReadAll())
        {
            if (location.Data.IsStart)
            {
                _start = location;
            }

            if (location.Data.IsEnd)
            {
                _end = location;
            }

            if (location.Data.LocationType == LocationType.Empty)
            {
                _graph.AddNode(location);

                foreach (var neighbourCoordinate in location.Data.Coordinates.Neighbours())
                {
                    var neighbour = _map.Read(neighbourCoordinate);
                    if (neighbour.Data.LocationType == LocationType.Empty)
                    {
                        _graph.AddEdge(new GraphEdge<Location>
                        {
                            Source = location,
                            Destination = neighbour,
                            Distance = 1
                        });
                    }
                }
            }
        }
    }

    public override string Part1()
    {
        var result = 0;
        var shortestPath = _graph.GetShortestPathDistance(_start, _end, 10000);

        for (var x = 1; x <= _map.MaxX - 2; x += 1)
        {
            for (var y = 1; y <= _map.MaxY - 1; y += 1)
            {
                if (TryCheatHorizontal(x, y, out var newShortestPath))
                {
                    var saves = shortestPath - newShortestPath;
                    TraceLine($"Horizontal {x},{y} Saves {saves}");
                    if (saves >= 100)
                    {
                        result += 1;
                    }
                }
            }
        }
        for (var x = 1; x <= _map.MaxX - 1; x += 1)
        {
            for (var y = 1; y <= _map.MaxY - 2; y += 1)
            {
                if (TryCheatVertical(x, y, out var newShortestPath))
                {
                    var saves = shortestPath - newShortestPath;
                    TraceLine($"Vertical {x},{y} Saves {saves}");
                    if (saves >= 100)
                    {
                        result += 1;
                    }
                }
            }
        }

        return result.ToString();
    }

    private bool TryCheatHorizontal(int x, int y, out int newShortestPath)
    {
        var leftNode = _map.Read(x, y);
        var rightNode = _map.Read(x + 1, y);

        return TryCheat(leftNode, rightNode, out newShortestPath);
    }

    private bool TryCheatVertical(int x, int y, out int newShortestPath)
    {
        var topNode = _map.Read(x, y);
        var bottomNode = _map.Read(x, y + 1);

        return TryCheat(topNode, bottomNode, out newShortestPath);
    }

    private bool TryCheat(GraphNode<Location> node1, GraphNode<Location> node2, out int newShortestPath)
    {
        var tempNodes = new List<GraphNode<Location>>();
        var tempEdges = new List<GraphEdge<Location>>();
        
        var makeEmptyNodes = new List<GraphNode<Location>>()
            {
                node1,
                node2
            }
            .Where(n => n.Data.LocationType == LocationType.Wall)
            .ToList();

        if (!makeEmptyNodes.Any())
        {
            newShortestPath = -1;
            return false;
        }

        foreach (var makeEmptyNode in makeEmptyNodes)
        {
            tempNodes.Add(makeEmptyNode);
            _graph.AddNode(makeEmptyNode);

            foreach (var neighbourCoordinate in makeEmptyNode.Data.Coordinates.Neighbours())
            {
                var neighbour = _map.Read(neighbourCoordinate);
                if (neighbour.Data.LocationType == LocationType.Empty)
                {
                    var edge1 = new GraphEdge<Location>
                    {
                        Source = makeEmptyNode,
                        Destination = neighbour,
                        Distance = 1
                    };
                    var edge2 = new GraphEdge<Location>
                    {
                        Source = neighbour,
                        Destination = makeEmptyNode,
                        Distance = 1
                    };
                    tempEdges.Add(edge1);
                    tempEdges.Add(edge2);
                    _graph.AddEdge(edge1);
                    _graph.AddEdge(edge2);
                }
            }
        }

        if (makeEmptyNodes.Count == 2)
        {
            var edge1 = new GraphEdge<Location>
            {
                Source = makeEmptyNodes[0],
                Destination = makeEmptyNodes[1],
                Distance = 1
            };
            var edge2 = new GraphEdge<Location>
            {
                Source = makeEmptyNodes[1],
                Destination = makeEmptyNodes[0],
                Distance = 1
            };
            tempEdges.Add(edge1);
            tempEdges.Add(edge2);
            _graph.AddEdge(edge1);
            _graph.AddEdge(edge2);
        }

        newShortestPath = _graph.GetShortestPathDistance(_start, _end, 10000);

        foreach (var tempEdge in tempEdges)
        {
            _graph.RemoveEdge(tempEdge);
        }

        foreach (var tempNode in tempNodes)
        {
            _graph.RemoveNode(tempNode);
        }

        return true;
    }

    public override string Part2()
    {
        return string.Empty;
    }

    private class Location : IGraphNodeData
    {
        public Coordinate2D Coordinates { get; set; }
        public LocationType LocationType { get; set; }
        public bool IsStart { get; set; }
        public bool IsEnd { get; set; }

        public Location(Coordinate2D coordinates, char locationType)
        {
            Coordinates = coordinates;
            LocationType = GetLocationType(locationType);
            IsStart = locationType == 'S';
            IsEnd = locationType == 'E';
        }

        public string GetIdentifier()
        {
            return Coordinates.ToString();
        }

        private LocationType GetLocationType(char locationType)
        {
            switch (locationType)
            {
                case '#': return LocationType.Wall;
                case 'S': return LocationType.Empty;
                case 'E': return LocationType.Empty;
                case '.': return LocationType.Empty;
            }

            throw new Exception($"Unknown location type {locationType}");
        }
    }

    private enum LocationType
    {
        Wall,
        Empty
    }
}
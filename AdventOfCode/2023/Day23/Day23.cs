using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.Graphs;

namespace AdventOfCode._2023.Day23;

public class Day23 : Day
{
    public Day23() : base(2023, 23, "Day23/input_2023_23.txt", "2354", "6686", false)
    {

    }

    private Grid2D<char> _map;
    private Coordinate2D _start;
    private Coordinate2D _end;

    private Graph<NodeData> _graph;

    public override void Initialise()
    {
        _map = GridReader.LoadGrid(InputLines, (c, _) => c);
        _start = new Coordinate2D(1, _map.MaxY);
        _end = new Coordinate2D(_map.MaxX - 1, 0);
    }

    public override string Part1() => GetLongestPath(NeighboursConsideringOneWay);
    public override string Part2() => GetLongestPath(NeighboursIgnoringOneWay);

    private string GetLongestPath(Func<Coordinate2D, List<Coordinate2D>> getNeighbours)
    {
        LoadGraph(getNeighbours);

        _graph.TryGetNode(_start.ToString(), out var startNode);
        _graph.TryGetNode(_end.ToString(), out var endNode);

        var startContext = new Context
        {
            DistanceTraveled = 0,
            Node = startNode,
            Previous = null
        };

        var longest = FindLongestPath(startNode, endNode, startContext);

        return longest.DistanceTraveled.ToString();
    }

    private Context FindLongestPath(
        GraphNode<NodeData> current,
        GraphNode<NodeData> end,
        Context context)
    {
        if (current.Data.Location.Equals(end.Data.Location))
        {
            TraceLine($"Found path {context.DistanceTraveled}");
            return context;
        }

        var paths = new List<Context>();
        var neighbours = _graph.GetNeighbours(current);
        foreach(var neighbour in neighbours)
        {
            if (Visited(context, neighbour.Destination.Data.Location))
            {
                continue;
            }

            var nextContext = new Context
            {
                Previous = context,
                Node = neighbour.Destination,
                DistanceTraveled = context.DistanceTraveled + neighbour.Distance
            };

            var nextPath = FindLongestPath(neighbour.Destination, end, nextContext);
            if (nextPath != null)
            {
                paths.Add(nextPath);
            }
        }

        var bestPath = paths.OrderByDescending(p => p.DistanceTraveled).FirstOrDefault();

        return bestPath;
    }

    private class Context
    {
        public Context Previous { get; set; }
        public GraphNode<NodeData> Node { get; set; }
        public int DistanceTraveled { get; set; }
    }

    private bool Visited(Context context, Coordinate2D coordinate)
    {
        if (context.Node.Data.Location.Equals(coordinate))
        {
            return true;
        }

        if (context.Previous == null)
        {
            return false;
        }

        return Visited(context.Previous, coordinate);
    }

    private void LoadGraph(Func<Coordinate2D, List<Coordinate2D>> getNeighbours)
    {
        _graph = new Graph<NodeData>();
        TraceLine($"Adding node at {_start}");
        _graph.AddNode(new GraphNode<NodeData>(new NodeData(_start)));
        TraceLine($"Adding node at {_end}");
        _graph.AddNode(new GraphNode<NodeData>(new NodeData(_end)));

        foreach (var x in _map.XIndexes())
        {
            foreach (var y in _map.YIndexes())
            {
                var coordinate = new Coordinate2D(x, y);
                if (_map.Read(coordinate) != '#')
                {
                    var neighbours = NeighboursIgnoringOneWay(coordinate);
                    if (neighbours.Count > 2)
                    {
                        TraceLine($"Adding node at {coordinate}");
                        _graph.AddNode(new GraphNode<NodeData>(new NodeData(coordinate)));
                    }
                }
            }
        }

        foreach(var node in _graph.AllNodes())
        {
            LoadEdges(node, getNeighbours);
        }
    }

    private void LoadEdges(GraphNode<NodeData> node, Func<Coordinate2D, List<Coordinate2D>> getNeighbours)
    {
        var start = node.Data.Location;
        var neighbours = getNeighbours(start);
        foreach(var neighbour in neighbours)
        {
            var current = neighbour;
            var previous = start;
            var distance = 1;

            GraphNode<NodeData> end;
            while (!_graph.TryGetNode(current.ToString(), out end))
            {
                var next = current
                    .Neighbours()
                    .Where(_map.IsInGrid)
                    .Where(n => !n.Equals(previous))
                    .Where(n => _map.Read(n) != '#')
                    .Single();

                previous = current;
                current = next;
                distance += 1;
            }

            TraceLine($"Adding edge from {node} to {end} with distance {distance}");
            _graph.AddEdge(new GraphEdge<NodeData>
            {
                Source = node,
                Destination = end,
                Distance = distance
            });
        }
    }

    private class NodeData : IGraphNodeData
    {
        public NodeData(Coordinate2D location)
        {
            Location = location;
        }

        public Coordinate2D Location { get; }

        public string GetIdentifier()
        {
            return Location.ToString();
        }

        public override string ToString()
        {
            return GetIdentifier();
        }
    }

    private List<Coordinate2D> NeighboursConsideringOneWay(Coordinate2D coordinate)
    {
        var result = new List<Coordinate2D>();
        foreach (var direction in Enum.GetValues<Direction>())
        {
            var neighbourCoordinate = coordinate.Neighbour(direction);
            if (!_map.IsInGrid(neighbourCoordinate))
            {
                continue;
            }

            var neighbourValue = _map.Read(neighbourCoordinate);
            bool canMoveToNeighbour = false;
            switch (neighbourValue)
            {
                case '>': canMoveToNeighbour = direction == Direction.Right; break;
                case '<': canMoveToNeighbour = direction == Direction.Left; break;
                case '^': canMoveToNeighbour = direction == Direction.Up; break;
                case 'v': canMoveToNeighbour = direction == Direction.Down; break;
                case '.': canMoveToNeighbour = true; break;
            }

            var hereValue = _map.Read(coordinate);
            bool canMoveFromHere = false;
            switch (hereValue)
            {
                case '>': canMoveFromHere = direction == Direction.Right; break;
                case '<': canMoveFromHere = direction == Direction.Left; break;
                case '^': canMoveFromHere = direction == Direction.Up; break;
                case 'v': canMoveFromHere = direction == Direction.Down; break;
                case '.': canMoveFromHere = true; break;
            }

            if (canMoveToNeighbour && canMoveFromHere)
            {
                result.Add(neighbourCoordinate);
            }
        }

        return result;
    }

    private List<Coordinate2D> NeighboursIgnoringOneWay(Coordinate2D coordinate)
    {
        var neighbours = coordinate
            .Neighbours()
            .Where(_map.IsInGrid)
            .ToList();

        var result = new List<Coordinate2D>();
        foreach (var neighbour in neighbours)
        {
            if (_map.Read(neighbour) != '#')
            {
                result.Add(neighbour);
            }
        }

        return result;
    }
}
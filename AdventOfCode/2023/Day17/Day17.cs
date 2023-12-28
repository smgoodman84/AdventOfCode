using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.Graphs;

namespace AdventOfCode._2023.Day17
{
    public class Day17 : Day
    {
        public Day17() : base(2023, 17, "Day17/input_2023_17.txt", "698", "825", true)
        {

        }

        private Grid2D<Location> _map;
        private Coordinate2D _start;
        private Coordinate2D _end;
        public override void Initialise()
        {
            var height = InputLines.Count;
            var width = InputLines[0].Length;

            _start = new Coordinate2D(0, height - 1);
            _end = new Coordinate2D(width - 1, 0);

            _map = new Grid2D<Location>(width, height);
            foreach (var coordinate in _map.AllCoordinates())
            {
                var c = InputLines[(int)(height - 1 - coordinate.Y)][(int)coordinate.X];
                var location = new Location
                {
                    Coordinate = coordinate,
                    HeatLoss = c - '0'
                };

                _map.Write(coordinate, location);
            }
        }
        public override string Part1() => GetMinimumHeatloss(1, 3);
        public override string Part2() => GetMinimumHeatloss(4, 10);

        public string GetMinimumHeatloss(int minDistance, int maxDistance)
        {
            var graph = new Graph<NodeData>();
            foreach (var coordinate in _map.AllCoordinates())
            {
                var heatloss = _map.Read(coordinate).HeatLoss;
                AddNode(graph, coordinate, Direction.Up, heatloss);
                AddNode(graph, coordinate, Direction.Down, heatloss);
                AddNode(graph, coordinate, Direction.Left, heatloss);
                AddNode(graph, coordinate, Direction.Right, heatloss);
            }

            foreach (var coordinate in _map.AllCoordinates())
            {
                if (graph.TryGetNode(NodeData.GetIdentifier(coordinate, Direction.Up), out var enteredFromUp))
                {
                    AddEdges(graph,
                        enteredFromUp,
                        Direction.Right,
                        Coordinate2D.XRange((int)coordinate.X, (int)coordinate.X - minDistance + 1, coordinate.Y).Skip(1),
                        Coordinate2D.XRange((int)coordinate.X - minDistance, (int)coordinate.X - maxDistance, coordinate.Y));

                    AddEdges(graph,
                        enteredFromUp,
                        Direction.Left,
                        Coordinate2D.XRange((int)coordinate.X, (int)coordinate.X + minDistance - 1, coordinate.Y).Skip(1),
                        Coordinate2D.XRange((int)coordinate.X + minDistance, (int)coordinate.X + maxDistance, coordinate.Y));
                }

                if (graph.TryGetNode(NodeData.GetIdentifier(coordinate, Direction.Down), out var enteredFromDown))
                {
                    AddEdges(graph,
                        enteredFromDown,
                        Direction.Right,
                        Coordinate2D.XRange((int)coordinate.X, (int)coordinate.X - minDistance + 1, coordinate.Y).Skip(1),
                        Coordinate2D.XRange((int)coordinate.X - minDistance, (int)coordinate.X - maxDistance, coordinate.Y));

                    AddEdges(graph,
                        enteredFromDown,
                        Direction.Left,
                        Coordinate2D.XRange((int)coordinate.X, (int)coordinate.X + minDistance - 1, coordinate.Y).Skip(1),
                        Coordinate2D.XRange((int)coordinate.X + minDistance, (int)coordinate.X + maxDistance, coordinate.Y));
                }


                if (graph.TryGetNode(NodeData.GetIdentifier(coordinate, Direction.Left), out var enteredFromLeft))
                {
                    AddEdges(graph,
                        enteredFromLeft,
                        Direction.Up,
                        Coordinate2D.YRange((int)coordinate.Y, (int)coordinate.Y - minDistance + 1, coordinate.X).Skip(1),
                        Coordinate2D.YRange((int)coordinate.Y - minDistance, (int)coordinate.Y - maxDistance, coordinate.X));

                    AddEdges(graph,
                        enteredFromLeft,
                        Direction.Down,
                        Coordinate2D.YRange((int)coordinate.Y, (int)coordinate.Y + minDistance - 1, coordinate.X).Skip(1),
                        Coordinate2D.YRange((int)coordinate.Y + minDistance, (int)coordinate.Y + maxDistance, coordinate.X));
                }

                if (graph.TryGetNode(NodeData.GetIdentifier(coordinate, Direction.Right), out var enteredFromRight))
                {
                    AddEdges(graph,
                        enteredFromRight,
                        Direction.Up,
                        Coordinate2D.YRange((int)coordinate.Y, (int)coordinate.Y - minDistance + 1, coordinate.X).Skip(1),
                        Coordinate2D.YRange((int)coordinate.Y - minDistance, (int)coordinate.Y - maxDistance, coordinate.X));

                    AddEdges(graph,
                        enteredFromRight,
                        Direction.Down,
                        Coordinate2D.YRange((int)coordinate.Y, (int)coordinate.Y + minDistance - 1, coordinate.X).Skip(1),
                        Coordinate2D.YRange((int)coordinate.Y + minDistance, (int)coordinate.Y + maxDistance, coordinate.X));
                }
            }

            graph.TryGetNode(NodeData.GetIdentifier(_start, Direction.Right), out var startRight);
            graph.TryGetNode(NodeData.GetIdentifier(_start, Direction.Down), out var startDown);

            graph.TryGetNode(NodeData.GetIdentifier(_end, Direction.Left), out var endLeft);
            graph.TryGetNode(NodeData.GetIdentifier(_end, Direction.Up), out var endUp);

            var shortestPath = int.MaxValue;
            foreach(var start in new[] { startRight, startDown })
            {
                foreach (var end in new[] { endLeft, endUp })
                {
                    var currentShortestPath = graph.GetShortestPathNodesAndDistance(
                        start,
                        end,
                        (int)(10 * _map.Width * _map.Height));

                    if (currentShortestPath.Distance < shortestPath)
                    {
                        shortestPath = currentShortestPath.Distance;
                    }
                }
            }

            return shortestPath.ToString();
        }

        private void AddEdges(
            Graph<NodeData> graph,
            GraphNode<NodeData> startNode,
            Direction enteringNodesFrom,
            IEnumerable<Coordinate2D> startPath,
            IEnumerable<Coordinate2D> path)
        {
            var heatloss = 0;
            foreach (var destinationCoordinate in startPath)
            {
                if (_map.IsInGrid(destinationCoordinate))
                {
                    heatloss += _map.Read(destinationCoordinate).HeatLoss;
                }
            }

            foreach (var destinationCoordinate in path)
            {
                if (graph.TryGetNode(NodeData.GetIdentifier(destinationCoordinate, enteringNodesFrom), out var destinationNode))
                {
                    heatloss += _map.Read(destinationCoordinate).HeatLoss;
                    graph.AddEdge(new GraphEdge<NodeData>
                    {
                        Source = startNode,
                        Destination = destinationNode,
                        Distance = heatloss
                    });
                }
            }
        }

        private void AddNode(
            Graph<NodeData> graph,
            Coordinate2D coordinate,
            Direction direction,
            int heatloss)
        {
            var neighbour = coordinate.Neighbour(direction);
            if (!_map.IsInGrid(neighbour))
            {
                return;
            }

            graph.AddNode(new GraphNode<NodeData>(new NodeData
            {
                Coordinate = coordinate,
                EnteringFrom = direction,
                HeatLoss = heatloss
            }));
        }

        private class NodeData : IGraphNodeData
        {
            public Coordinate2D Coordinate { get; set; }
            public Direction EnteringFrom { get; set; }
            public int HeatLoss { get; set; }

            public static string GetIdentifier(Coordinate2D coordinate, Direction enteringFrom)
            {
                return $"{coordinate} from {enteringFrom}";
            }

            public string GetIdentifier()
            {
                return GetIdentifier(Coordinate, EnteringFrom);
            }
        }

        private class Location
        {
            public Coordinate2D Coordinate { get; set; }
            public int HeatLoss { get; set; }

            public override string ToString()
            {
                return $"{HeatLoss}";
            }
        }
    }
}

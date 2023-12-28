using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.Graphs;

namespace AdventOfCode._2023.Day17
{
    public class Day17 : Day
    {
        public Day17() : base(2023, 17, "Day17/input_2023_17.txt", "698", "", true)
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

        public override string Part1()
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
                    var heatloss = 0;
                    for (var x = coordinate.X - 1; x >= coordinate.X - 3; x -= 1)
                    {
                        var destinationCoordinate = new Coordinate2D(x, coordinate.Y);
                        if (graph.TryGetNode(NodeData.GetIdentifier(destinationCoordinate, Direction.Right), out var destinationNode))
                        {
                            heatloss += _map.Read(destinationCoordinate).HeatLoss;
                            graph.AddEdge(new GraphEdge<NodeData>
                            {
                                Source = enteredFromUp,
                                Destination = destinationNode,
                                Distance = heatloss
                            });
                        }
                    }

                    heatloss = 0;
                    for (var x = coordinate.X + 1; x <= coordinate.X + 3; x += 1)
                    {
                        var destinationCoordinate = new Coordinate2D(x, coordinate.Y);
                        if (graph.TryGetNode(NodeData.GetIdentifier(destinationCoordinate, Direction.Left), out var destinationNode))
                        {
                            heatloss += _map.Read(destinationCoordinate).HeatLoss;
                            graph.AddEdge(new GraphEdge<NodeData>
                            {
                                Source = enteredFromUp,
                                Destination = destinationNode,
                                Distance = heatloss
                            });
                        }
                    }
                }

                if (graph.TryGetNode(NodeData.GetIdentifier(coordinate, Direction.Down), out var enteredFromDown))
                {
                    var heatloss = 0;
                    for (var x = coordinate.X - 1; x >= coordinate.X - 3; x -= 1)
                    {
                        var destinationCoordinate = new Coordinate2D(x, coordinate.Y);
                        if (graph.TryGetNode(NodeData.GetIdentifier(destinationCoordinate, Direction.Right), out var destinationNode))
                        {
                            heatloss += _map.Read(destinationCoordinate).HeatLoss;
                            graph.AddEdge(new GraphEdge<NodeData>
                            {
                                Source = enteredFromDown,
                                Destination = destinationNode,
                                Distance = heatloss
                            });
                        }
                    }

                    heatloss = 0;
                    for (var x = coordinate.X + 1; x <= coordinate.X + 3; x += 1)
                    {
                        var destinationCoordinate = new Coordinate2D(x, coordinate.Y);
                        if (graph.TryGetNode(NodeData.GetIdentifier(destinationCoordinate, Direction.Left), out var destinationNode))
                        {
                            heatloss += _map.Read(destinationCoordinate).HeatLoss;
                            graph.AddEdge(new GraphEdge<NodeData>
                            {
                                Source = enteredFromDown,
                                Destination = destinationNode,
                                Distance = heatloss
                            });
                        }
                    }
                }


                if (graph.TryGetNode(NodeData.GetIdentifier(coordinate, Direction.Left), out var enteredFromLeft))
                {
                    var heatloss = 0;
                    for (var y = coordinate.Y - 1; y >= coordinate.Y - 3; y -= 1)
                    {
                        var destinationCoordinate = new Coordinate2D(coordinate.X, y);
                        if (graph.TryGetNode(NodeData.GetIdentifier(destinationCoordinate, Direction.Up), out var destinationNode))
                        {
                            heatloss += _map.Read(destinationCoordinate).HeatLoss;
                            graph.AddEdge(new GraphEdge<NodeData>
                            {
                                Source = enteredFromLeft,
                                Destination = destinationNode,
                                Distance = heatloss
                            });
                        }
                    }

                    heatloss = 0;
                    for (var y = coordinate.Y + 1; y <= coordinate.Y + 3; y += 1)
                    {
                        var destinationCoordinate = new Coordinate2D(coordinate.X, y);
                        if (graph.TryGetNode(NodeData.GetIdentifier(destinationCoordinate, Direction.Down), out var destinationNode))
                        {
                            heatloss += _map.Read(destinationCoordinate).HeatLoss;
                            graph.AddEdge(new GraphEdge<NodeData>
                            {
                                Source = enteredFromLeft,
                                Destination = destinationNode,
                                Distance = heatloss
                            });
                        }
                    }
                }

                if (graph.TryGetNode(NodeData.GetIdentifier(coordinate, Direction.Right), out var enteredFromRight))
                {
                    var heatloss = 0;
                    for (var y = coordinate.Y - 1; y >= coordinate.Y - 3; y -= 1)
                    {
                        var destinationCoordinate = new Coordinate2D(coordinate.X, y);
                        if (graph.TryGetNode(NodeData.GetIdentifier(destinationCoordinate, Direction.Up), out var destinationNode))
                        {
                            heatloss += _map.Read(destinationCoordinate).HeatLoss;
                            graph.AddEdge(new GraphEdge<NodeData>
                            {
                                Source = enteredFromRight,
                                Destination = destinationNode,
                                Distance = heatloss
                            });
                        }
                    }

                    heatloss = 0;
                    for (var y = coordinate.Y + 1; y <= coordinate.Y + 3; y += 1)
                    {
                        var destinationCoordinate = new Coordinate2D(coordinate.X, y);
                        if (graph.TryGetNode(NodeData.GetIdentifier(destinationCoordinate, Direction.Down), out var destinationNode))
                        {
                            heatloss += _map.Read(destinationCoordinate).HeatLoss;
                            graph.AddEdge(new GraphEdge<NodeData>
                            {
                                Source = enteredFromRight,
                                Destination = destinationNode,
                                Distance = heatloss
                            });
                        }
                    }
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

        public override string Part2()
        {
            return string.Empty;
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

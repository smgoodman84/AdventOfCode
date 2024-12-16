using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.Graphs;

namespace AdventOfCode._2024.Day16;

public class Day16 : Day
{
    public Day16() : base(2024, 16, "Day16/input_2024_16.txt", "92432", "", true)
    {
    }

    Grid2D<MapLocation> _map;
    Graph<Node> _graph = new Graph<Node>();
    GraphNode<Node> _start;
    public override void Initialise()
    {
        _map = Grid2D<MapLocation>.CreateWithCartesianCoordinates(
            InputLines,
            (coord, c) => new MapLocation(coord, c)
        );

        var startLocation = _map.ReadAll().Single(x => x.IsStart);
        _start = new GraphNode<Node>(new Node
        {
            Location = startLocation,
            Direction = Direction.Right
        });
        _graph.AddNode(_start);

        var explored = new HashSet<GraphNode<Node>>();
        var unexplored = new HashSet<GraphNode<Node>>();
        unexplored.Add(_start);

        while (unexplored.Any())
        {
            var exploring = unexplored.First();

            Explore(exploring, explored, unexplored);

            explored.Add(exploring);
            unexplored.Remove(exploring);
        }
    }

    private void Explore(
        GraphNode<Node> exploring,
        HashSet<GraphNode<Node>> explored,
        HashSet<GraphNode<Node>> unexplored)
    {
        TraceLine($"Exploring {exploring.Data}");
        ExploreOnwards(exploring, explored, unexplored);
        ExploreRight(exploring, explored, unexplored);
        ExploreLeft(exploring, explored, unexplored);
    }

    private void ExploreOnwards(
        GraphNode<Node> exploring,
        HashSet<GraphNode<Node>> explored,
        HashSet<GraphNode<Node>> unexplored)
    {
        var cost = 0;
        var current = exploring.Data;
        var addedNode = false;
        var runIntoWall = false;

        while (!(addedNode || runIntoWall))
        {
            var nextLocation = current.Location.Location.Neighbour(exploring.Data.Direction);
            var next = _map.Read(nextLocation);

            if (CanTurnLeft(current)
                || CanTurnRight(current))
            {
                var node = new GraphNode<Node>(new Node
                {
                    Location = current.Location,
                    Direction = exploring.Data.Direction
                });

                if (!explored.Contains(node) && !unexplored.Contains(node))
                {
                    _graph.AddNode(node);
                    unexplored.Add(node);
                    addedNode = true;
                }

                _graph.AddEdge(new GraphEdge<Node>()
                {
                    Source = exploring,
                    Destination = node,
                    Distance = cost
                });
            }

            if (next.LocationType == LocationType.Wall)
            {
                runIntoWall = true;
                // TraceLine($"Exploring {exploring.Data} Run into wall at {nextLocation}");
                if (!current.Location.Equals(exploring.Data.Location.Location))
                {
                    var node = new GraphNode<Node>(new Node
                    {
                        Location = current.Location,
                        Direction = exploring.Data.Direction
                    });

                    if (!explored.Contains(node) && !unexplored.Contains(node))
                    {
                        _graph.AddNode(node);
                        unexplored.Add(node);
                        addedNode = true;
                    }

                    _graph.AddEdge(new GraphEdge<Node>()
                    {
                        Source = exploring,
                        Destination = node,
                        Distance = cost
                    });
                }
            }
            else
            {
                cost += 1;
            }

            current = new Node
            {
                Location = next,
                Direction = current.Direction
            };
        }
    }

    private void ExploreRight(
        GraphNode<Node> exploring,
        HashSet<GraphNode<Node>> explored,
        HashSet<GraphNode<Node>> unexplored)
    {
        if (!CanTurnRight(exploring.Data))
        {
            return;
        }

        var currentDirection = exploring.Data.Direction;
        var newDirection = currentDirection.TurnRight();

        var node = new GraphNode<Node>(new Node
        {
            Location = exploring.Data.Location,
            Direction = newDirection
        });

        if (!explored.Contains(node) && !unexplored.Contains(node))
        {
            _graph.AddNode(node);
            unexplored.Add(node);
        }

        _graph.AddEdge(new GraphEdge<Node>()
        {
            Source = exploring,
            Destination = node,
            Distance = 1000
        });
    }

    private void ExploreLeft(
        GraphNode<Node> exploring,
        HashSet<GraphNode<Node>> explored,
        HashSet<GraphNode<Node>> unexplored)
    {
        if (!CanTurnLeft(exploring.Data))
        {
            return;
        }

        var currentDirection = exploring.Data.Direction;
        var newDirection = currentDirection.TurnLeft();

        var node = new GraphNode<Node>(new Node
        {
            Location = exploring.Data.Location,
            Direction = newDirection
        });

        if (!explored.Contains(node) && !unexplored.Contains(node))
        {
            _graph.AddNode(node);
            unexplored.Add(node);
        }

        _graph.AddEdge(new GraphEdge<Node>()
        {
            Source = exploring,
            Destination = node,
            Distance = 1000
        });
    }

    private bool CanTurnRight(Node exploring)
    {
        var currentDirection = exploring.Direction;
        var newDirection = currentDirection.TurnRight();
        var nextLocation = exploring.Location.Location.Neighbour(newDirection);
        var next = _map.Read(nextLocation);

        if (next.LocationType == LocationType.Wall)
        {
            return false;
        }

        // TraceLine($"Can turn right from {exploring}");
        return true;
    }

    private bool CanTurnLeft(Node exploring)
    {
        var currentDirection = exploring.Direction;
        var newDirection = currentDirection.TurnLeft();
        var nextLocation = exploring.Location.Location.Neighbour(newDirection);
        var next = _map.Read(nextLocation);

        if (next.LocationType == LocationType.Wall)
        {
            return false;
        }

        // TraceLine($"Can turn left from {exploring}");
        return true;
    }

    public override string Part1()
    {
        var endNodes = _graph
            .AllNodes()
            .Where(n => n.Data.Location.IsEnd)
            .ToList();
        
        var minCost = int.MaxValue;
        foreach (var endNode in endNodes)
        {
            try
            {
                var shortestPath = _graph.GetShortestPathDistance(_start, endNode, 1000000);
                if (shortestPath < minCost)
                {
                    minCost = shortestPath;
                }
            }
            catch (Exception ex)
            {
                var stop = "here";
            }
        }
        return minCost.ToString();
    }

    public override string Part2()
    {
        return string.Empty;
    }

    private class Node : IGraphNodeData
    {
        public MapLocation Location { get; set; }
        public Direction Direction { get; set; }

	    public string GetIdentifier()
        {
            return $"{Location.Location} {Direction}";
        }

        public override string ToString()
        {
            return GetIdentifier();
        }

        public override bool Equals(object obj)
        {
            var node = obj as Node;
            if (node == null)
            {
                return false;
            }
            return node.GetIdentifier().Equals(GetIdentifier());
        }

        public override int GetHashCode()
        {
            return GetIdentifier().GetHashCode();
        }
    }

    private class MapLocation
    {
        public Coordinate2D Location { get; set; }
        public LocationType LocationType { get; set; }

        public bool IsStart { get; set; }
        public bool IsEnd { get; set; }

        public MapLocation(Coordinate2D location, char c)
        {
            Location = location;
            LocationType = LocationType.Empty;
            IsStart = false;
            IsEnd = false;

            switch (c)
            {
                case '#':
                    LocationType = LocationType.Wall;
                    break;
                case 'S':
                    IsStart = true;
                    break;
                case 'E':
                    IsEnd = true;
                    break;
            }
        }
    }

    private enum LocationType
    {
        Empty,
        Wall
    }
}
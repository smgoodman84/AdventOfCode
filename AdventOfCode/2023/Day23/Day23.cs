using System.Xml.Linq;
using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.Graphs;

namespace AdventOfCode._2023.Day23
{
    public class Day23 : Day
    {
        public Day23() : base(2023, 23, "Day23/input_2023_23.txt", "2354", "", true)
        {

        }

        private Grid2D<char> _map;
        private Dictionary<Coordinate2D, Path> _paths;
        private Coordinate2D _start;
        private Coordinate2D _end;
        private int _pathIndex;

        private Graph<NodeData> _graph;
        public override void Initialise()
        {
            _map = GridReader.LoadGrid(InputLines, (c, _) => c);
            _start = new Coordinate2D(1, _map.MaxY);
            _end = new Coordinate2D(_map.MaxX - 1, 0);
            _paths = new Dictionary<Coordinate2D, Path>();
            _pathIndex = 0;
        }

        public override string Part1()
        {
            // return string.Empty;
            LoadPaths(_start, new HashSet<string>(), CanMoveTo);
            // DrawPaths();
            // DrawPathsTogether();

            var firstPath = _paths[_start];
            var result = GetLongestPath(firstPath) - 1;
            return result.ToString();
        }

        public override string Part2()
        {
            Initialise();

            LoadGraph();
            _graph.TryGetNode(_start.ToString(), out var startNode);
            _graph.TryGetNode(_end.ToString(), out var endNode);
            var longestPath = _graph.GetLongestPath(startNode, endNode,(int)MaxDistance());

            var maxDistance = MaxDistance();
            TraceLine($"Max Distance: {maxDistance}");
            TraceLine($"Longest Path: {longestPath}");
            var result = maxDistance - longestPath;
            return longestPath.ToString();
        }

        private long MaxDistance()
        {
            return _map.Width * _map.Height;
        }

        private void LoadGraph()
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
                        var neighbours = Neighbours(coordinate);
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
                LoadEdges(node);
            }
        }

        private void LoadEdges(GraphNode<NodeData> node)
        {
            var start = node.Data.Location;
            var neighbours = Neighbours(start);
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
                        .Where(n => _map.IsInGrid(n))
                        .Where(n => !n.Equals(previous))
                        .Where(n => _map.Read(n) != '#')
                        .Single();

                    previous = current;
                    current = next;
                    distance += 1;
                }

                int savedDistance = (int)MaxDistance() - distance;
                ///TraceLine($"Adding edge from {node} to {end} with distance {savedDistance} ({distance})");
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

        private List<Coordinate2D> Neighbours(Coordinate2D coordinate)
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

        private int GetLongestPath(Path path)
        {
            var longestChild = 0;
            foreach(var childPath in path.NextPaths)
            {
                var childLength = GetLongestPath(childPath);
                if (childLength > longestChild)
                {
                    longestChild = childLength;
                }
            }

            var totalLongest = longestChild + path.Length;

            // TraceLine($"Path {path.PathIndex} Longest = {totalLongest} ({path.Length} + {longestChild})");
            return totalLongest;
        }

        private void DrawPaths()
        {
            foreach(var path in _paths.Values.OrderBy(p => p.PathIndex))
            {
                TraceLine($"Path {path.PathIndex}: {path.Start} -> {path.End} ({path.Length})");
                foreach (var y in _map.YIndexes().OrderByDescending(y => y))
                {
                    foreach (var x in _map.XIndexes())
                    {
                        if (path.AllSteps.Contains(new Coordinate2D(x, y)))
                        {
                            Trace("O");
                        }
                        else
                        {
                            Trace('.');
                        }
                    }
                    TraceLine();
                }
            }
        }

        private void DrawPathsTogether()
        {
            foreach (var y in _map.YIndexes().OrderByDescending(y => y))
            {
                foreach (var x in _map.XIndexes())
                {
                    var path = _paths.Values.FirstOrDefault(p => p.AllSteps.Contains(new Coordinate2D(x, y)));
                    if (path == null)
                    {
                        Trace(" ");
                    }
                    else
                    {
                        Trace((path.PathIndex % 10).ToString());
                    }
                }
                TraceLine();
            }
        }

        private void LoadPaths(
            Coordinate2D start,
            HashSet<string> visited,
            Func<Coordinate2D, Coordinate2D, HashSet<string>, bool> canMoveTo,
            Coordinate2D? previousEnd = null)
        {
            // TraceLine($"Loading paths from {start} with previousEnd {previousEnd}");
            var allSteps = new List<Coordinate2D>()
            {
                start
            };
            visited.Add(start.ToString());
            var length = 1;

            var current = start;
            var previous = previousEnd;
            while (true)
            {
                var nextSteps = GetNextSteps(current, visited, previous, canMoveTo);
                if (nextSteps.Count > 1)
                {
                    if (!_paths.Values.Any(p => p.Start == start && p.End == current))
                    {
                        var path = new Path
                        {
                            Start = start,
                            End = current,
                            Length = length,
                            AllSteps = allSteps,
                            NextPaths = new List<Path>(),
                            PathIndex = _pathIndex++
                        };

                        // TraceLine($"Got path from {start} to {current}");
                        foreach (var nextStep in nextSteps)
                        {
                            var duplicateVisited = visited.ToHashSet<string>();
                            LoadPaths(nextStep, duplicateVisited, canMoveTo, current);
                            if (_paths.ContainsKey(nextStep))
                            {
                                path.NextPaths.Add(_paths[nextStep]);
                            }
                        }

                        _paths.Add(path.Start, path);
                    }
                    return;
                }

                if (!nextSteps.Any())
                {
                    return;
                }

                var next = nextSteps.Single();
                allSteps.Add(next);
                visited.Add(next.ToString());
                length += 1;

                previous = current;
                current = next;

                if (current.Equals(_end))
                {
                    if (!_paths.Values.Any(p => p.Start == start && p.End == current))
                    {
                        var path = new Path
                        {
                            Start = start,
                            End = current,
                            Length = length,
                            AllSteps = allSteps,
                            NextPaths = new List<Path>(),
                            PathIndex = _pathIndex++
                        };

                        _paths.Add(path.Start, path);
                    }
                    return;
                }
            }
        }

        private List<Coordinate2D> GetNextSteps(
            Coordinate2D start,
            HashSet<string> visited,
            Coordinate2D? previous,
            Func<Coordinate2D, Coordinate2D, HashSet<string>, bool> canMoveTo)
        {
            var result = new List<Coordinate2D>();

            var neighbours = start.Neighbours();
            foreach (var neighbour in neighbours)
            {
                if (!_map.IsInGrid(neighbour))
                {
                    continue;
                }

                if (previous != null && neighbour.Equals(previous))
                {
                    continue;
                }

                if (canMoveTo(start, neighbour, visited))
                {
                    result.Add(neighbour);
                    // TraceLine($"Next step {neighbour}");
                }
            }

            return result;
        }

        private bool CanMoveTo(Coordinate2D from, Coordinate2D to, HashSet<string> visited)
        {
            var toValue = _map.Read(to);
            var fromValue = _map.Read(from);

            if (toValue == '#')
            {
                return false;
            }

            if (fromValue == '.' && toValue == '.')
            {
                return true;
            }

            switch (fromValue)
            {
                case '<': return to.Equals(from.Left());
                case '>': return to.Equals(from.Right());
                case '^': return to.Equals(from.Up());
                case 'v': return to.Equals(from.Down());
            }

            switch (toValue)
            {
                case '<': return to.Equals(from.Left());
                case '>': return to.Equals(from.Right());
                case '^': return to.Equals(from.Up());
                case 'v': return to.Equals(from.Down());
            }

            return toValue == '.';
        }

        private bool CanMoveToPart2(Coordinate2D from, Coordinate2D to, HashSet<string> visited)
        {
            if (from.X == 3L && from.Y == 19L)
            {
                var stop = true;
            }

            var toValue = _map.Read(to);

            if (toValue == '#')
            {
                return false;
            }

            if (visited.Contains(to.ToString()))
            {
                return false;
            }

            return true;
        }

        private class Path
        {
            public int PathIndex { get; set; }
            public Coordinate2D Start { get; set; }
            public Coordinate2D End { get; set; }
            public int Length { get; set; }
            public List<Coordinate2D> AllSteps { get; set; }
            public List<Path> NextPaths { get; set; }
        }
    }
}

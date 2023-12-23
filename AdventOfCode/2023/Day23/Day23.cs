using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2023.Day23
{
    public class Day23 : Day
    {
        public Day23() : base(2023, 23, "Day23/input_2023_23.txt", "2354", "", true)
        {

        }

        private Grid2D<char> _map;
        private Dictionary<Coordinate2D, Path> _paths = new Dictionary<Coordinate2D, Path>();
        private Coordinate2D _start;
        private Coordinate2D _end;
        private int _pathIndex = 0;
        public override void Initialise()
        {
            _map = GridReader.LoadGrid(InputLines, (c, _) => c);
            _start = new Coordinate2D(1, _map.MaxY);
            _end = new Coordinate2D(_map.MaxX - 1, 0);
        }

        public override string Part1()
        {
            LoadPaths(_start);
            // DrawPaths();
            DrawPathsTogether();

            var firstPath = _paths[_start];
            var result = GetLongestPath(firstPath) - 1;
            return result.ToString();
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

            TraceLine($"Path {path.PathIndex} Longest = {totalLongest} ({path.Length} + {longestChild})");
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

        private void LoadPaths(Coordinate2D start, Coordinate2D? previousEnd = null)
        {
            TraceLine($"Loading paths from {start} with previousEnd {previousEnd}");
            var allSteps = new List<Coordinate2D>()
            {
                start
            };
            var length = 1;

            var current = start;
            var previous = previousEnd;
            while (true)
            {
                var nextSteps = GetNextSteps(current, previous);
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

                        TraceLine($"Got path from {start} to {current}");
                        foreach (var nextStep in nextSteps)
                        {
                            LoadPaths(nextStep, current);
                            path.NextPaths.Add(_paths[nextStep]);
                        }

                        _paths.Add(path.Start, path);
                    }
                    return;
                }

                var next = nextSteps.Single();
                allSteps.Add(next);
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

        private List<Coordinate2D> GetNextSteps(Coordinate2D start, Coordinate2D? previous = null)
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

                if (CanMoveTo(start, neighbour))
                {
                    result.Add(neighbour);
                    // TraceLine($"Next step {neighbour}");
                }
            }

            return result;
        }

        private bool CanMoveTo(Coordinate2D from, Coordinate2D to)
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

        public override string Part2()
        {
            return string.Empty;
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

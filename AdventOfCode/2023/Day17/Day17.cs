using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2023.Day17
{
    public class Day17 : Day
    {
        public Day17() : base(2023, 17, "Day17/input_2023_17.txt", "", "", true)
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
            var end = _map.Read(_end);

            var previousBest = 999_999;
            var again = true;
            while (again)
            {
                CalculateShortestPaths();

                var shortestPath = end.ShortestPaths.OrderBy(p => p.TotalHeatLoss).First();
                var minimumHeatLoss = shortestPath.TotalHeatLoss;

                if (minimumHeatLoss < previousBest)
                {
                    again = true;
                    previousBest = minimumHeatLoss;
                    TraceLine($"New best: {previousBest}");
                }
                else
                {
                    again = false;
                }
            }


            foreach (var path in end.ShortestPaths.OrderBy(p => p.TotalHeatLoss))
            {
                // DrawPath(path);
            }

            // 947 - too high (10, 10)
            // 981 - too high
            // 1037 - too high
            return previousBest.ToString();
        }

        private void DrawPath(Path path)
        {
            TraceLine();
            foreach (var y in _map.YIndexes().OrderByDescending(y => y))
            {
                foreach (var x in _map.XIndexes())
                {
                    if (path.Steps.Any(s => s.X == x && s.Y == y))
                    {
                        Trace("#");
                    }
                    else
                    {
                        Trace(".");
                    }
                }
                TraceLine();
            }
        }

        private void CalculateShortestPaths()
        {
            var currentStart = _start;
            CalculateShortestPaths(currentStart);

            while (currentStart.Y > 0)
            {
                currentStart = currentStart.Down();

                var currentPosition = currentStart;
                CalculateShortestPaths(currentPosition);

                var inGrid = true;
                while (inGrid)
                {
                    currentPosition = currentPosition.Up().Right();
                    if (_map.IsInGrid(currentPosition))
                    {
                        CalculateShortestPaths(currentPosition);
                    }
                    else
                    {
                        inGrid = false;
                    }
                }
            }

            while (currentStart.X < _map.MaxX)
            {
                currentStart = currentStart.Right();

                var currentPosition = currentStart;
                CalculateShortestPaths(currentPosition);

                var inGrid = true;
                while (inGrid)
                {
                    currentPosition = currentPosition.Up().Right();
                    if (_map.IsInGrid(currentPosition))
                    {
                        CalculateShortestPaths(currentPosition);
                    }
                    else
                    {
                        inGrid = false;
                    }
                }
            }
        }

        public override string Part2()
        {
            return string.Empty;
        }

        private void CalculateShortestPaths(Coordinate2D coordinate)
        {
            var location = _map.Read(coordinate);
            /*
            if (location.ShortestPaths != null)
            {
                TraceLine($"Already Got Shortest Path {location.Coordinate}");
                return;
            }
            */
            // TraceLine($"Calculating Shortest Path {location.Coordinate}");

            if (location.Coordinate.Equals(_start))
            {
                location.ShortestPaths = new List<Path>
                {
                    new Path
                    {
                        Steps = new List<Coordinate2D>()
                        {
                            _start
                        },
                        TotalHeatLoss = 0
                    }
                };

                return;
            }

            var validNeighbours = location.Coordinate.Neighbours()
                .Where(_map.IsInGrid)
                .Select(_map.Read)
                .ToList();

            var paths = location.ShortestPaths ?? new List<Path>();
            foreach (var neighbour in validNeighbours)
            {
                // CalculateShortestPaths(neighbour);

                var neighbourShortestPaths = neighbour.ShortestPaths;
                if (neighbourShortestPaths != null)
                {
                    foreach (var path in neighbourShortestPaths)
                    {
                        var pathToHere = new Path
                        {
                            Steps = path.Steps.Concat(new[] { location.Coordinate }).ToList(),
                            TotalHeatLoss = path.TotalHeatLoss + location.HeatLoss
                        };

                        if (IsValidPath(pathToHere))
                        {
                            paths.Add(pathToHere);
                        }
                    }
                }
            }

            location.ShortestPaths = paths
                .OrderBy(p => p.TotalHeatLoss)
                .Take(35)
                // 3 -> 846
                // 5 -> 841
                // 10 -> 827
                // 15 -> 755
                // 18 -> 810 ?!
                // 25 -> 738
                // 35 -> 748
                .ToList();
        }

        private bool IsValidPath(Path path)
        {
            var steps = path.Steps;
            var lastStepIndex = steps.Count - 1;


            if (steps.Count > 2)
            {
                if (steps[lastStepIndex].Equals(steps[lastStepIndex-2]))
                {
                    return false;
                }
            }

            var dX = new List<long>();
            var dY = new List<long>();

            var startCheckFromIndex = lastStepIndex - 4;
            if (startCheckFromIndex < 0)
            {
                return true;
            }

            var currentIndex = startCheckFromIndex;
            while (currentIndex < lastStepIndex)
            {
                var currentStep = steps[currentIndex];
                var nextStep = steps[currentIndex + 1];

                dX.Add(nextStep.X - currentStep.X);
                dY.Add(nextStep.Y - currentStep.Y);

                currentIndex += 1;
            }

            if (dX.GroupBy(x => x).Count() == 1 && dY.GroupBy(y => y).Count() == 1)
            {
                return false;
            }

            return true;
        }


        private class Path
        {
            public List<Coordinate2D> Steps { get; set; }
            public int TotalHeatLoss { get; set; }
        }

        private class Location
        {
            public Coordinate2D Coordinate { get; set; }
            public int HeatLoss { get; set; }
            public List<Path> ShortestPaths { get; set; }

            public override string ToString()
            {
                return $"{HeatLoss}";
            }
        }
    }
}

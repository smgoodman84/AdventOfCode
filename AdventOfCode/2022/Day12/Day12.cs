using System.Linq;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2022.Day12
{
    public class Day12 : Day
    {
        public Day12() : base(2022, 12, "Day12/input_2022_12.txt", "534", "525")
        {

        }

        private Grid2D<Location> _locations;
        private Coordinate2D _startLocation;
        private Coordinate2D _endLocation;

        public override void Initialise()
        {
            var height = InputLines.Count;
            var width = InputLines.First().Length;

            _locations = new Grid2D<Location>(width, height);

            var y = 0;
            foreach (var line in InputLines)
            {
                var x = 0;
                foreach (var c in line)
                {
                    if (c == 'S')
                    {
                        _startLocation = new Coordinate2D(x, y);
                    }

                    if (c == 'E')
                    {
                        _endLocation = new Coordinate2D(x, y);
                    }

                    var locationHeight = GetHeight(c);
                    _locations.Write(x, y, new Location(locationHeight));

                    x += 1;
                }

                y += 1;
            }
        }

        private static int GetHeight(char c)
        {
            if (c == 'S')
            {
                return 0;
            }

            if (c == 'E')
            {
                return 25;
            }

            return "abcdefghijklmnopqrstuvwxyz".IndexOf(c);
        }

        public override string Part1()
        {
            Explore(_startLocation, new Path2D(Enumerable.Empty<Coordinate2D>()));
            var shortestPath = _locations.Read(_endLocation).Path.Length;
            return shortestPath.ToString();
        }

        private void Explore(Coordinate2D location, Path2D pathToLocation)
        {
            if (_locations.Read(location).SaveShortestPath(pathToLocation))
            {
                foreach (var neighbour in location.Neighbours())
                {
                    if (_locations.IsInGrid(neighbour))
                    {
                        if (_locations.Read(neighbour).Height <= _locations.Read(location).Height + 1)
                        {
                            Explore(neighbour, pathToLocation.Append(neighbour));
                        }
                    }
                }
            }
        }

        public override string Part2()
        {
            Initialise();

            Explore2(_endLocation, new Path2D(Enumerable.Empty<Coordinate2D>()));

            var pathsToLowest = _locations
                .ReadAll()
                .Where(x => x.Height == 0)
                .ToList();

            var result = pathsToLowest
                .OrderBy(x => x.Path == null ? int.MaxValue : x.Path.Length)
                .First();

            return result.Path.Length.ToString();
        }

        private void Explore2(Coordinate2D location, Path2D pathToLocation)
        {
            if (_locations.Read(location).SaveShortestPath(pathToLocation))
            {
                foreach (var neighbour in location.Neighbours())
                {
                    if (_locations.IsInGrid(neighbour))
                    {
                        if (_locations.Read(neighbour).Height >= _locations.Read(location).Height - 1)
                        {
                            Explore2(neighbour, pathToLocation.Append(neighbour));
                        }
                    }
                }
            }
        }

        private class Location
        {
            public int Height { get; }
            public Path2D Path { get; private set; }

            public Location(int height)
            {
                Height = height;
            }

            public bool SaveShortestPath(Path2D path)
            {
                if (Path == null
                    || path.Length < Path.Length)
                {
                    Path = path;
                    return true;
                }

                return false;
            }
        }
    }
}


using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2023.Day17
{
    public class Day17 : Day
    {
        public Day17() : base(2023, 17, "Day17/input_2023_17.txt", "", "", false)
        {

        }

        private Grid2D<Location> _map;
        public override void Initialise()
        {
            var height = InputLines.Count;
            _map = new Grid2D<Location>(InputLines[0].Length, height);
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
            return string.Empty;
        }

        public override string Part2()
        {
            return string.Empty;
        }

        private Path GetShortestPath(Location location)
        {
            if (location.ShortestPaths != default)
            {
                return location.ShortestPaths
                    .OrderByDescending(x => x.TotalHeatLoss)
                    .First();
            }

            var validNeighbours = location.Coordinate.Neighbours()
                .Where(n => _map.IsInGrid(n))
                .ToList();

            var shortestPaths = new List<Path>();
            foreach (var neighbour in validNeighbours)
            {
                // var shortestPaths
            }

            return null;
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

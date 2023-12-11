using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2023.Day11
{
    public class Day11 : Day
    {
        public Day11() : base(2023, 11, "Day11/input_2023_11.txt", "9684228", "483844716556", true)
        {

        }

        private Grid2D<Space> _map;
        private List<Space> _galaxies;
        private List<long> _emptyRows;
        private List<long> _emptyColumns;
        public override void Initialise()
        {
            var galaxyNumber = 1;
            _galaxies = new List<Space>();
            _map = new Grid2D<Space>(InputLines[0].Length, InputLines.Count);
            {
                var y = 0;
                foreach (var line in InputLines)
                {
                    var x = 0;
                    foreach (var c in line)
                    {
                        if (c == '#')
                        {
                            var galaxy = new Space(galaxyNumber, new Coordinate2D(x, y));
                            _galaxies.Add(galaxy);
                            _map.Write(x, y, galaxy);
                            galaxyNumber += 1;
                        }
                        else
                        {
                            _map.Write(x, y, new Space(null, new Coordinate2D(x, y)));
                        }

                        x += 1;
                    }
                    y += 1;
                }
            }

            _emptyRows = new List<long>();
            for (var currentY = _map.MinY; currentY <= _map.MaxY; currentY++)
            {
                if (_map.ReadRow(currentY).All(s => !s.IsGalaxy))
                {
                    TraceLine($"Row {currentY} Empty");
                    _emptyRows.Add(currentY);
                }
            }

            _emptyColumns = new List<long>();
            for (var currentX = _map.MinX; currentX <= _map.MaxX; currentX++)
            {
                if (_map.ReadColumn(currentX).All(s => !s.IsGalaxy))
                {
                    TraceLine($"Column {currentX} Empty");
                    _emptyColumns.Add(currentX);
                }
            }
        }

        public override string Part1()
        {
            foreach (var galaxy in _galaxies)
            {
                var newX = galaxy.Location.X + _emptyColumns.Count(c => c < galaxy.Location.X);
                var newY = galaxy.Location.Y + _emptyRows.Count(c => c < galaxy.Location.Y);
                galaxy.NewLocation = new Coordinate2D(newX, newY);
            }

            var totalShortestPath = 0L;
            foreach(var g1 in _galaxies)
            {
                foreach (var g2 in _galaxies)
                {
                    if (g1.GalaxyNumber < g2.GalaxyNumber)
                    {
                        var shortestPath = g1.NewLocation.ManhattanDistanceTo(g2.NewLocation);
                        // TraceLine($"{g1.GalaxyNumber} -> {g2.GalaxyNumber}; {g1.NewLocation} -> {g2.NewLocation} = {shortestPath}");
                        totalShortestPath += shortestPath;
                    }
                }
            }

            return totalShortestPath.ToString();
        }

        public override string Part2()
        {
            foreach (var galaxy in _galaxies)
            {
                var newX = galaxy.Location.X + (_emptyColumns.Count(c => c < galaxy.Location.X) * 999_999L);
                var newY = galaxy.Location.Y + (_emptyRows.Count(c => c < galaxy.Location.Y) * 999_999L);
                galaxy.NewLocationPart2 = new Coordinate2D(newX, newY);
            }

            var totalShortestPath = 0L;
            foreach (var g1 in _galaxies)
            {
                foreach (var g2 in _galaxies)
                {
                    if (g1.GalaxyNumber < g2.GalaxyNumber)
                    {
                        var shortestPath = g1.NewLocationPart2.ManhattanDistanceTo(g2.NewLocationPart2);
                        // TraceLine($"{g1.GalaxyNumber} -> {g2.GalaxyNumber}; {g1.NewLocationPart2} -> {g2.NewLocationPart2} = {shortestPath}");
                        totalShortestPath += shortestPath;
                    }
                }
            }

            return totalShortestPath.ToString();
        }

        private class Space
        {
            public Space(int? galaxyNumber, Coordinate2D location)
            {
                GalaxyNumber = galaxyNumber;
                Location = location;
            }

            public int? GalaxyNumber { get; }
            public Coordinate2D Location { get; }
            public Coordinate2D NewLocation { get; set; }
            public Coordinate2D NewLocationPart2 { get; set; }

            public bool IsGalaxy => GalaxyNumber.HasValue;
        }
    }
}


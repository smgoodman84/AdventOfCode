using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2023.Day11
{
    public class Day11 : Day
    {
        public Day11() : base(2023, 11, "Day11/input_2023_11.txt", "9684228", "483844716556", false)
        {

        }

        private List<Space> _galaxies;
        private Dictionary<long, int> _previousEmptyRows;
        private Dictionary<long, int> _previousEmptyColumns;
        public override void Initialise()
        {
            var galaxyNumber = 1;
            _galaxies = new List<Space>();
            var map = new Grid2D<Space>(InputLines[0].Length, InputLines.Count);
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
                            map.Write(x, y, galaxy);
                            galaxyNumber += 1;
                        }
                        else
                        {
                            map.Write(x, y, new Space(null, new Coordinate2D(x, y)));
                        }

                        x += 1;
                    }
                    y += 1;
                }
            }

            _previousEmptyRows = new Dictionary<long, int>();
            var emptyRows = 0;
            for (var currentY = map.MinY; currentY <= map.MaxY; currentY++)
            {
                _previousEmptyRows.Add(currentY, emptyRows);
                if (map.ReadRow(currentY).All(s => !s.IsGalaxy))
                {
                    TraceLine($"Row {currentY} Empty");
                    emptyRows += 1;
                }
            }

            _previousEmptyColumns = new Dictionary<long, int>();
            var emptyColumns = 0;
            for (var currentX = map.MinX; currentX <= map.MaxX; currentX++)
            {
                _previousEmptyColumns.Add(currentX, emptyColumns);
                if (map.ReadColumn(currentX).All(s => !s.IsGalaxy))
                {
                    TraceLine($"Column {currentX} Empty");
                    emptyColumns += 1;
                }
            }
        }

        private long GetShortestPathsAfterExpansions(long expansionCount)
        {
            var newLocations = new Dictionary<int, Coordinate2D>();
            foreach (var galaxy in _galaxies)
            {
                var newX = galaxy.Location.X + (_previousEmptyColumns[galaxy.Location.X] * expansionCount);
                var newY = galaxy.Location.Y + (_previousEmptyRows[galaxy.Location.Y] * expansionCount);
                newLocations.Add(galaxy.GalaxyNumber.Value, new Coordinate2D(newX, newY));
            }

            var totalShortestPath = 0L;
            foreach (var g1 in _galaxies)
            {
                foreach (var g2 in _galaxies)
                {
                    if (g1.GalaxyNumber < g2.GalaxyNumber)
                    {
                        var g1Location = newLocations[g1.GalaxyNumber.Value];
                        var g2Location = newLocations[g2.GalaxyNumber.Value];
                        var shortestPath = g1Location.ManhattanDistanceTo(g2Location);

                        TraceLine($"{g1.GalaxyNumber} -> {g2.GalaxyNumber}; {g1Location} -> {g2Location} = {shortestPath}");
                        totalShortestPath += shortestPath;
                    }
                }
            }

            return totalShortestPath;
        }

        public override string Part1()
        {
            return GetShortestPathsAfterExpansions(1).ToString();
        }

        public override string Part2()
        {

            return GetShortestPathsAfterExpansions(999_999).ToString();
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

            public bool IsGalaxy => GalaxyNumber.HasValue;
        }
    }
}


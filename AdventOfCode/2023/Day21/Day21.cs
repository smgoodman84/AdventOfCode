using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2023.Day21
{
    public class Day21 : Day
    {
        public Day21() : base(2023, 21, "Day21/input_2023_21.txt", "3762", "", true)
        {

        }

        Grid2D<Location> _map;
        public override void Initialise()
        {
            _map = GridReader.LoadGrid(InputLines, (c, coord) => new Location(coord, c));
        }

        public override string Part1()
        {
            var start = _map.ReadAll().First(x => x.IsStart);
            Flood(start, 0);

            foreach(var y in _map.YIndexes().OrderByDescending(y => y))
            {
                foreach (var x in _map.XIndexes())
                {
                    var location = _map.Read(x, y);
                    if (location.IsRock)
                    {
                        Trace("#");
                    }
                    else if (location.MinDistance.HasValue)
                    {
                        Trace((location.MinDistance % 10).ToString());
                    }
                    else
                    {
                        Trace(".");
                    }
                }
                TraceLine();
            }

            var reachable = _map.ReadAll()
                .Where(x => x.MinDistance.HasValue && x.MinDistance.Value % 2 == 0)
                .Count(x => x.MinDistance <= 64);

            return reachable.ToString();
        }

        private void Flood(Location location, int distance)
        {
            if (location.IsRock)
            {
                return;
            }

            if (location.MinDistance.HasValue)
            {
                if (distance >= location.MinDistance.Value)
                {
                    return;
                }

                 // TraceLine($"Shorter distance {location.Coordinate} = {distance}");
            }

            // TraceLine($"{location.Coordinate} = {distance}");
            location.MinDistance = distance;
            foreach (var neighbour in location.Coordinate.Neighbours())
            {
                if (_map.IsInGrid(neighbour))
                {
                    var neighbouringLocation = _map.Read(neighbour);
                    Flood(neighbouringLocation, distance + 1);
                }
            }
        }

        public override string Part2()
        {
            return string.Empty;
        }

        private class Location
        {
            public int? MinDistance { get; set; }
            public bool IsRock { get; set; } = false;
            public bool IsStart { get; set; } = false;
            public Location(Coordinate2D coordinate, char c)
            {
                Coordinate = coordinate;

                switch (c)
                {
                    case 'S':
                        IsStart = true;
                        break;
                    case '#':
                        IsRock = true;
                        break;
                }
            }

            public Coordinate2D Coordinate { get; }
        }
    }
}

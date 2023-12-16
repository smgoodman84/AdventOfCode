using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2023.Day16
{
    public class Day16 : Day
    {
        public Day16() : base(2023, 16, "Day16/input_2023_16.txt", "8098", "", false)
        {

        }

        private Grid2D<Location> _map;
        private HashSet<string> _followed = new HashSet<string>();
        public override void Initialise()
        {
            var height = InputLines.Count;
            _map = new Grid2D<Location>(InputLines[0].Length, height);
            foreach (var y in _map.YIndexes())
            {
                foreach (var x in _map.XIndexes())
                {
                    var location = new Location
                    {
                        Type = InputLines[(int)(height - 1 - y)][(int)x],
                        EnergizedCount = 0
                    };

                    _map.Write(x, y, location);
                }
            }
        }

        public override string Part1()
        {
            FollowBeam(new Coordinate2D(0, _map.MaxY), Direction.Left);

            var energizedCount = _map.ReadAll().Count(x => x.EnergizedCount > 0);


            foreach (var y in _map.YIndexes().OrderByDescending(y => y))
            {
                foreach (var x in _map.XIndexes())
                {
                    var location = _map.Read(x, y);
                    Trace(location.EnergizedCount > 0 ? '#' : '.');
                }
                TraceLine();
            }

            return energizedCount.ToString();
        }

        private void FollowBeam(Coordinate2D coordinate, Direction entersFrom)
        {
            if (!_map.IsInGrid(coordinate))
            {
                TraceLine($"Left the grid from {entersFrom} at {coordinate}");
                return;
            }

            var description = $"{coordinate} {entersFrom}";
            if (_followed.Contains(description))
            {
                TraceLine($"Already tracing from {entersFrom} at {coordinate}");
                return;
            }
            _followed.Add(description);

            var location = _map.Read(coordinate);
            location.EnergizedCount += 1;
            TraceLine($"Energized {coordinate}");

            var up = coordinate.Up();
            var down = coordinate.Down();
            var left = coordinate.Left();
            var right = coordinate.Right();

            switch (location.Type)
            {
                case '.':
                    switch (entersFrom)
                    {
                        case Direction.Left:
                            FollowBeam(right, Direction.Left);
                            break;
                        case Direction.Right:
                            FollowBeam(left, Direction.Right);
                            break;
                        case Direction.Up:
                            FollowBeam(down, Direction.Up);
                            break;
                        case Direction.Down:
                            FollowBeam(up, Direction.Down);
                            break;
                    }
                    break;
                case '/':
                    switch (entersFrom)
                    {
                        case Direction.Left:
                            FollowBeam(up, Direction.Down);
                            break;
                        case Direction.Right:
                            FollowBeam(down, Direction.Up);
                            break;
                        case Direction.Up:
                            FollowBeam(left, Direction.Right);
                            break;
                        case Direction.Down:
                            FollowBeam(right, Direction.Left);
                            break;
                    }
                    break;
                case '\\':
                    switch (entersFrom)
                    {
                        case Direction.Left:
                            FollowBeam(down, Direction.Up);
                            break;
                        case Direction.Right:
                            FollowBeam(up, Direction.Down);
                            break;
                        case Direction.Up:
                            FollowBeam(right, Direction.Left);
                            break;
                        case Direction.Down:
                            FollowBeam(left, Direction.Right);
                            break;
                    }
                    break;
                case '-':
                    switch (entersFrom)
                    {
                        case Direction.Left:
                            FollowBeam(right, Direction.Left);
                            break;
                        case Direction.Right:
                            FollowBeam(left, Direction.Right);
                            break;
                        case Direction.Up:
                        case Direction.Down:
                            FollowBeam(left, Direction.Right);
                            FollowBeam(right, Direction.Left);
                            break;
                    }
                    break;
                case '|':
                    switch (entersFrom)
                    {
                        case Direction.Left:
                        case Direction.Right:
                            FollowBeam(down, Direction.Up);
                            FollowBeam(up, Direction.Down);
                            break;
                        case Direction.Up:
                            FollowBeam(down, Direction.Up);
                            break;
                        case Direction.Down:
                            FollowBeam(up, Direction.Down);
                            break;
                    }
                    break;
            }
        }

        public override string Part2()
        {
            return string.Empty;
        }

        private class Location
        {
            public char Type { get; set; }
            public int EnergizedCount { get; set; }

            public override string ToString()
            {
                return $"{Type} {EnergizedCount}";
            }
        }

        private class BeamStart
        {
            public Direction EntersFrom { get; set; }
            public Coordinate2D Coordinate { get; set; }
        }
    }
}

using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2023.Day16;

public class Day16 : Day
{
    public Day16() : base(2023, 16, "Day16/input_2023_16.txt", "8098", "8335", false)
    {

    }

    public override void Initialise()
    {
    }

    private Map LoadMap()
    {
        var height = InputLines.Count;
        var map = new Grid2D<Location>(InputLines[0].Length, height);
        foreach (var y in map.YIndexes())
        {
            foreach (var x in map.XIndexes())
            {
                var location = new Location
                {
                    Type = InputLines[(int)(height - 1 - y)][(int)x],
                    EnergizedCount = 0
                };

                map.Write(x, y, location);
            }
        }

        return new Map(map);
    }

    public override string Part1()
    {
        var map = LoadMap();
        var energizedCount = map.GetEnergizedCount(new Coordinate2D(0, map._map.MaxY), Direction.Left);

        return energizedCount.ToString();
    }

    public override string Part2()
    {
        var baseMap = LoadMap()._map;
        var maxEnergizedCount = 0;
        foreach(var y in baseMap.YIndexes())
        {
            var map = LoadMap();
            var energizedCount = map.GetEnergizedCount(new Coordinate2D(0, y), Direction.Left);
            if (energizedCount > maxEnergizedCount)
            {
                TraceLine($"{energizedCount} (0, {y}) Left - New Max");
                maxEnergizedCount = energizedCount;
            }
            else
            {
                TraceLine($"{energizedCount} (0, {y}) Left");
            }
        }

        foreach (var y in baseMap.YIndexes())
        {
            var map = LoadMap();
            var energizedCount = map.GetEnergizedCount(new Coordinate2D(baseMap.MaxX, y), Direction.Right);
            if (energizedCount > maxEnergizedCount)
            {
                TraceLine($"{energizedCount} ({baseMap.MaxX}, {y}) Right - New Max");
                maxEnergizedCount = energizedCount;
            }
            else
            {
                TraceLine($"{energizedCount} ({baseMap.MaxX}, {y}) Right");
            }
        }

        foreach (var x in baseMap.XIndexes())
        {
            var map = LoadMap();
            var energizedCount = map.GetEnergizedCount(new Coordinate2D(x, 0), Direction.Down);
            if (energizedCount > maxEnergizedCount)
            {
                TraceLine($"{energizedCount} ({x}, 0) Down - New Max");
                maxEnergizedCount = energizedCount;
            }
            else
            {
                TraceLine($"{energizedCount} ({x}, 0) Down");
            }
        }

        foreach (var x in baseMap.XIndexes())
        {
            var map = LoadMap();
            var energizedCount = map.GetEnergizedCount(new Coordinate2D(x, baseMap.MaxY), Direction.Up);
            if (energizedCount > maxEnergizedCount)
            {
                TraceLine($"{energizedCount} ({x}, {baseMap.MaxY}) Up - New Max");
                maxEnergizedCount = energizedCount;
            }
            else
            {
                TraceLine($"{energizedCount} ({x}, {baseMap.MaxY}) Up");
            }
        }

        return maxEnergizedCount.ToString();
    }


    private class Map
    {
        public Grid2D<Location> _map;
        private HashSet<string> _followed = new HashSet<string>();

        public Map(Grid2D<Location> map)
        {
            _map = map;
        }

        public int GetEnergizedCount(Coordinate2D start, Direction entersFrom)
        {

            FollowBeam(start, entersFrom);

            var energizedCount = _map.ReadAll().Count(x => x.EnergizedCount > 0);

            return energizedCount;
        }

        private void FollowBeam(Coordinate2D coordinate, Direction entersFrom)
        {
            if (!_map.IsInGrid(coordinate))
            {
                return;
            }

            var description = $"{coordinate} {entersFrom}";
            if (_followed.Contains(description))
            {
                return;
            }
            _followed.Add(description);

            var location = _map.Read(coordinate);
            location.EnergizedCount += 1;

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
}
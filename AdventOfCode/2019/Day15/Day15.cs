using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using AdventOfCode._2019.Intcode;
using AdventOfCode.Shared;

namespace AdventOfCode._2019.Day15
{
    public class Day15 : Day
    {
        public Day15() : base(2019, 15, "Day15/input_2019_15.txt", "366", "384")
        {

        }

        private IntcodeMachine _repairDroid { get; set; }
        private IOPipe _repairDroidInput { get; set; }
        private IOPipe _repairDroidOutput { get; set; }
        private Map _map { get; set; }
        private Point _position = new Point(0, 0);

        private bool _foundOxygen = false;
        private Point _oxygenLocation = new Point(0, 0);
        private List<Direction> _oxygenToHomePath = new List<Direction>();
        private List<Point> _trail = new List<Point>();

        private enum Direction
        {
            North = 1,
            South = 2,
            West = 3,
            East = 4
        }

        private enum StatusCode
        {
            HitWall = 0,
            Moved = 1,
            FoundOxygenSystem = 2
        }

        public override void Initialise()
        {
            _repairDroid = IntcodeMachine.Load(InputLines);
            _repairDroidInput = new IOPipe();
            _repairDroidOutput = new IOPipe();
            _repairDroid.SetInput(_repairDroidInput);
            _repairDroid.SetOutput(_repairDroidOutput);
            _map = new Map();
        }

        public override string Part1()
        {
            return GetShortestRouteToOxygen().ToString();
        }

        public override string Part2()
        {
            return GetTimeToFillWithOxygen().ToString();
        }

        public int GetShortestRouteToOxygen()
        {
            Explore(false).Wait();
            return _oxygenToHomePath.Count;
        }

        public int GetTimeToFillWithOxygen()
        {
            Explore(true).Wait();

            var frontier = new List<Point>()
            {
                _oxygenLocation
            };

            var iterations = 0;
            while (frontier.Any())
            {
                foreach (var location in frontier.ToList())
                {
                    var adjacentLocations = new List<Point>
                    {
                        new Point(location.X, location.Y + 1),
                        new Point(location.X, location.Y - 1),
                        new Point(location.X + 1, location.Y),
                        new Point(location.X - 1, location.Y),
                    };

                    foreach (var locationToCheck in adjacentLocations)
                    {
                        if (_map.GetMapStatus(locationToCheck) == Map.MapStatus.Passable)
                        {
                            _map.MapLocation(locationToCheck, Map.MapStatus.Oxygen);
                            frontier.Add(locationToCheck);
                        }
                    }

                    frontier.Remove(location);
                }

                iterations += 1;
            }

            return iterations - 1;
        }

        private async Task Explore(bool exploreEverywhere)
        {
            _map.MapLocation(_position, Map.MapStatus.Passable);
            _repairDroid.Execute().GetAwaiter().GetResult();
            await ExploreAllDirectionsAndReturn(exploreEverywhere);
            _map.Render(_position, _trail);
        }

        private async Task ExploreAllDirectionsAndReturn(bool exploreEverywhere)
        {
            await Explore(Direction.North, Direction.South, exploreEverywhere);
            await Explore(Direction.East, Direction.West, exploreEverywhere);
            await Explore(Direction.South, Direction.North, exploreEverywhere);
            await Explore(Direction.West, Direction.East, exploreEverywhere);
        }

        private async Task Explore(Direction direction, Direction reverseDirection, bool exploreEverywhere)
        {
            var moved = false;

            if ((!_foundOxygen || exploreEverywhere) && _map.GetMapStatus(GetNewLocation(direction)) == Map.MapStatus.Unmapped)
            {
                moved = await TryMove(direction);
                if (moved)
                {
                    await ExploreAllDirectionsAndReturn(exploreEverywhere);
                }
            }

            if (moved)
            {
                await TryMove(reverseDirection);
                if (_foundOxygen && !exploreEverywhere)
                {
                    _oxygenToHomePath.Add(reverseDirection);
                    _trail.Add(_position);
                }
            }
        }

        private async Task<bool> TryMove(Direction direction)
        {
            _repairDroidInput.Output((int)direction);

            var newLocation = GetNewLocation(direction);
            var result = (StatusCode)await _repairDroidOutput.ReadInput();
            switch (result)
            {
                case StatusCode.HitWall:
                    _map.MapLocation(newLocation, Map.MapStatus.Wall);
                    return false;
                case StatusCode.FoundOxygenSystem:
                    _foundOxygen = true;
                    _oxygenLocation = newLocation;
                    _map.MapLocation(newLocation, Map.MapStatus.Oxygen);
                    _position = newLocation;
                    return true;
                case StatusCode.Moved:
                    _map.MapLocation(newLocation, Map.MapStatus.Passable);
                    _position = newLocation;
                    return true;
            }

            throw new Exception($"Unexpected Status Code {result}");
        }

        private Point GetNewLocation(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return new Point(_position.X, _position.Y + 1);
                case Direction.East:
                    return new Point(_position.X + 1, _position.Y);
                case Direction.South:
                    return new Point(_position.X, _position.Y - 1);
                case Direction.West:
                    return new Point(_position.X - 1, _position.Y);
            }

            throw new Exception($"Unexpected Direction {direction}");
        }

        private class Map
        {
            public enum MapStatus
            {
                Unmapped,
                Wall,
                Passable,
                Oxygen
            }

            private Dictionary<int, Dictionary<int, MapStatus>> _map = new Dictionary<int, Dictionary<int, MapStatus>>();
            public void MapLocation(Point location, MapStatus status)
            {
                if (!_map.ContainsKey(location.X))
                {
                    _map.Add(location.X, new Dictionary<int, MapStatus>());
                }

                _map[location.X][location.Y] = status;
            }

            public MapStatus GetMapStatus(Point location)
            {
                if (!_map.ContainsKey(location.X))
                {
                    return MapStatus.Unmapped;
                }
                if (!_map[location.X].ContainsKey(location.Y))
                {
                    return MapStatus.Unmapped;
                }

                return _map[location.X][location.Y];
            }

            public void Render(Point droidPosition, List<Point> trail)
            {
                var xMin = _map.Keys.Min();
                var xMax = _map.Keys.Max();
                var yMin = _map.SelectMany(x => x.Value.Keys).Min();
                var yMax = _map.SelectMany(x => x.Value.Keys).Max();

                for (var y = yMax; y >= yMin; y--)
                {
                    for (var x = xMin; x <= xMax; x++)
                    {
                        if (x == droidPosition.X && y == droidPosition.Y)
                        {
                            Console.Write("D");
                        }
                        else if (trail.Any(t => t.X == x && t.Y == y))
                        {
                            Console.Write("*");
                        }
                        else
                        {
                            switch (GetMapStatus(new Point(x, y)))
                            {
                                case MapStatus.Oxygen:
                                    Console.Write("o");
                                    break;
                                case MapStatus.Passable:
                                    Console.Write(" ");
                                    break;
                                case MapStatus.Wall:
                                    Console.Write("#");
                                    break;
                                case MapStatus.Unmapped:
                                    Console.Write("?");
                                    break;
                            }
                        }
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}

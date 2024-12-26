using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2024.Day06;

public class Day06 : Day
{
    public Day06() : base(2024, 6, "Day06/input_2024_06.txt", "5516", "2008", true)
    {

    }

    public override void Initialise()
    {
    }

    public override string Part1()
    {
        var lab = new Lab(InputLines);

        (var outcome, var visitHistory) = lab.WalkRoute(lab.IntialGuardPosition, Direction.Up);
        
        return visitHistory.GetLocationVisitCount().ToString();
    }

    public override string Part2()
    {
        // Need to move visited out of lab positions
        // so we can have separate visit list per exploration
        // only returning OffGrid at the moment
        var lab = new Lab(InputLines);

        (var outcome, var visitHistory) = lab.WalkRoute(lab.IntialGuardPosition, Direction.Up);

        var loopCount = lab.GetLoopCount(visitHistory);
        
        return loopCount.ToString();
    }

    private class Position
    {
        public Coordinate2D Location { get; set; }
        public bool IsOccupied { get; set; }
        // public Dictionary<Direction, VisitHistory> VisitHistory { get; set; } = new();

        public Position(char c, int x, int y)
        {
            Location = new Coordinate2D(x, y);
            IsOccupied = c == '#';
            // VisitHistory = null;
        }
    }

    private enum RouteOutcome
    {
        OffGrid,
        Loop
    }

    private class VisitHistory
    {
        public VisitHistory()
        {
            
        }

        private bool _hasBeenVisited = false;
        private Coordinate2D _location;
        private Direction _direction;
        private VisitHistory _previousHistory;

        private VisitHistory(Coordinate2D location, Direction direction, VisitHistory visitHistory)
        {
            _hasBeenVisited = true;
            _location = location;
            _direction = direction;
            _previousHistory = visitHistory;
        }

        public int GetLocationVisitCount()
        {
            var allVisitedLocations = GetAllVisitedLocations().ToList();

            return allVisitedLocations.Select(x => x.Location).Distinct().Count();
        }

        public IEnumerable<(Coordinate2D Location, Direction Direction)> GetAllVisitedLocations()
        {
            if (!_hasBeenVisited)
            {
                yield break;
            }

            yield return (_location, _direction);

            foreach (var previousLocation in _previousHistory.GetAllVisitedLocations())
            {
                yield return previousLocation;
            }
        }

        public bool HasBeenVisited(Coordinate2D location)
        {
            if (!_hasBeenVisited)
            {
                return false;
            }

            if (location.Equals(_location))
            {
                return true;
            }

            return _previousHistory.HasBeenVisited(location);
        }

        public bool HasBeenVisited(Coordinate2D location, Direction direction)
        {
            if (!_hasBeenVisited)
            {
                return false;
            }

            if (location.Equals(_location) && direction == _direction)
            {
                return true;
            }

            return _previousHistory.HasBeenVisited(location, direction);
        }

        public VisitHistory HistoryFor(Coordinate2D location, Direction direction)
        {
            if (!_hasBeenVisited)
            {
                throw new Exception($"No history for {location} {direction}");
            }

            if (location.Equals(_location) && direction == _direction)
            {
                return this;
            }

            return _previousHistory.HistoryFor(location, direction);
        }

        public VisitHistory RecordVisit(Coordinate2D location, Direction direction)
        {
            return new VisitHistory(location, direction, this);
        }
    }

    private class VisitKey
    {
        public Coordinate2D Location { get; set; }
        public Direction Direction { get; set; }

        public override string ToString()
        {
            return $"{Location}_{Direction}";
        }

        public override bool Equals(object obj)
        {
            var visitKey = obj as VisitKey;
            if (obj == null && visitKey == null)
            {
                return true;
            }

            if (obj == null || visitKey == null)
            {
                return false;
            }

            return ToString().Equals(visitKey.ToString());
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
    private class Lab
    {
        public Coordinate2D IntialGuardPosition;

        private Grid2D<Position> _map;

        public Lab(List<string> inputLines)
        {
            _map = new Grid2D<Position>(inputLines[0].Length, inputLines.Count);
            var y = inputLines.Count - 1;
            foreach (var line in inputLines)
            {
                var x = 0;
                foreach (var c in line)
                {
                    _map.Write(x, y, new Position(c, x, y));
                    if (c == '^')
                    {
                        IntialGuardPosition = new Coordinate2D(x, y);
                    }

                    x += 1;
                }
                y -= 1;
            }
        }

        public (RouteOutcome RouteOutcome, VisitHistory VisitHistory) WalkRoute(
            Coordinate2D guardPosition,
            Direction guardDirection,
            VisitHistory visitHistory = null)
        {
            visitHistory ??= new VisitHistory();

            if (visitHistory.HasBeenVisited(guardPosition, guardDirection))
            {
                return (RouteOutcome.Loop, visitHistory);
            }

            var thisLocation = _map.Read(guardPosition);
            visitHistory = visitHistory.RecordVisit(guardPosition, guardDirection);
            //thisLocation.VisitHistory.Add(guardDirection, visitHistory);

            var nextPosition = guardPosition.Neighbour(guardDirection);
            if (!_map.IsInGrid(nextPosition))
            {
                return (RouteOutcome.OffGrid, visitHistory);
            }

            var nextLocation = _map.Read(nextPosition);
            if (nextLocation.IsOccupied)
            {
                var newGuardDirection = RightTurn(guardDirection);

                return WalkRoute(guardPosition, newGuardDirection, visitHistory);
                // Console.WriteLine($"Turned to face {_guardDirection}");
            }

            return WalkRoute(nextPosition, guardDirection, visitHistory);
        }

        private VisitKey VisitKey(Coordinate2D location, Direction direction)
        {
            return new VisitKey
            {
                Location = location,
                Direction = direction
            };
        }

        private Direction RightTurn(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up: return Direction.Right;
                case Direction.Right: return Direction.Down;
                case Direction.Down: return Direction.Left;
                case Direction.Left: return Direction.Up;
            }

            throw new Exception("Unknown direction");
        }

        public int GetLoopCount(VisitHistory visitHistory)
        {
            var loopCount = 0;

            var visitedLocations = visitHistory
                .GetAllVisitedLocations()
                .ToList();

            var locationDirections = visitedLocations
                .GroupBy(x => x.Location)
                .ToList();

            foreach (var locationDirection in locationDirections)
            //var locationDirection = (new Coordinate2D(4, 3), Direction.Left)
            {
                
                var visited = locationDirection.Key;

                var visitedDirections = locationDirection
                    .Select(ovl => ovl.Direction)
                    .ToList();
                /*
                var visited = new Coordinate2D(4, 3);
                var visitedDirections = new [] { Direction.Left };
    */
                // Console.WriteLine($"Checking Visited Location {visited}");
                foreach (var visitDirection in visitedDirections)
                {
                    var history = visitHistory.HistoryFor(visited, visitDirection);
                    // Console.WriteLine($"- Checking Direction {visitDirection}");
                    var potentialBlockPositionLocation = visited.Neighbour(visitDirection);

                    // Console.WriteLine($"  - Potential block {potentialBlockPositionLocation}");
                    if (!_map.IsInGrid(potentialBlockPositionLocation))
                    {
                        // Console.WriteLine($"  - Off grid {potentialBlockPositionLocation}");
                        continue;
                    }

                    var potentialBlockPosition = _map.Read(potentialBlockPositionLocation);
                    if (potentialBlockPosition.IsOccupied)
                    {
                        // Console.WriteLine($"  - Already blocked {potentialBlockPositionLocation}");
                        continue;
                    }

                    var potentialLoopDirection = RightTurn(visitDirection);
                    (var potentialOutcome, var potentialHistory) = WalkRoute(visited, potentialLoopDirection, history);
                    // Console.WriteLine($"  - Outcome {potentialOutcome}");
                    if (potentialOutcome == RouteOutcome.Loop)
                    {
                        // Console.WriteLine($"  - Created Loop by blocking {potentialBlockPositionLocation}");
                        loopCount += 1;
                    }
                }
            }

            return loopCount;
        }
    }
}
using System.Data;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2024.Day06;

public class Day06 : Day
{
    public Day06() : base(2024, 6, "Day06/input_2024_06.txt", "5516", "", true)
    {

    }

    public override void Initialise()
    {
    }

    public override string Part1()
    {
        var lab = new Lab(InputLines);
        
        return lab.GetVisitedCount().ToString();
    }

    public override string Part2()
    {
        var loopCount = 0;
        var y = InputLines.Count - 1;
        foreach (var line in InputLines)
        {
            TraceLine($"Checking line {y}");
            var x = 0;
            foreach (var c in line)
            {
                var lab = new Lab(InputLines);
                if (lab.TryBlock(x, y))
                {
                    var outcome = lab.GetRouteOutcome();
                    if (outcome == RouteOutcome.Loop)
                    {
                        // TraceLine($"{x},{y} loops");
                        loopCount += 1;
                    }
                    else
                    {
                        // TraceLine($"{x},{y} does not loop");
                    }
                }

                x += 1;
            }
            y -= 1;
        }

        return loopCount.ToString();
    }

    private class Position
    {
        public char InitialValue { get; set; }
        public bool IsOccupied { get; set; }
        public bool Visited { get; set; }
        public List<Direction> VisitDirections = new List<Direction>();
        public Position(char c)
        {
            InitialValue = c;
            IsOccupied = c == '#';
            Visited = c == '^';
        }
    }

    private enum RouteOutcome
    {
        OnPath,
        OffGrid,
        Loop
    }

    private class Lab
    {
        private Grid2D<Position> _map;
        private Coordinate2D _guardPosition;
        private Direction _guardDirection = Direction.Up;

        public Lab(List<string> inputLines)
        {
            _map = new Grid2D<Position>(inputLines[0].Length, inputLines.Count);
            var y = inputLines.Count - 1;
            foreach (var line in inputLines)
            {
                var x = 0;
                foreach (var c in line)
                {
                    _map.Write(x, y, new Position(c));
                    if (c == '^')
                    {
                        _guardPosition = new Coordinate2D(x, y);
                    }

                    x += 1;
                }
                y -= 1;
            }
        }

        private RouteOutcome MoveNext()
        {
            var nextPosition = _guardPosition.Neighbour(_guardDirection);
            if (!_map.IsInGrid(nextPosition))
            {
                // Console.WriteLine($"Gone off grid");
                return RouteOutcome.OffGrid;
            }

            var nextLocation = _map.Read(nextPosition);
            if (nextLocation.IsOccupied)
            {
                TurnRight();
                // Console.WriteLine($"Turned to face {_guardDirection}");
                return RouteOutcome.OnPath;
            }

            _guardPosition = nextPosition;

            if (nextLocation.Visited
             && nextLocation.VisitDirections.Contains(_guardDirection))
            {
                return RouteOutcome.Loop;
            }

            nextLocation.Visited = true;
            nextLocation.VisitDirections.Add(_guardDirection);

            // Console.WriteLine($"Now at {nextPosition}");
            return RouteOutcome.OnPath;
        }

        private void TurnRight()
        {
            switch (_guardDirection)
            {
                case Direction.Up: _guardDirection = Direction.Right; break;
                case Direction.Right: _guardDirection = Direction.Down; break;
                case Direction.Down: _guardDirection = Direction.Left; break;
                case Direction.Left: _guardDirection = Direction.Up; break;
            }
        }

        public RouteOutcome GetRouteOutcome()
        {
            while (true)
            {
                var outcome = MoveNext();
                if (outcome != RouteOutcome.OnPath)
                {
                    return outcome;
                }
            }
        }

        public int GetVisitedCount()
        {
            while (MoveNext() == RouteOutcome.OnPath);

            var visitedCount = _map.ReadAll().Count(x => x.Visited);

            return visitedCount;
        }

        public bool TryBlock(int x, int y)
        {
            var position = _map.Read(x, y);
            if (position.IsOccupied)
            {
                return false;
            }

            if (_guardPosition.X == x && _guardPosition.Y == y)
            {
                return false;
            }

            position.IsOccupied = true;
            return true;
        }
    }
}
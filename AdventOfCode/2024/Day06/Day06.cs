using System.Data;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2024.Day06;

public class Day06 : Day
{
    public Day06() : base(2024, 6, "Day06/input_2024_06.txt", "5516", "", true)
    {

    }

    private Grid2D<Position> _map;
    private Coordinate2D _guardPosition;
    private Direction _guardDirection = Direction.Up;
    public override void Initialise()
    {
        _map = new Grid2D<Position>(InputLines[0].Length, InputLines.Count);
        var y = InputLines.Count - 1;
        foreach (var line in InputLines)
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

    private bool MoveNext()
    {
        var nextPosition = _guardPosition.Neighbour(_guardDirection);
        if (!_map.IsInGrid(nextPosition))
        {
            TraceLine($"Gone off grid");
            return false;
        }

        var nextLocation = _map.Read(nextPosition);
        if (nextLocation.IsOccupied)
        {
            TurnRight();
            TraceLine($"Turned to face {_guardDirection}");
            return true;
        }

        _guardPosition = nextPosition;
        nextLocation.Visited = true;

        TraceLine($"Now at {nextPosition}");
        return true;
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

    public override string Part1()
    {
        while (MoveNext());

        var visitedCount = _map.ReadAll().Count(x => x.Visited);

        return visitedCount.ToString();
    }

    public override string Part2()
    {
        return string.Empty;
    }

    private class Position
    {
        public char InitialValue { get; set; }
        public bool IsOccupied { get; set; }
        public bool Visited { get; set; }
        public Position(char c)
        {
            InitialValue = c;
            IsOccupied = c == '#';
            Visited = c == '^';
        }
    }
}
using System;

namespace AdventOfCode.Shared.Geometry;

public static class DirectionExtensions
{
    public static Direction TurnRight(this Direction direction)
    {
        switch (direction)
        {
            case Direction.Left: return Direction.Up;
            case Direction.Up: return Direction.Right;
            case Direction.Right: return Direction.Down;
            case Direction.Down: return Direction.Left;
        }

        throw new Exception($"Unknown direction {direction}");
    }

    public static Direction TurnLeft(this Direction direction)
    {
        switch (direction)
        {
            case Direction.Down: return Direction.Right;
            case Direction.Right: return Direction.Up;
            case Direction.Up: return Direction.Left;
            case Direction.Left: return Direction.Down;
        }

        throw new Exception($"Unknown direction {direction}");
    }
}

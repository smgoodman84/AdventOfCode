using System;

namespace AdventOfCode.Shared.Geometry;

public class LineSegment
{
    public LineSegment(Coordinate2D start, Coordinate2D end)
    {
        Start = start;
        End = end;
    }

    public Coordinate2D Start { get; }
    public Coordinate2D End { get; }

    public override string ToString()
    {
        return $"{Start} -> {End}";
    }

    public bool IsVertical => Start.X == End.X;
    public bool IsHorizontal => Start.Y == End.Y;

    public bool IsOnLineSegment(Coordinate2D coordinate)
    {
        if (Start.X == End.X)
        {
            var minY = Math.Min(Start.Y, End.Y);
            var maxY = Math.Max(Start.Y, End.Y);
            return coordinate.X == Start.X
                   && coordinate.Y >= minY
                   && coordinate.Y <= maxY;
        }
        
        if (Start.Y == End.Y)
        {
            var minX = Math.Min(Start.X, End.X);
            var maxX = Math.Max(Start.X, End.X);
            return coordinate.Y == Start.Y
                   && coordinate.X >= minX
                   && coordinate.X <= maxX;
        }

        // TODO: Handle non vertical/horizontal lines
        
        return false;
    }
}
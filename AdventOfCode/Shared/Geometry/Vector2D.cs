namespace AdventOfCode.Shared.Geometry;

public class Vector2D
{
    public long X { get; }
    public long Y { get; }

    public Vector2D(long x, long y)
    {
        X = x;
        Y = y;
    }

    public override bool Equals(object obj)
    {
        var coordinate = obj as Coordinate2D;
        if (coordinate == null)
        {
            return false;
        }

        return coordinate.X == X && coordinate.Y == Y;
    }

    public override string ToString()
    {
        return $"{X},{Y}";
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }
}
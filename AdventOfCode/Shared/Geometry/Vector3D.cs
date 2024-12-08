namespace AdventOfCode.Shared.Geometry;

public class Vector3D
{
    public long X { get; }
    public long Y { get; }
    public long Z { get; }

    public Vector3D(long x, long y, long z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Vector3D AddX(long x)
    {
        return new Vector3D(
            X + x,
            Y,
            Z);
    }

    public Vector3D AddY(long y)
    {
        return new Vector3D(
            X,
            Y + y,
            Z);
    }

    public Vector3D AddZ(long z)
    {
        return new Vector3D(
            X,
            Y,
            Z + z);
    }

    public override bool Equals(object obj)
    {
        var vector = obj as Vector3D;
        if (vector == null)
        {
            return false;
        }

        return vector.X == X && vector.Y == Y && vector.Z == Z;
    }

    public override string ToString()
    {
        return $"{X},{Y},{Z}";
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }
}
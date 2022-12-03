using System;

namespace AdventOfCode.Shared.Geometry
{
    public class Coordinate3D
    {
        public long X { get; }
        public long Y { get; }
        public long Z { get; }

        public Coordinate3D(long x, long y, long z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Coordinate3D Add(Coordinate3D coordinate)
        {
            return new Coordinate3D(
                X + coordinate.X,
                Y + coordinate.Y,
                Z + coordinate.Z);
        }

        public Coordinate3D AddX(long x)
        {
            return new Coordinate3D(
                X + x,
                Y,
                Z);
        }

        public Coordinate3D AddY(long y)
        {
            return new Coordinate3D(
                X,
                Y + y,
                Z);
        }

        public Coordinate3D AddZ(long z)
        {
            return new Coordinate3D(
                X,
                Y,
                Z + z);
        }

        public override string ToString()
        {
            return $"{X},{Y},{Z}";
        }

        public static readonly Coordinate3D Origin = new Coordinate3D(0, 0, 0);
    }
}

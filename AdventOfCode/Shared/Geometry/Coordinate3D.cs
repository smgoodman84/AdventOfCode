using System.Collections.Generic;

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

        public Coordinate3D(string coordinate)
        {
            var split = coordinate.Split(",", System.StringSplitOptions.TrimEntries);
            X = long.Parse(split[0]);
            Y = long.Parse(split[1]);
            Z = long.Parse(split[2]);
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


        public Coordinate3D Up() => AddY(1);
        public Coordinate3D Down() => AddY(-1);
        public Coordinate3D Left() => AddX(-1);
        public Coordinate3D Right() => AddX(1);
        public Coordinate3D Forward() => AddZ(1);
        public Coordinate3D Backward() => AddZ(-1);


        public IEnumerable<Coordinate3D> Neighbours()
        {
            yield return Up();
            yield return Down();
            yield return Left();
            yield return Right();
            yield return Forward();
            yield return Backward();
        }

        public override string ToString()
        {
            return $"{X},{Y},{Z}";
        }

        public static readonly Coordinate3D Origin = new Coordinate3D(0, 0, 0);
    }
}

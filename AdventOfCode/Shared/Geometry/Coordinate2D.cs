namespace AdventOfCode.Shared.Geometry
{
    public class Coordinate2D
    {
        public long X { get; }
        public long Y { get; }

        public Coordinate2D(long x, long y)
        {
            X = x;
            Y = y;
        }

        public Coordinate2D Add(Coordinate2D coordinate)
        {
            return new Coordinate2D(X + coordinate.X, Y + coordinate.Y);
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }

        public static readonly Coordinate2D Origin = new Coordinate2D(0, 0);
    }
}

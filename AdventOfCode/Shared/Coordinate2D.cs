namespace AdventOfCode.Shared
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

        public override string ToString()
        {
            return $"{X},{Y}";
        }

        public static Coordinate2D Origin = new Coordinate2D(0, 0);
    }
}

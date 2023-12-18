namespace AdventOfCode.Shared.Geometry
{
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
    }
}


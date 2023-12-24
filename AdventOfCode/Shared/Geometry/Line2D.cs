namespace AdventOfCode.Shared.Geometry
{
    public class Line2D
    {
        public Line2D(Coordinate2D point1, Coordinate2D point2)
        {
            Point1 = point1;
            Point2 = point2;
        }

        public Coordinate2D Point1 { get; }
        public Coordinate2D Point2 { get; }

        public bool TryGetIntersection(Line2D that, out double x, out double y)
        {
            /*
            y = m1 * x + c1
            y = m2 * x + c2
            m1 * x + c1 = m2 * x + c2
            m1 * x = m2 * x + c2 - c1
            m1 * x - m2 * x = c2 - c1
            (m1 - m2) * x = c2 - c1
            x = (c2 - c1) / (m1 - m2)
            */

            if (GetGradient() == that.GetGradient())
            {
                x = 0;
                y = 0;
                return false;
            }

            x = (that.GetC() - GetC()) / (GetGradient() - that.GetGradient());
            y = GetGradient() * x + GetC();
            return true;
        }

        private double? _gradient;
        public double GetGradient()
        {
            if (!_gradient.HasValue)
            {
                _gradient = ((double)Point2.Y - (double)Point1.Y)
                    / ((double)Point2.X - (double)Point1.X);
            }

            return _gradient.Value;
        }

        private double? _c;
        public double GetC()
        {
            if (!_c.HasValue)
            {
                _c = Point1.Y - (GetGradient() * Point1.X);
            }

            return _c.Value;
        }

        public override string ToString()
        {
            return $"{Point1} -> {Point2}";
        }
    }
}


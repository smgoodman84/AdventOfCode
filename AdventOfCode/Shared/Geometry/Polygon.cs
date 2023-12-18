using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Shared.Geometry
{
    public class Polygon
    {
        public Polygon(IEnumerable<LineSegment> lineSegments)
        {
            LineSegments = lineSegments.ToList();
        }

        public List<LineSegment> LineSegments { get; }

        public double CalculateArea()
        {
            // https://www.geeksforgeeks.org/area-of-a-polygon-with-given-n-ordered-vertices
            double area = 0.0;

            var coordinates = LineSegments.Select(x => x.Start).ToList();

            // Calculate value of shoelace formula
            var previous = coordinates.Last();
            foreach(var current in coordinates)
            {
                area += (previous.X + current.X) * (previous.Y - current.Y);
                previous = current;
            }

            return Math.Abs(area / 2.0);
        }
    }
}


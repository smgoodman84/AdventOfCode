using System;

namespace AdventOfCode2019.Day12
{
    public class Point3D
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public void Add(Point3D point)
        {
            X += point.X;
            Y += point.Y;
            Z += point.Z;
        }

        public int Energy()
        {
            return Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
        }
    }
}

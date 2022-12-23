﻿using System;
using System.Collections.Generic;

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

        public Coordinate2D(string coordinate)
        {
            var split = coordinate.Split(",");
            X = long.Parse(split[0]);
            Y = long.Parse(split[1]);
        }

        public Coordinate2D Add(Coordinate2D coordinate)
        {
            return new Coordinate2D(X + coordinate.X, Y + coordinate.Y);
        }

        public long ManhattanDistanceTo(Coordinate2D coordinate)
        {
            return Math.Abs(coordinate.X - X)
                + Math.Abs(coordinate.Y - Y);
        }

        public IEnumerable<Coordinate2D> Neighbours()
        {
            yield return Up();
            yield return Down();
            yield return Left();
            yield return Right();
        }

        public Coordinate2D Neighbour(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return Up();
                case Direction.Down:
                    return Down();
                case Direction.Left:
                    return Left();
                case Direction.Right:
                    return Right();
            }

            throw new Exception($"Unsupported Direction {direction}");
        }

        public Coordinate2D NorthEast() => North().East();
        public Coordinate2D NorthWest() => North().West();
        public Coordinate2D North() => Up();
        public Coordinate2D Up()
        {
            return new Coordinate2D(X, Y + 1);
        }


        public Coordinate2D SouthEast() => South().East();
        public Coordinate2D SouthWest() => South().West();
        public Coordinate2D South() => Down();
        public Coordinate2D Down()
        {
            return new Coordinate2D(X, Y - 1);
        }

        public Coordinate2D West() => Left();
        public Coordinate2D Left()
        {
            return new Coordinate2D(X - 1, Y);
        }

        public Coordinate2D East() => Right();
        public Coordinate2D Right()
        {
            return new Coordinate2D(X + 1, Y);
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }

        public static readonly Coordinate2D Origin = new Coordinate2D(0, 0);
    }
}

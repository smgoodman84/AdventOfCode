﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Shared.Geometry;

public class Coordinate2D
{
    public long X { get; }
    public long Y { get; }

    public CoordinateSystem CoordinateSystem { get;}

    public Coordinate2D(
        long x,
        long y,
        CoordinateSystem coordinateSystem = CoordinateSystem.Cartesian)
    {
        X = x;
        Y = y;
        CoordinateSystem = coordinateSystem;
    }

    public Coordinate2D(
        string coordinate,
        CoordinateSystem coordinateSystem = CoordinateSystem.Cartesian)
    {
        var split = coordinate.Split(",");
        X = long.Parse(split[0]);
        Y = long.Parse(split[1]);
        CoordinateSystem = coordinateSystem;
    }

    public Vector2D Subtract(Coordinate2D coordinate)
    {
        return new Vector2D(X - coordinate.X, Y - coordinate.Y);
    }

    public Coordinate2D Add(Vector2D vector)
    {
        return new Coordinate2D(X + vector.X, Y + vector.Y, CoordinateSystem);
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

    public IEnumerable<Coordinate2D> AllNeighbours()
    {
        yield return Up().Left();
        yield return Up();
        yield return Up().Right();
        yield return Left();
        yield return Right();
        yield return Down().Left();
        yield return Down();
        yield return Down().Right();
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
        if (CoordinateSystem == CoordinateSystem.Screen)
        {
            return new Coordinate2D(X, Y - 1, CoordinateSystem);
        }

        return new Coordinate2D(X, Y + 1, CoordinateSystem);
    }


    public Coordinate2D SouthEast() => South().East();
    public Coordinate2D SouthWest() => South().West();
    public Coordinate2D South() => Down();
    public Coordinate2D Down()
    {
        if (CoordinateSystem == CoordinateSystem.Screen)
        {
            return new Coordinate2D(X, Y + 1, CoordinateSystem);
        }
        
        return new Coordinate2D(X, Y - 1, CoordinateSystem);
    }

    public Coordinate2D West() => Left();
    public Coordinate2D Left()
    {
        return new Coordinate2D(X - 1, Y, CoordinateSystem);
    }

    public Coordinate2D East() => Right();
    public Coordinate2D Right()
    {
        return new Coordinate2D(X + 1, Y, CoordinateSystem);
    }

    public override string ToString()
    {
        return $"{X},{Y}";
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    public static readonly Coordinate2D CartesianOrigin = new Coordinate2D(0, 0);
    public static readonly Coordinate2D ScreenOrigin = new Coordinate2D(0, 0);

    public static IEnumerable<Coordinate2D> XRange(
        int xStart,
        int xEnd,
        long y,
        CoordinateSystem coordinateSystem = CoordinateSystem.Cartesian)
    {
        if (xEnd < xStart)
        {
            return XRange(xEnd, xStart, y).OrderByDescending(c => c.X);
        }

        return Enumerable.Range(xStart, xEnd - xStart + 1)
            .Select(x => new Coordinate2D(x, y, coordinateSystem));
    }

    public static IEnumerable<Coordinate2D> YRange(
        int yStart,
        int yEnd,
        long x,
        CoordinateSystem coordinateSystem = CoordinateSystem.Cartesian)
    {
        if (yEnd < yStart)
        {
            return YRange(yEnd, yStart, x).OrderByDescending(c => c.Y);
        }

        return Enumerable.Range(yStart, yEnd - yStart + 1)
            .Select(y => new Coordinate2D(x, y, coordinateSystem));
    }

    public override bool Equals(object obj)
    {
        var coordinate = obj as Coordinate2D;
        if (coordinate == null)
        {
            return false;
        }

        return coordinate.X == X
            && coordinate.Y == Y
            && coordinate.CoordinateSystem == CoordinateSystem;
    }
}
using System;
using System.Collections.Generic;

namespace AdventOfCode.Shared.Geometry;

public class Grid2D<T> : IGrid2D<T>
{
    private T[,] _grid;

    public long Width { get; }
    public long Height { get; }

    public long MinX { get; }
    public long MaxX { get; }
    public long MinY { get; }
    public long MaxY { get; }

    public Grid2D(int width, int height)
        : this(0, 0, width - 1, height - 1)
    {
    }

    public Grid2D(int minX, int minY, int maxX, int maxY)
    {
        var height = maxY - minY + 1;
        var width = maxX - minX + 1;

        MinX = minX;
        MinY = minY;
        MaxX = maxX;
        MaxY = maxY;

        _grid = new T[width, height];
        Width = width;
        Height = height;
    }

    public static Grid2D<T> CreateWithCartesianCoordinates(
        List<string> inputLines,
        Func<Coordinate2D, char, T> elementMapper)
    {
        var height = inputLines.Count;
        var width = inputLines[0].Length;

        var grid = new Grid2D<T>(width, height);
        
        var y = height - 1;
        foreach (var line in inputLines)
        {
            var x = 0;
            foreach (var c in line)
            {
                var coordinate = new Coordinate2D(x, y);
                var element = elementMapper(coordinate, c);
                grid.Write(coordinate, element);

                x += 1;
            }
            y -= 1;
        }

        return grid;
    }

    public bool IsInGrid(long x, long y) => IsInGrid((int)x, (int)y);
    public bool IsInGrid(int x, int y)
    {
        return MinX <= x && x <= MaxX
                         && MinY <= y && y <= MaxY;
    }

    public bool IsInGrid(Coordinate2D coordinate)
    {
        return IsInGrid((int)coordinate.X, (int)coordinate.Y);
    }

    public T Read(long x, long y) => Read((int)x, (int)y);
    public T Read(int x, int y)
    {
        return _grid[x - MinX, y - MinY];
    }

    public T Read(Coordinate2D coordinate)
    {
        return Read((int)coordinate.X, (int)coordinate.Y);
    }

    public void Write(long x, long y, T value) => Write((int)x, (int)y, value);
    public void Write(int x, int y, T value)
    {
        _grid[x - MinX, y - MinY] = value;
    }

    public void Write(Coordinate2D coordinate, T value)
    {
        Write((int)coordinate.X, (int)coordinate.Y, value);
    }

    public IEnumerable<T> ReadAll()
    {
        foreach (var y in YIndexes())
        {
            foreach (var x in XIndexes())
            {
                yield return Read(x, y);
            }
        }
    }

    public IEnumerable<T> ReadRow(long y)
    {
        foreach (var x in XIndexes())
        {
            yield return Read(x, y);
        }
    }

    public IEnumerable<T> ReadColumn(long x)
    {
        foreach (var y in YIndexes())
        {
            yield return Read(x, y);
        }
    }

    public IEnumerable<long> YIndexes()
    {
        for (var y = MinY; y <= MaxY; y++)
        {
            yield return y;
        }
    }

    public IEnumerable<long> XIndexes()
    {
        for (var x = MinX; x <= MaxX; x++)
        {
            yield return x;
        }
    }

    public IEnumerable<Coordinate2D> AllCoordinates() => FindCoordinates(_ => true);
    public IEnumerable<Coordinate2D> FindCoordinates(Func<T, bool> predicate)
    {
        foreach (var y in YIndexes())
        {
            foreach (var x in XIndexes())
            {
                if (predicate(Read(x, y)))
                {
                    yield return new Coordinate2D(x, y);
                }
            }
        }
    }

    public Grid2D<T> Clone()
    {
        var clone = new Grid2D<T>((int)Width, (int)Height);

        foreach (var y in YIndexes())
        {
            foreach (var x in XIndexes())
            {
                clone.Write(x, y, Read(x,y));
            }
        }

        return clone;
    }
}
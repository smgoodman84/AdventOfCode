using System;
using System.Collections.Generic;

namespace AdventOfCode.Shared.Geometry
{
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
            for (var y = MinY; y <= MaxY; y++)
            {
                for (var x = MinX; x <= MaxX; x++)
                {
                    yield return Read(x, y);
                }
            }
        }

        public IEnumerable<Coordinate2D> FindCoordinates(Func<T, bool> predicate)
        {
            for (var y = MinY; y <= MaxY; y++)
            {
                for (var x = MinX; x <= MaxX; x++)
                {
                    if (predicate(Read(x, y)))
                    {
                        yield return new Coordinate2D(x, y);
                    }
                }
            }
        }
    }
}

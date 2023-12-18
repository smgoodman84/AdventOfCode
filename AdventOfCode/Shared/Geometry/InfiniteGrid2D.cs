using System;
using System.Collections.Generic;
using AdventOfCode.Shared.DataStructures;

namespace AdventOfCode.Shared.Geometry
{
    public class InfiniteGrid2D<T> : IGrid2D<T>
    {
        private Dictionary2D<long, long, T> _grid;

        public long Width { get; private set; }
        public long Height { get; private set; }

        public long MinX { get; private set; }
        public long MaxX { get; private set; }
        public long MinY { get; private set; }
        public long MaxY { get; private set; }

        private bool _any;
        private T _defaultValue;

        public InfiniteGrid2D(T defaultValue)
        {
            _grid = new Dictionary2D<long, long, T>();
            _defaultValue = defaultValue;
        }

        public bool IsInGrid(int x, int y) => IsInGrid((long)x, (long)y);
        public bool IsInGrid(long x, long y)
        {
            return MinX <= x && x <= MaxX
                && MinY <= y && y <= MaxY;
        }

        public bool IsInGrid(Coordinate2D coordinate)
        {
            return IsInGrid(coordinate.X, coordinate.Y);
        }

        public T Read(int x, int y) => Read((long)x, (long)y);
        public T Read(long x, long y)
        {
            return _grid.ContainsKey(x,y) ? _grid[x][y] : _defaultValue;
        }

        public T Read(Coordinate2D coordinate)
        {
            return Read(coordinate.X, coordinate.Y);
        }

        public void Write(int x, int y, T value) => Write((long)x, (long)y, value);
        public void Write(long x, long y, T value)
        {
            if (!_any || x < MinX)
            {
                MinX = x;
                Width = MaxX - MinX + 1;
            }
            if (!_any || x > MaxX)
            {
                MaxX = x;
                Width = MaxX - MinX + 1;
            }
            if (!_any || y < MinY)
            {
                MinY = y;
                Height = MaxY - MinY + 1;
            }
            if (!_any || y > MaxY)
            {
                MaxY = y;
                Height = MaxY - MinY + 1;
            }

            _any = true;
            _grid.Set(x, y, value);
        }

        public void Write(Coordinate2D coordinate, T value)
        {
            Write(coordinate.X, coordinate.Y, value);
        }

        public IEnumerable<Coordinate2D> FindCoordinates(Func<T, bool> predicate)
        {
            foreach(var x in _grid.Keys)
            {
                foreach(var y in _grid[x].Keys)
                {
                    if (predicate(_grid[x][y]))
                    {
                        yield return new Coordinate2D(x, y);
                    }
                }
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

        public IEnumerable<T> ReadAll()
        {
            for (var y = MinY; y < Height; y++)
            {
                for (var x = MinX; x < Width; x++)
                {
                    yield return Read(x, y);
                }
            }
        }
    }
}

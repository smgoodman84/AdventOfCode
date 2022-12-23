using System;
using System.Collections.Generic;
using AdventOfCode.Shared.DataStructures;

namespace AdventOfCode.Shared.Geometry
{
    public class InfiniteGrid2D<T>
    {
        private Dictionary2D<long, long, T> _grid;

        public long Width { get; private set; }
        public long Height { get; private set; }

        private long _minX;
        private long _maxX;
        private long _minY;
        private long _maxY;
        private bool _any;
        private T _defaultValue;

        public InfiniteGrid2D(T defaultValue)
        {
            _grid = new Dictionary2D<long, long, T>();
            _defaultValue = defaultValue;
        }

        public bool IsInGrid(long x, long y)
        {
            return _minX <= x && x <= _maxX
                && _minY <= y && y <= _maxY;
        }

        public bool IsInGrid(Coordinate2D coordinate)
        {
            return IsInGrid(coordinate.X, coordinate.Y);
        }

        public T Read(long x, long y)
        {
            return _grid.ContainsKey(x,y) ? _grid[x][y] : _defaultValue;
        }

        public T Read(Coordinate2D coordinate)
        {
            return Read(coordinate.X, coordinate.Y);
        }

        public void Write(long x, long y, T value)
        {
            if (!_any || x < _minX)
            {
                _minX = x;
                Width = _maxX - _minX + 1;
            }
            if (!_any || x > _maxX)
            {
                _maxX = x;
                Width = _maxX - _minX + 1;
            }
            if (!_any || y < _minY)
            {
                _minY = y;
                Height = _maxY - _minY + 1;
            }
            if (!_any || y > _maxY)
            {
                _maxY = y;
                Height = _maxY - _minY + 1;
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

        public IEnumerable<T> ReadAll()
        {
            for (var y = _minY; y < Height; y++)
            {
                for (var x = _minX; x < Width; x++)
                {
                    yield return Read(x, y);
                }
            }
        }
    }
}

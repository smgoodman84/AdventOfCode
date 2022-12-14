using System.Collections.Generic;

namespace AdventOfCode.Shared.Geometry
{
    public class Grid2D<T>
    {
        private T[,] _grid;

        public int Width { get; }
        public int Height { get; }

        private int _minX;
        private int _maxX;
        private int _minY;
        private int _maxY;

        public Grid2D(int width, int height)
            : this(0, 0, width - 1, height - 1)
        {
        }

        public Grid2D(int minX, int minY, int maxX, int maxY)
        {
            var height = maxY - minY + 1;
            var width = maxX - minX + 1;

            _minX = minX;
            _minY = minY;
            _maxX = maxX;
            _maxY = maxY;

            _grid = new T[width, height];
            Width = width;
            Height = height;
        }

        public bool IsInGrid(int x, int y)
        {
            return _minX <= x && x <= _maxX
                && _minY <= y && y <= _maxY;
        }

        public bool IsInGrid(Coordinate2D coordinate)
        {
            return IsInGrid((int)coordinate.X, (int)coordinate.Y);
        }

        public T Read(int x, int y)
        {
            return _grid[x - _minX, y - _minY];
        }

        public T Read(Coordinate2D coordinate)
        {
            return Read((int)coordinate.X, (int)coordinate.Y);
        }

        public void Write(int x, int y, T value)
        {
            _grid[x - _minX, y - _minY] = value;
        }

        public void Write(Coordinate2D coordinate, T value)
        {
            Write((int)coordinate.X, (int)coordinate.Y, value);
        }

        public IEnumerable<T> ReadAll()
        {
            for(var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    yield return Read(x, y);
                }
            }
        }
    }
}

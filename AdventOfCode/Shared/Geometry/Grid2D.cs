using System.Collections.Generic;

namespace AdventOfCode.Shared.Geometry
{
    public class Grid2D<T>
    {
        private T[,] _grid;

        public int Width { get; }
        public int Height { get; }

        public Grid2D(int width, int height)
        {
            _grid = new T[width, height];
            Width = width;
            Height = height;
        }

        public bool IsInGrid(int x, int y)
        {
            return 0 <= x && x < Width
                && 0 <= y && y < Height;
        }

        public bool IsInGrid(Coordinate2D coordinate)
        {
            return IsInGrid((int)coordinate.X, (int)coordinate.Y);
        }

        public T Read(int x, int y)
        {
            return _grid[x, y];
        }

        public T Read(Coordinate2D coordinate)
        {
            return Read((int)coordinate.X, (int)coordinate.Y);
        }

        public void Write(int x, int y, T value)
        {
            _grid[x, y] = value;
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

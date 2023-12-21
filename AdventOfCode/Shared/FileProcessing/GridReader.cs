using System;
using System.Collections.Generic;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode.Shared.FileProcessing
{
    public static class GridReader
    {
        public static Grid2D<T> LoadGrid<T>(
            List<string> inputLines,
            Func<char, Coordinate2D, T> initialiser 
            )
        {
            var height = inputLines.Count;
            var grid = new Grid2D<T>(inputLines[0].Length, height);
            foreach (var y in grid.YIndexes())
            {
                foreach (var x in grid.XIndexes())
                {
                    var coordinate = new Coordinate2D(x, y);

                    var c = inputLines[(int)(height - 1 - y)][(int)x];

                    var value = initialiser(c, coordinate);

                    grid.Write(coordinate, value);
                }
            }

            return grid;
        }
    }
}


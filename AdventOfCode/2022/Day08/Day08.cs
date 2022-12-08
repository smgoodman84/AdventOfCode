using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.PrimitiveExtensions;

namespace AdventOfCode._2022.Day08
{
    public class Day08 : Day
    {
        public Day08() : base(2022, 8, "Day08/input_2022_08.txt", "1801", "209880")
        {

        }

        private TreeGrid _treeGrid;
        public override void Initialise()
        {
            var treeHeights = InputLines
                .Select(r => r.ToCharArray().Select(c => int.Parse(c.ToString())).ToArray())
                .ToArray();

            _treeGrid = new TreeGrid(treeHeights);
        }

        private class TreeGrid
        {
            private int[][] _treeHeights;
            public TreeGrid(int[][] treeHeights)
            {
                _treeHeights = treeHeights;
            }

            public int TreeHeight(Coordinate2D coordinate) => TreeHeight((int)coordinate.X, (int)coordinate.Y);
            public int TreeHeight(int x, int y) => _treeHeights[y][x];
            public int Width => _treeHeights[0].Length;
            public int Height => _treeHeights.Length;
        }

        private class TreeVisibility
        {
            public int TotalVisible { get; }

            private readonly TreeGrid _treeGrid;
            private bool[][] _visible;

            public TreeVisibility(TreeGrid treeGrid)
            {
                _treeGrid = treeGrid;

                _visible = Enumerable.Range(0, _treeGrid.Height)
                    .Select(r => Enumerable.Range(0, _treeGrid.Width).Select(c => false).ToArray())
                    .ToArray();

                var viewLines = GetViewLines();
                foreach (var viewLine in viewLines)
                {
                    MarkVisible(viewLine);
                }

                TotalVisible = _visible
                    .SelectMany(r => r)
                    .Sum(visible => visible ? 1 : 0);
            }

            private void MarkVisible(Coordinate2D coordinate)
            {
                _visible[coordinate.Y][coordinate.X] = true;
            }

            private void MarkVisible(IEnumerable<Coordinate2D> viewLine)
            {
                var maxHeight = -1;
                foreach (var coordinate in viewLine)
                {
                    var height = _treeGrid.TreeHeight(coordinate);
                    if (height > maxHeight)
                    {
                        maxHeight = height;
                        MarkVisible(coordinate);
                    }
                }
            }

            private IEnumerable<IEnumerable<Coordinate2D>> GetViewLines()
            {
                foreach (var y in Enumerable.Range(0, _treeGrid.Height))
                {
                    // From the left
                    yield return Enumerable.Range(0, _treeGrid.Width)
                        .Select(x => new Coordinate2D(x, y));

                    // From the right
                    yield return Enumerable.Range(0, _treeGrid.Width)
                        .Reverse()
                        .Select(x => new Coordinate2D(x, y));
                }

                foreach (var x in Enumerable.Range(0, _treeGrid.Width))
                {
                    // From the top
                    yield return Enumerable.Range(0, _treeGrid.Width)
                        .Select(y => new Coordinate2D(x, y));

                    // From the bottom
                    yield return Enumerable.Range(0, _treeGrid.Width)
                        .Reverse()
                        .Select(y => new Coordinate2D(x, y));
                }
            }
        }

        public override string Part1()
        {
            var visibility = new TreeVisibility(_treeGrid);

            return visibility.TotalVisible.ToString();
        }

        public override string Part2()
        {
            // from left
            var maxScore = 0;
            foreach (var y in Enumerable.Range(0, _treeGrid.Height))
            {
                foreach (var x in Enumerable.Range(0, _treeGrid.Width))
                {
                    var score = CalculateScenicScore(x, y);
                    if (score > maxScore)
                    {
                        maxScore = score;
                    }
                }
            }

            return maxScore.ToString();
        }

        private int CalculateScenicScore(int x, int y)
        {
            var thisTreeHeight = _treeGrid.TreeHeight(x, y);

            // out left
            var totalLeft = 0;
            for (var currentX = x-1; currentX >= 0; currentX -= 1)
            {
                var thatTreeHeight = _treeGrid.TreeHeight(currentX, y);
                if (thatTreeHeight < thisTreeHeight)
                {
                    totalLeft += 1;
                }
                if (thatTreeHeight >= thisTreeHeight)
                {
                    totalLeft += 1;
                    break;
                }
            }

            // out right
            var totalRight = 0;
            for (var currentX = x + 1; currentX < _treeGrid.Width; currentX += 1)
            {
                var thatTreeHeight = _treeGrid.TreeHeight(currentX, y);
                if (thatTreeHeight < thisTreeHeight)
                {
                    totalRight += 1;
                }
                if (thatTreeHeight >= thisTreeHeight)
                {
                    totalRight += 1;
                    break;
                }
            }


            // out up
            var totalUp = 0;
            for (var currentY = y - 1; currentY >= 0; currentY -= 1)
            {
                var thatTreeHeight = _treeGrid.TreeHeight(x, currentY);
                if (thatTreeHeight < thisTreeHeight)
                {
                    totalUp += 1;
                }
                if (thatTreeHeight >= thisTreeHeight)
                {
                    totalUp += 1;
                    break;
                }
            }


            // out down
            var totalDown = 0;
            for (var currentY = y + 1; currentY < _treeGrid.Height; currentY += 1)
            {
                var thatTreeHeight = _treeGrid.TreeHeight(x, currentY);
                if (thatTreeHeight < thisTreeHeight)
                {
                    totalDown += 1;
                }
                if (thatTreeHeight >= thisTreeHeight)
                {
                    totalDown += 1;
                    break;
                }
            }

            return totalLeft * totalRight * totalUp * totalDown;
        }
    }
}


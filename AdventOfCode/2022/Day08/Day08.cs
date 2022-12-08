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
                    var score = new ScenicScore(_treeGrid, new Coordinate2D(x, y))
                        .CalculateScenicScore();

                    if (score > maxScore)
                    {
                        maxScore = score;
                    }
                }
            }

            return maxScore.ToString();
        }

        private class ScenicScore
        {
            private readonly TreeGrid _treeGrid;
            private readonly Coordinate2D _coordinate;
            private readonly int _thisTreeHeight;

            public ScenicScore(TreeGrid treeGrid, Coordinate2D coordinate)
            {
                _treeGrid = treeGrid;
                _coordinate = coordinate;
                _thisTreeHeight = _treeGrid.TreeHeight(coordinate);
            }

            private int GetScoreForViewLine(IEnumerable<Coordinate2D> viewLine)
            {
                var totalVisible = 0;
                foreach(var coordinate in viewLine)
                {
                    totalVisible += 1;

                    if (_treeGrid.TreeHeight(coordinate) >= _thisTreeHeight)
                    {
                        return totalVisible;
                    }
                }
                return totalVisible;
            }

            public int CalculateScenicScore()
            {
                var treeX = (int)_coordinate.X;
                var treeY = (int)_coordinate.Y;

                var leftViewLine = treeX <= 0 ? Enumerable.Empty<Coordinate2D>() :
                    Enumerable.Range(0, treeX)
                    .Reverse()
                    .Select(x => new Coordinate2D(x, treeY));

                var rightViewLine = treeX >= _treeGrid.Width - 1 ? Enumerable.Empty<Coordinate2D>() :
                    Enumerable.Range(treeX + 1, _treeGrid.Width - treeX - 1)
                    .Select(x => new Coordinate2D(x, treeY));

                var upViewLine = treeY <= 0 ? Enumerable.Empty<Coordinate2D>() :
                    Enumerable.Range(0, treeY)
                    .Reverse()
                    .Select(y => new Coordinate2D(treeX, y));

                var downViewLine = treeY >= _treeGrid.Height - 1 ? Enumerable.Empty<Coordinate2D>() :
                    Enumerable.Range(treeY + 1, _treeGrid.Height - treeY - 1)
                    .Select(y => new Coordinate2D(treeX, y));

                return GetScoreForViewLine(leftViewLine)
                    * GetScoreForViewLine(rightViewLine)
                    * GetScoreForViewLine(upViewLine)
                    * GetScoreForViewLine(downViewLine);
            }
        }
    }
}


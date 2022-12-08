using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;
using AdventOfCode.Shared.PrimitiveExtensions;

namespace AdventOfCode._2022.Day08
{
    public class Day08 : Day
    {
        public Day08() : base(2022, 8, "Day08/input_2022_08.txt", "", "")
        {

        }


        private int[][] _treeHeights;
        private bool[][] _visible;

        public override void Initialise()
        {
            _treeHeights = InputLines
                .Select(r => r.ToCharArray().Select(c => int.Parse(c.ToString())).ToArray())
                .ToArray();

            _visible = InputLines
                .Select(r => r.ToCharArray().Select(c => false).ToArray())
                .ToArray();
        }

        public override string Part1()
        {
            // from left
            var leftVisible = 0;
            foreach (var y in Enumerable.Range(0, _treeHeights.Length))
            {
                var maxHeight = -1;
                var visibleCount = 0;
                foreach (var x in Enumerable.Range(0, _treeHeights[0].Length))
                {
                    var height = _treeHeights[y][x];
                    if (height > maxHeight)
                    {
                        //Console.WriteLine($"Left {x}{y}");
                        visibleCount += 1;
                        maxHeight = height;
                        _visible[y][x] = true;
                    }
                }
                leftVisible += visibleCount;
            }

            // from right
            var rightVisible = 0;
            foreach (var y in Enumerable.Range(0, _treeHeights.Length))
            {
                var maxHeight = -1;
                var visibleCount = 0;
                foreach (var x in Enumerable.Range(0, _treeHeights[0].Length).Reverse())
                {
                    var height = _treeHeights[y][x];
                    if (height > maxHeight)
                    {
                        //Console.WriteLine($"Right {x}{y}");
                        visibleCount += 1;
                        maxHeight = height;
                        _visible[y][x] = true;
                    }
                }
                rightVisible += visibleCount;
            }

            // from top
            var topVisible = 0;
            foreach (var x in Enumerable.Range(0, _treeHeights[0].Length))
            {
                var maxHeight = -1;
                var visibleCount = 0;
                foreach (var y in Enumerable.Range(0, _treeHeights.Length))
                {
                    var height = _treeHeights[y][x];
                    if (height > maxHeight)
                    {
                        //Console.WriteLine($"Top {x}{y}");
                        visibleCount += 1;
                        maxHeight = height;
                        _visible[y][x] = true;
                    }
                }
                topVisible += visibleCount;
            }

            // from bottom
            var bottomVisible = 0;
            foreach (var x in Enumerable.Range(0, _treeHeights[0].Length))
            {
                var maxHeight = -1;
                var visibleCount = 0;
                foreach (var y in Enumerable.Range(0, _treeHeights.Length).Reverse())
                {
                    var height = _treeHeights[y][x];
                    if (height > maxHeight)
                    {
                        //Console.WriteLine($"Bottom {x}{y}");
                        visibleCount += 1;
                        maxHeight = height;
                        _visible[y][x] = true;
                    }
                }
                bottomVisible += visibleCount;
            }

            var totalVisible = _visible
                .SelectMany(r => r)
                .Sum(visible => visible ? 1 : 0);

            return totalVisible.ToString();
        }

        public override string Part2()
        {
            // from left
            var maxScore = 0;
            foreach (var y in Enumerable.Range(0, _treeHeights.Length))
            {
                foreach (var x in Enumerable.Range(0, _treeHeights[0].Length))
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
            var thisTreeHeight = _treeHeights[y][x];

            // out left
            var totalLeft = 0;
            for (var currentX = x-1; currentX >= 0; currentX -= 1)
            {
                var thatTreeHeight = _treeHeights[y][currentX];
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
            for (var currentX = x + 1; currentX < _treeHeights[0].Length; currentX += 1)
            {
                var thatTreeHeight = _treeHeights[y][currentX];
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
                var thatTreeHeight = _treeHeights[currentY][x];
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
            for (var currentY = y + 1; currentY < _treeHeights.Length; currentY += 1)
            {
                var thatTreeHeight = _treeHeights[currentY][x];
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


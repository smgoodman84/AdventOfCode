using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.PrimitiveExtensions;

namespace AdventOfCode._2022.Day14
{
    public class Day14 : Day
    {
        public Day14() : base(2022, 14, "Day14/input_2022_14.txt", "1016", "")
        {

        }

        private static Coordinate2D SandOrigin = new Coordinate2D(500, 0);
        private Grid2D<Item> _cave;

        public override void Initialise()
        {
            var rockPaths = InputLines
                .Select(x => new Path(x))
                .ToList();

            var minX = rockPaths.Select(p => p.MinX).Concat(new[] { SandOrigin.X }).Min();
            var maxX = rockPaths.Select(p => p.MaxX).Concat(new[] { SandOrigin.X }).Max();
            var minY = rockPaths.Select(p => p.MinY).Concat(new[] { SandOrigin.Y }).Min();
            var maxY = rockPaths.Select(p => p.MaxY).Concat(new[] { SandOrigin.Y }).Max();

            _cave = new Grid2D<Item>((int)minX, (int)minY, (int)maxX, (int)maxY);

            foreach (var rockPath in rockPaths)
            {
                foreach (var coordinate in rockPath.WalkPath())
                {
                    _cave.Write(coordinate, Item.Rock);
                }
            }
        }

        public bool DoesNextSandComesToRest()
        {
            var sandPosition = SandOrigin;

            while (true)
            {
                var down = sandPosition.Up(); // Inverted Coordinate System

                if (!_cave.IsInGrid(down))
                {
                    return false;
                }

                if (_cave.Read(down) == Item.Empty)
                {
                    sandPosition = down;
                    continue;
                }

                var downLeft = down.Left();
                if (!_cave.IsInGrid(downLeft))
                {
                    return false;
                }

                if (_cave.Read(downLeft) == Item.Empty)
                {
                    sandPosition = downLeft;
                    continue;
                }

                var downRight = down.Right();
                if (!_cave.IsInGrid(downRight))
                {
                    return false;
                }

                if (_cave.Read(downRight) == Item.Empty)
                {
                    sandPosition = downRight;
                    continue;
                }

                _cave.Write(sandPosition, Item.Sand);
                return true;
            }
        }

        public override string Part1()
        {
            var sandCount = 0;
            while (DoesNextSandComesToRest())
            {
                sandCount += 1;
            }

            return sandCount.ToString();
        }

        public override string Part2()
        {
            return "";
        }

        private enum Item
        {
            Empty,
            Rock,
            Sand
        }

        private class Path
        {
            private List<Coordinate2D> _points;

            public long MinX => _points.Select(p => p.X).Min();
            public long MaxX => _points.Select(p => p.X).Max();
            public long MinY => _points.Select(p => p.Y).Min();
            public long MaxY => _points.Select(p => p.Y).Max();

            public Path(string path)
            {
                _points = path
                    .Split(" -> ")
                    .Select(x => new Coordinate2D(x))
                    .ToList();
            }

            public IEnumerable<Coordinate2D> WalkPath()
            {
                var start = _points.First();
                yield return start;

                var current = start;
                foreach(var end in _points.Skip(1))
                {
                    var xDelta = 0;
                    if (end.X > current.X)
                    {
                        xDelta = 1;
                    }
                    if (end.X < current.X)
                    {
                        xDelta = -1;
                    }

                    var yDelta = 0;
                    if (end.Y > current.Y)
                    {
                        yDelta = 1;
                    }
                    if (end.Y < current.Y)
                    {
                        yDelta = -1;
                    }

                    while (current.X != end.X || current.Y != end.Y)
                    {
                        current = new Coordinate2D(current.X + xDelta, current.Y + yDelta);
                        yield return current;
                    }
                }
            }
        }
    }
}

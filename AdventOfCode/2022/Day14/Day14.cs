using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2022.Day14
{
    public class Day14 : Day
    {
        public Day14() : base(2022, 14, "Day14/input_2022_14.txt", "1016", "25402")
        {

        }

        private static readonly Coordinate2D SandOrigin = new Coordinate2D(500, 0);
        private List<Path> _rockPaths;

        public override void Initialise()
        {
            _rockPaths = InputLines
                .Select(x => new Path(x))
                .ToList();
        }

        private bool DoesNextSandComesToRest(Grid2D<Item> cave, Coordinate2D atPosition = null)
        {
            var sandPosition = SandOrigin;

            while (true)
            {
                var down = sandPosition.Up(); // Inverted Coordinate System

                if (!cave.IsInGrid(down))
                {
                    return false;
                }

                if (cave.Read(down) == Item.Empty)
                {
                    sandPosition = down;
                    continue;
                }

                var downLeft = down.Left();
                if (!cave.IsInGrid(downLeft))
                {
                    return false;
                }

                if (cave.Read(downLeft) == Item.Empty)
                {
                    sandPosition = downLeft;
                    continue;
                }

                var downRight = down.Right();
                if (!cave.IsInGrid(downRight))
                {
                    return false;
                }

                if (cave.Read(downRight) == Item.Empty)
                {
                    sandPosition = downRight;
                    continue;
                }

                cave.Write(sandPosition, Item.Sand);
                if (atPosition != null)
                {
                    var result = sandPosition.X == atPosition.X
                        && sandPosition.Y == atPosition.Y;
                    return result;
                }
                return true;
            }
        }

        public override string Part1()
        {
            var minX = _rockPaths.Select(p => p.MinX).Concat(new[] { SandOrigin.X }).Min();
            var maxX = _rockPaths.Select(p => p.MaxX).Concat(new[] { SandOrigin.X }).Max();
            var minY = _rockPaths.Select(p => p.MinY).Concat(new[] { SandOrigin.Y }).Min();
            var maxY = _rockPaths.Select(p => p.MaxY).Concat(new[] { SandOrigin.Y }).Max();

            var cave = new Grid2D<Item>((int)minX, (int)minY, (int)maxX, (int)maxY);

            foreach (var rockPath in _rockPaths)
            {
                foreach (var coordinate in rockPath.WalkPath())
                {
                    cave.Write(coordinate, Item.Rock);
                }
            }

            var sandCount = 0;
            while (DoesNextSandComesToRest(cave))
            {
                sandCount += 1;
            }

            return sandCount.ToString();
        }

        public override string Part2()
        {
            var minX = _rockPaths.Select(p => p.MinX).Concat(new[] { SandOrigin.X }).Min() - 500;
            var maxX = _rockPaths.Select(p => p.MaxX).Concat(new[] { SandOrigin.X }).Max() + 500;
            var minY = _rockPaths.Select(p => p.MinY).Concat(new[] { SandOrigin.Y }).Min();
            var maxY = _rockPaths.Select(p => p.MaxY).Concat(new[] { SandOrigin.Y }).Max() + 2;

            var cave = new Grid2D<Item>((int)minX, (int)minY, (int)maxX, (int)maxY);

            foreach (var rockPath in _rockPaths)
            {
                foreach (var coordinate in rockPath.WalkPath())
                {
                    cave.Write(coordinate, Item.Rock);
                }
            }

            for (var x = minX; x <= maxX; x++)
            {
                cave.Write((int)x, (int)maxY, Item.Rock);
            }

            var sandCount = 0;
            while (!DoesNextSandComesToRest(cave, SandOrigin))
            {
                sandCount += 1;
            }
            sandCount += 1;

            return sandCount.ToString();
        }

        private enum Item
        {
            Empty,
            Rock,
            Sand
        }

        private class Path
        {
            private readonly List<Coordinate2D> _points;

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
                foreach (var end in _points.Skip(1))
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

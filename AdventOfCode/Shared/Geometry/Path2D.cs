using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Shared.Geometry
{
    public class Path2D
    {
        public IEnumerable<Coordinate2D> Path { get; }
        public int Length => Path.Count();

        public Path2D(IEnumerable<Coordinate2D> path)
        {
            Path = path.ToList();
        }

        public Path2D Append(Coordinate2D coordinate)
        {
            var newPath = Path.Concat(new[] { coordinate });
            return new Path2D(newPath);
        }

        public Path2D MoveUp() => Move(x => x, y => y + 1);
        public Path2D MoveDown() => Move(x => x, y => y - 1);
        public Path2D MoveLeft() => Move(x => x - 1, y => y);
        public Path2D MoveRight() => Move(x => x + 1, y => y);

        private Path2D Move(Func<long, long> adjustX, Func<long, long> adjustY)
        {
            var last = Path.Last();
            return Append(new Coordinate2D(adjustX(last.X), adjustY(last.Y)));
        }
    }
}

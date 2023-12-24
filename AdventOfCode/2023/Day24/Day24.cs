using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2023.Day24
{
    public class Day24 : Day
    {
        public Day24() : base(2023, 24, "Day24/input_2023_24.txt", "19523", "", true)
        {

        }

        private List<Hailstone> _hailstones;
        public override void Initialise()
        {
            _hailstones = InputLines
                .Select((l, i) => new Hailstone(l, i))
                .ToList();
        }

        public override string Part1()
        {
            // var minPosition = 7.0;
            // var maxPosition = 27.0;
            var minPosition = 200000000000000.0;
            var maxPosition = 400000000000000.0;

            var count = 0;
            foreach(var h1 in _hailstones)
            {
                foreach (var h2 in _hailstones.Where(x => x.LineNumber > h1.LineNumber))
                {
                    var intersects = h1.GetLine().TryGetIntersection(h2.GetLine(), out var intersectX, out var intersectY);
                    if (!intersects)
                    {
                        TraceLine($"{h1} Does not intersect {h2}");
                        continue;
                    }

                    if ((h1.Velocity.X > 0 && intersectX < h1.Start.X)
                        || (h1.Velocity.X < 0 && intersectX > h1.Start.X))
                    {
                        TraceLine($"{h1} Intersects in the past (1) {h2} at ({intersectX},{intersectY})");
                        continue;
                    }

                    if ((h2.Velocity.X > 0 && intersectX < h2.Start.X)
                        || (h2.Velocity.X < 0 && intersectX > h2.Start.X))
                    {
                        TraceLine($"{h1} Intersects in the past (2) {h2} at ({intersectX},{intersectY})");
                        continue;
                    }

                    if (minPosition <= intersectX && intersectX <= maxPosition
                        && minPosition <= intersectY && intersectY <= maxPosition)
                    {
                        TraceLine($"{h1} Intersects in the window {h2} at ({intersectX},{intersectY})");
                        count += 1;
                        continue;
                    }

                    TraceLine($"{h1} Intersects outside the window {h2} at ({intersectX},{intersectY})");
                }
            }
            return count.ToString();
        }

        public override string Part2()
        {
            return string.Empty;
        }

        private class Hailstone
        {
            public Coordinate3D Start { get; set; }
            public Coordinate3D Velocity { get; set; }
            public int LineNumber { get; }

            public Hailstone(string description, int lineNumber)
            {
                var split = description.Split(" @ ");
                Start = new Coordinate3D(split[0]);
                Velocity = new Coordinate3D(split[1]);
                LineNumber = lineNumber;
            }

            public Line2D GetLine()
            {
                return new Line2D(GetPosition2D(0), GetPosition2D(1));
            }

            private Coordinate2D GetPosition2D(int time)
            {
                var x = Start.X + time * Velocity.X;
                var y = Start.Y + time * Velocity.Y;
                return new Coordinate2D(x, y);
            }

            public override string ToString()
            {
                return $"{LineNumber}: {Start} @ {Velocity}";
            }
        }
    }
}

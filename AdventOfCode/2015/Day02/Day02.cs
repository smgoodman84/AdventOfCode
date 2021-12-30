using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2015.Day02
{
    public class Day02 : Day
    {
        public Day02() : base(2015, 2, @"Day02/input.txt", "1586300", "3737498")
        {
        }

        private List<Present> _presents;
        public override void Initialise()
        {
            _presents = InputLines
                .Select(l => Present.Parse(l))
                .ToList();
        }

        private class Present : RectangularCuboid
        {
            public Present(long width, long height, long length)
                : base (width, height, length)
            {
            }

            public static Present Parse(string dimensions)
            {
                var dims = dimensions
                    .Split('x')
                    .Select(long.Parse)
                    .OrderBy(x => x)
                    .ToArray();

                return new Present(dims[0], dims[1], dims[2]);
            }

            public long WrappingPaperRequired => SurfaceArea + SmallArea;

            public long RibbonRequired =>
                2 * SmallDimension +
                2 * MiddleDimension +
                Volume;
        }

        public override string Part1()
        {
            var result = _presents.Sum(x => x.WrappingPaperRequired);

            return result.ToString();
        }

        public override string Part2()
        {
            var result = _presents.Sum(x => x.RibbonRequired);

            return result.ToString();
        }
    }
}
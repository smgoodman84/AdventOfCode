using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2015.Day02
{
    public class Day02 : Day
    {
        public Day02() : base(2015, 1, @"Day02/input.txt", "1586300", "")
        {
        }

        private List<Present> _presents;
        public override void Initialise()
        {
            _presents = InputLines
                .Select(l => new Present(l))
                .ToList();
        }

        private class Present
        {
            public Present(string dimensions)
            {
                var dims = dimensions
                    .Split('x')
                    .Select(int.Parse)
                    .OrderBy(x => x)
                    .ToArray();

                Height = dims[0];
                Width = dims[1];
                Length = dims[2];
            }

            public int Height { get; private set; }
            public int Width { get; private set; }
            public int Length { get; private set; }

            public int WrappingPaperRequired =>
                2 * Length * Width +
                3 * Width * Height +
                2 * Height * Length;
        }

        public override string Part1()
        {
            var result = _presents.Sum(x => x.WrappingPaperRequired);

            return result.ToString();
        }

        public override string Part2()
        {
            var line = InputLines.First();

            var position = 0;
            var floor = 0;

            foreach (var c in line)
            {
                if (c == '(')
                {
                    floor += 1;
                }

                if (c == ')')
                {
                    floor -= 1;
                }

                position += 1;

                if (floor == -1)
                {
                    return position.ToString();
                }
            }

            return "";
        }
    }
}
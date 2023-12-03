using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2023.Day03
{
    public class Day03 : Day
    {
        public Day03() : base(2023, 3, "Day03/input_2023_03.txt", "498559", "")
        {

        }

        public override void Initialise()
        {
        }

        public override string Part1()
        {
            var grid = InputLines
                .Select(l => l.Trim().ToArray())
                .ToArray();

            var symbols = new List<Coordinate2D>();
            foreach (var y in Enumerable.Range(0, grid.Length))
            {
                foreach (var x in Enumerable.Range(0, grid[y].Length))
                {
                    if (IsSymbol(grid[y][x]))
                    {
                        symbols.Add(new Coordinate2D(x, y));
                    }
                }
            }

            var partNumbers = new List<PartNumber>();
            foreach (var y in Enumerable.Range(0, grid.Length))
            {
                var readingNumber = false;
                var number = string.Empty;
                var location = new List<Coordinate2D>();
                foreach (var x in Enumerable.Range(0, grid[y].Length))
                {
                    var isDigit = IsDigit(grid[y][x]);
                    if (readingNumber)
                    {
                        if (isDigit)
                        {
                            number = $"{number}{grid[y][x]}";
                            location.Add(new Coordinate2D(x, y));
                        }

                        if (!isDigit || x == grid[y].Length - 1)
                        {
                            var partNumber = new PartNumber
                            {
                                Number = int.Parse(number),
                                Location = location,
                            };
                            partNumbers.Add(partNumber);

                            readingNumber = false;
                            number = string.Empty;
                            location = new List<Coordinate2D>();
                        }
                    }
                    else
                    {
                        if (isDigit)
                        {
                            readingNumber = true;
                            number = $"{number}{grid[y][x]}";
                            location.Add(new Coordinate2D(x, y));
                        }
                    }
                }
            }

            var actualPartNumbers = new List<PartNumber>();
            foreach (var partNumber in partNumbers)
            {
                var allNeighbours = partNumber
                    .Location
                    .SelectMany(l => l.AllNeighbours())
                    .Where(n => !partNumber.Location.Any(l => l.X == n.X && l.Y == n.Y))
                    .ToList();

                if (symbols.Any(s => allNeighbours.Any(n => n.X == s.X && n.Y == s.Y)))
                {
                    actualPartNumbers.Add(partNumber);
                }
            }

            var sum = actualPartNumbers.Sum(pn => pn.Number);

            return sum.ToString();
        }

        public override string Part2()
        {
            return string.Empty;
        }

        private static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private static bool IsSymbol(char c)
        {
            if (IsDigit(c))
            {
                return false;
            }

            return c != '.';
        }

        private class PartNumber
        {
            public int Number { get; set; }
            public List<Coordinate2D> Location { get; set; } = new List<Coordinate2D>();
        }
    }
}


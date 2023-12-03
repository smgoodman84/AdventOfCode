using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2023.Day03
{
    public class Day03 : Day
    {
        public Day03() : base(2023, 3, "Day03/input_2023_03.txt", "498559", "")
        {

        }

        private char[][] _grid;
        private List<Coordinate2D> _symbols;
        private List<PartNumber> _partNumbers;
        public override void Initialise()
        {
            _grid = InputLines
                .Select(l => l.Trim().ToArray())
                .ToArray();

            _symbols = new List<Coordinate2D>();
            foreach (var y in Enumerable.Range(0, _grid.Length))
            {
                foreach (var x in Enumerable.Range(0, _grid[y].Length))
                {
                    if (IsSymbol(_grid[y][x]))
                    {
                        _symbols.Add(new Coordinate2D(x, y));
                    }
                }
            }

            var potentialPartNumbers = new List<PartNumber>();
            foreach (var y in Enumerable.Range(0, _grid.Length))
            {
                var readingNumber = false;
                var number = string.Empty;
                var location = new List<Coordinate2D>();
                foreach (var x in Enumerable.Range(0, _grid[y].Length))
                {
                    var isDigit = IsDigit(_grid[y][x]);
                    if (readingNumber)
                    {
                        if (isDigit)
                        {
                            number = $"{number}{_grid[y][x]}";
                            location.Add(new Coordinate2D(x, y));
                        }

                        if (!isDigit || x == _grid[y].Length - 1)
                        {
                            var partNumber = new PartNumber
                            {
                                Number = int.Parse(number),
                                Location = location,
                            };
                            potentialPartNumbers.Add(partNumber);

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
                            number = $"{number}{_grid[y][x]}";
                            location.Add(new Coordinate2D(x, y));
                        }
                    }
                }
            }

            _partNumbers = new List<PartNumber>();
            foreach (var partNumber in potentialPartNumbers)
            {
                var allNeighbours = partNumber
                    .Location
                    .SelectMany(l => l.AllNeighbours())
                    .Where(n => !partNumber.Location.Any(l => l.X == n.X && l.Y == n.Y))
                    .ToList();

                if (_symbols.Any(s => allNeighbours.Any(n => n.X == s.X && n.Y == s.Y)))
                {
                    _partNumbers.Add(partNumber);
                }
            }
        }

        public override string Part1()
        {
            var sum = _partNumbers.Sum(pn => pn.Number);

            return sum.ToString();
        }

        public override string Part2()
        {
            var totalRatios = 0;
            var potentialGears = _symbols.Where(s => _grid[s.Y][s.X] == '*');

            foreach (var potentialGear in potentialGears)
            {
                var gearNeighoburs = potentialGear.AllNeighbours().ToList();

                var neighbouringPartNumbers = _partNumbers
                    .Where(pn => pn.Location.Any(l => gearNeighoburs.Any(gn => gn.X == l.X && gn.Y == l.Y)))
                    .ToList();

                if (neighbouringPartNumbers.Count() == 2)
                {
                    var ratio = 1;
                    foreach (var neighbouringPartNumber in neighbouringPartNumbers)
                    {
                        ratio *= neighbouringPartNumber.Number;
                    }
                    totalRatios += ratio;
                }
            }
            return totalRatios.ToString();
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


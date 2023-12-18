using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2023.Day18
{
    public class Day18 : Day
    {
        public Day18() : base(2023, 18, "Day18/input_2023_18.txt", "", "", true)
        {

        }

        private List<Instruction> _instructions;
        public override void Initialise()
        {
            _instructions = InputLines
                .Select(l => new Instruction(l))
                .ToList();
        }

        public override string Part1()
        {
            var grid = new InfiniteGrid2D<Location>(new Location
            {
                IsDug = false,
                Colour = "#000000"
            });

            var currentCoordinate = Coordinate2D.Origin;
            grid.Write(currentCoordinate, new Location
            {
                IsDug = true,
                Colour = "#ffffff"
            });

            foreach (var instruction in _instructions)
            {
                for (var i = 0; i < instruction.Distance; i += 1)
                {
                    currentCoordinate = currentCoordinate.Neighbour(instruction.Direction);

                    grid.Write(currentCoordinate, new Location
                    {
                        IsDug = true,
                        Colour = instruction.Colour
                    });
                }
            }

            Draw(grid, "pre-dig.txt");

            var firstInstruction = _instructions.First();
            var fillStart = Coordinate2D.Origin.Neighbour(OppositeDirection(firstInstruction.Direction));

            DigOut(grid, fillStart);

            TraceLine();
            Draw(grid, "post-dig.txt");

            var dugCount = CountDug(grid);

            return dugCount.ToString();
        }

        private static void DigOut(InfiniteGrid2D<Location> grid, Coordinate2D location)
        {
            var neighbours = location.Neighbours();
            foreach (var neighbour in neighbours)
            {
                var current = grid.Read(neighbour);
                if (!current.IsDug)
                {
                    grid.Write(neighbour, new Location
                    {
                        IsDug = true,
                        Colour = "#000000"
                    });

                    DigOut(grid, neighbour);
                }
            }
        }

        private static void DigOut1(InfiniteGrid2D<Location> grid)
        {
            foreach (var y in grid.YIndexes())
            {
                var inside = false;
                var onEdge = false;
                foreach (var x in grid.XIndexes())
                {
                    var current = grid.Read(x, y);
                    if (inside)
                    {
                        if (onEdge)
                        {
                            if (!current.IsDug)
                            {
                                onEdge = false;
                                inside = false;
                            }
                        }
                        else
                        {
                            if (current.IsDug)
                            {
                                onEdge = true;
                            }
                            else
                            {
                                grid.Write(x, y, new Location
                                {
                                    IsDug = true,
                                    Colour = "#000000"
                                });
                            }
                        }
                    }
                    else
                    {
                        if (onEdge)
                        {
                            if (!current.IsDug)
                            {
                                onEdge = false;
                                inside = true;

                                grid.Write(x, y, new Location
                                {
                                    IsDug = true,
                                    Colour = "#000000"
                                });
                            }
                        }
                        else
                        {
                            if (current.IsDug)
                            {
                                onEdge = true;
                            }
                        }
                    }
                }
            }
        }

        private static int CountDug(InfiniteGrid2D<Location> grid)
        {
            var dug = 0;
            foreach (var y in grid.YIndexes())
            {
                foreach (var x in grid.XIndexes())
                {
                    var current = grid.Read(x, y);
                    if (current.IsDug)
                    {
                        dug += 1;
                    }
                }
            }

            return dug;
        }

        public override string Part2()
        {
            return string.Empty;
        }

        private void Draw(InfiniteGrid2D<Location> grid)
        {
            foreach (var y in grid.YIndexes().OrderByDescending(y => y))
            {
                foreach (var x in grid.XIndexes())
                {
                    var current = grid.Read(x, y);
                    var isStart = x == 0 && y == 0;
                    var toDraw = isStart ? 'S' : (current.IsDug ? '#' : '.');
                    Trace(toDraw);
                }
                TraceLine();
            }
        }
        private void Draw(InfiniteGrid2D<Location> grid, string filename)
        {
            var lines = new List<string>();
            foreach (var y in grid.YIndexes().OrderByDescending(y => y))
            {
                var currentLine = "";
                foreach (var x in grid.XIndexes())
                {
                    var current = grid.Read(x, y);
                    var isStart = x == 0 && y == 0;
                    var toDraw = isStart ? 'S' : (current.IsDug ? '#' : '.');
                    currentLine = $"{currentLine}{toDraw}";
                }
                lines.Add(currentLine);
            }

            File.WriteAllLines(filename, lines);
        }

        private Direction OppositeDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up: return Direction.Down;
                case Direction.Down: return Direction.Up;
                case Direction.Left: return Direction.Right;
                case Direction.Right: return Direction.Left;
            }

            throw new Exception($"Unrecognised direction: {direction}");
        }

        private class Location
        {
            public bool IsDug { get; set; }
            public string Colour { get; set; }
        }

        private class Instruction
        {
            public string Description { get; set; }
            public Direction Direction { get; set; }
            public int Distance { get; set; }
            public string Colour { get; set; }

            public Instruction(string description)
            {
                Description = description;

                var split = description.Split(" ");
                Direction = ParseDirection(split[0]);
                Distance = int.Parse(split[1]);
                Colour = split[2].Substring(1, 7);
            }

            private Direction ParseDirection(string direction)
            {
                switch (direction)
                {
                    case "U": return Direction.Up;
                    case "D": return Direction.Down;
                    case "L": return Direction.Left;
                    case "R": return Direction.Right;
                }

                throw new Exception($"Unrecognised direction: {direction}");
            }
        }
    }
}

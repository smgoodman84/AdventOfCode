using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.Numbers;
using AdventOfCode.Shared.PrimitiveExtensions;

namespace AdventOfCode._2022.Day17
{
    public class Day17 : Day
    {
        public Day17() : base(2022, 17, "Day17/input_2022_17.txt", "3071", "")
        {

        }


        private const string Rocks = @"
####

.#.
###
.#.

..#
..#
###

#
#
#
#

##
##
";

        private Queue<Grid2D<Space>> _rocks;
        private Queue<Move> _moves;
        public override void Initialise()
        {
            _moves = Parse(InputLines.Single());
            _rocks = ParsePieces();
        }

        private Queue<Grid2D<Space>> ParsePieces()
        {
            var result = new Queue<Grid2D<Space>>();

            var rocks = LineGrouper.GroupLinesBySeperator(Rocks.Split(Environment.NewLine));

            foreach (var rock in rocks.Where(p => p.Count > 0))
            {
                var height = rock.Count;
                var left = rock.Min(x => x.IndexOf('#'));
                var right = rock.Max(x => x.LastIndexOf('#'));
                var width = right - left + 1;
                var grid = new Grid2D<Space>(width, height);

                var y = height - 1;
                foreach(var line in rock)
                {
                    var x = 0;
                    foreach(var c in line)
                    {
                        switch (c)
                        {
                            case ' ':
                                grid.Write(x, y, Space.Empty);
                                break;
                            case '.':
                                grid.Write(x, y, Space.Empty);
                                break;
                            case '#':
                                grid.Write(x, y, Space.Rock);
                                break;
                        }
                        x += 1;
                    }
                    y -= 1;
                }

                result.Enqueue(grid);
            }

            return result;
        }

        private enum Space
        {
            Empty,
            Rock
        }

        private enum Move
        {
            Left,
            Right
        }

        private Queue<Move> Parse(string input)
        {
            var queue = new Queue<Move>();
            foreach (var c in input)
            {
                if (c == '<')
                {
                    queue.Enqueue(Move.Left);
                }
                else if (c == '>')
                {
                    queue.Enqueue(Move.Right);
                }
            }
            return queue;
        }

        private Grid2D<Space> GetNextRock()
        {
            var rock = _rocks.Dequeue();
            _rocks.Enqueue(rock);
            return rock;
        }

        private Move GetNextMove()
        {
            var move = _moves.Dequeue();
            _moves.Enqueue(move);
            return move;
        }

        public override string Part1()
        {
            return GetHeightAfterRocks(2022).ToString();
        }

        public override string Part2()
        {
            return "";
        }

        private int GetHeightAfterRocks(int rockCount)
        {
            var maxHeight = (_rocks.Max(p => p.Height) * (rockCount + 1)) + 3;
            var chamber = new Grid2D<Space>(7, maxHeight);
            var highestRock = 0;

            var rockNumber = 1;
            while (rockNumber <= rockCount)
            {
                var rock = GetNextRock();
                var rockLocation = new Coordinate2D(2, 3 + highestRock);

                /*
                Console.WriteLine($"Rock {rockNumber} Dropped:");
                for (var y = highestRock + 3 + rock.Height; y >= 0; y--)
                {
                    Console.Write('|');
                    for (var x = 0; x < chamber.Width; x++)
                    {
                        var isChamberRock = chamber.Read(x, y) == Space.Rock;
                        var fallingCoordinates = rockLocation.Add(new Coordinate2D(-x, -y));
                        var isFallingRock = rock.IsInGrid(fallingCoordinates) && rock.Read(fallingCoordinates) == Space.Rock;
                        if (isChamberRock)
                        {
                            Console.Write('#');
                        }
                        if (isFallingRock)
                        {
                            Console.Write('@');
                        }
                        else
                        {
                            Console.Write('.');
                        }
                    }
                    Console.WriteLine('|');
                }
                */
                var rockLanded = false;
                while (!rockLanded)
                {
                    var move = GetNextMove();
                    if (move == Move.Left)
                    {
                        var left = rockLocation.Left();
                        if (CanRockMoveTo(chamber, rock, left))
                        {
                            rockLocation = left;
                        }
                    }
                    else if (move == Move.Right)
                    {
                        var right = rockLocation.Right();
                        if (CanRockMoveTo(chamber, rock, right))
                        {
                            rockLocation = right;
                        }
                    }

                    var down = rockLocation.Down();
                    if (CanRockMoveTo(chamber, rock, down))
                    {
                        rockLocation = down;
                    }
                    else
                    {
                        var highestNewRock = SettleRock(chamber, rock, rockLocation) + 1;
                        if (highestNewRock > highestRock)
                        {
                            highestRock = highestNewRock;
                        }

                        rockLanded = true;
                    }
                }

                if (rockNumber < 5)
                {
                    Console.WriteLine($"Rock {rockNumber} Landed:");
                    for (var y = highestRock; y >= 0; y--)
                    {
                        Console.Write('|');
                        for (var x = 0; x < chamber.Width; x++)
                        {
                            var isRock = chamber.Read(x, y) == Space.Rock;
                            if (isRock)
                            {
                                Console.Write('#');
                            }
                            else
                            {
                                Console.Write('.');
                            }
                        }
                        Console.WriteLine('|');
                    }
                }

                rockNumber += 1;
            }

            return highestRock;
        }

        private int SettleRock(
            Grid2D<Space> chamber,
            Grid2D<Space> rock,
            Coordinate2D rockLocation)
        {
            var maxHeight = 0L;
            for (var y = 0; y < rock.Height; y++)
            {
                for (var x = 0; x < rock.Width; x++)
                {
                    if (rock.Read(x, y) == Space.Rock)
                    {
                        var chamberLocation = rockLocation.Add(new Coordinate2D(x, y));
                        chamber.Write(chamberLocation, Space.Rock);
                        if (chamberLocation.Y > maxHeight)
                        {
                            maxHeight = chamberLocation.Y;
                        }
                    }
                }
            }

            return (int)maxHeight;
        }

        private bool CanRockMoveTo(
            Grid2D<Space> chamber,
            Grid2D<Space> rock,
            Coordinate2D rockLocation)
        {
            for(var y = 0; y < rock.Height; y++)
            {
                for (var x = 0; x < rock.Width; x++)
                {
                    if (rock.Read(x, y) == Space.Rock)
                    {
                        var chamberLocation = rockLocation.Add(new Coordinate2D(x, y));
                        if (!chamber.IsInGrid(chamberLocation))
                        {
                            return false;
                        }

                        var chamberIsRock = chamber.Read(chamberLocation) == Space.Rock;
                        if (chamberIsRock)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}

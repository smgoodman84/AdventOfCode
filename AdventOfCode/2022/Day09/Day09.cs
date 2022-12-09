using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.PrimitiveExtensions;

namespace AdventOfCode._2022.Day09
{
    public class Day09 : Day
    {
        public Day09() : base(2022, 9, "Day09/input_2022_09.txt", "5619", "")
        {

        }

        private List<Instruction> _instructions;
        public override void Initialise()
        {
            _instructions = InputLines
                .Select(line => new Instruction(line))
                .ToList();
        }

        public override string Part1()
        {
            var tailTracker = new TailTracker(_instructions);
            var result = tailTracker.CountTailVisits();
            return result.ToString();
        }

        public override string Part2()
        {
            return "";
        }

        private class TailTracker
        {
            private readonly List<Instruction> _instructions;

            private Coordinate2D _headPosition;
            private Coordinate2D _tailPosition;
            private List<Coordinate2D> _tailVisits;

            private Dictionary<Direction, Coordinate2D> _directionMoves
                = new Dictionary<Direction, Coordinate2D>
                {
                    { Direction.Up, new Coordinate2D(0, -1) },
                    { Direction.Down, new Coordinate2D(0, 1) },
                    { Direction.Left, new Coordinate2D(-1, 0) },
                    { Direction.Right, new Coordinate2D(1, 0) },
                };

            public TailTracker(IEnumerable<Instruction> instructions)
            {
                _instructions = instructions.ToList();

                _headPosition = Coordinate2D.Origin;
                _tailPosition = Coordinate2D.Origin;

                _tailVisits = new List<Coordinate2D>()
                {
                    _tailPosition
                };
            }

            public int CountTailVisits()
            {
                foreach (var instruction in _instructions)
                {
                    var count = instruction.Distance;
                    while (count > 0)
                    {
                        _headPosition = _headPosition.Add(_directionMoves[instruction.Direction]);

                        UpdateTailPosition();

                        _tailVisits.Add(_tailPosition);
                        count -= 1;
                    }
                }

                return _tailVisits
                    .Select(v => v.ToString())
                    .Distinct()
                    .Count();
            }

            private void UpdateTailPosition()
            {
                if (_headPosition.X > _tailPosition.X + 1)
                {
                    if (_headPosition.Y == _tailPosition.Y)
                    {
                        _tailPosition = _tailPosition.Add(new Coordinate2D(1, 0));
                        return;
                    }

                    if (_headPosition.Y > _tailPosition.Y)
                    {
                        _tailPosition = _tailPosition.Add(new Coordinate2D(1, 1));
                        return;
                    }

                    if (_headPosition.Y < _tailPosition.Y)
                    {
                        _tailPosition = _tailPosition.Add(new Coordinate2D(1, -1));
                        return;
                    }
                }


                if (_headPosition.X < _tailPosition.X - 1)
                {
                    if (_headPosition.Y == _tailPosition.Y)
                    {
                        _tailPosition = _tailPosition.Add(new Coordinate2D(-1, 0));
                        return;
                    }

                    if (_headPosition.Y > _tailPosition.Y)
                    {
                        _tailPosition = _tailPosition.Add(new Coordinate2D(-1, 1));
                        return;
                    }

                    if (_headPosition.Y < _tailPosition.Y)
                    {
                        _tailPosition = _tailPosition.Add(new Coordinate2D(-1, -1));
                        return;
                    }
                }


                if (_headPosition.Y > _tailPosition.Y + 1)
                {
                    if (_headPosition.X == _tailPosition.X)
                    {
                        _tailPosition = _tailPosition.Add(new Coordinate2D(0, 1));
                        return;
                    }

                    if (_headPosition.X > _tailPosition.X)
                    {
                        _tailPosition = _tailPosition.Add(new Coordinate2D(1, 1));
                        return;
                    }

                    if (_headPosition.X < _tailPosition.X)
                    {
                        _tailPosition = _tailPosition.Add(new Coordinate2D(-1, 1));
                        return;
                    }
                }


                if (_headPosition.Y < _tailPosition.Y - 1)
                {
                    if (_headPosition.X == _tailPosition.X)
                    {
                        _tailPosition = _tailPosition.Add(new Coordinate2D(0, -1));
                        return;
                    }

                    if (_headPosition.X > _tailPosition.X)
                    {
                        _tailPosition = _tailPosition.Add(new Coordinate2D(1, -1));
                        return;
                    }

                    if (_headPosition.X < _tailPosition.X)
                    {
                        _tailPosition = _tailPosition.Add(new Coordinate2D(-1, -1));
                        return;
                    }
                }
            }
        }

        private enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        private class Instruction
        {
            public Direction Direction { get; }
            public int Distance { get; }

            public Instruction(string instruction)
            {
                var split = instruction.Split(" ");
                Direction = ParseDirection(split[0]);
                Distance = int.Parse(split[1]);
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

                throw new Exception("Unrecognised direction");
            }
        }
    }
}


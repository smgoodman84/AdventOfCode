using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2022.Day09
{
    public class Day09 : Day
    {
        public Day09() : base(2022, 9, "Day09/input_2022_09.txt", "5619", "2376")
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
            var tailTracker = new TailTracker(_instructions, 2);
            var result = tailTracker.CountTailVisits();
            return result.ToString();
        }

        public override string Part2()
        {
            var tailTracker = new TailTracker(_instructions, 10);
            var result = tailTracker.CountTailVisits();
            return result.ToString();
        }

        private class TailTracker
        {
            private readonly List<Instruction> _instructions;

            private readonly Coordinate2D[] _rope;
            private readonly List<Coordinate2D> _tailVisits;

            private readonly Dictionary<Direction, Coordinate2D> _directionMoves
                = new Dictionary<Direction, Coordinate2D>
                {
                    { Direction.Up, new Coordinate2D(0, -1) },
                    { Direction.Down, new Coordinate2D(0, 1) },
                    { Direction.Left, new Coordinate2D(-1, 0) },
                    { Direction.Right, new Coordinate2D(1, 0) },
                };

            public TailTracker(IEnumerable<Instruction> instructions, int ropeLength)
            {
                _instructions = instructions.ToList();

                _rope = Enumerable.Range(0, ropeLength)
                    .Select(x => Coordinate2D.Origin)
                    .ToArray();

                _tailVisits = new List<Coordinate2D>()
                {
                    TailPosition
                };
            }

            private Coordinate2D TailPosition => _rope.Last();

            public int CountTailVisits()
            {
                foreach (var instruction in _instructions)
                {
                    var count = instruction.Distance;
                    while (count > 0)
                    {
                        _rope[0] = _rope[0].Add(_directionMoves[instruction.Direction]);

                        for (var tailIndex = 1; tailIndex < _rope.Length; tailIndex += 1)
                        {
                            UpdateTailPosition(tailIndex);
                        }

                        _tailVisits.Add(TailPosition);
                        count -= 1;
                    }
                }

                return _tailVisits
                    .Select(v => v.ToString())
                    .Distinct()
                    .Count();
            }

            private void UpdateTailPosition(int tailIndex)
            {
                var headIndex = tailIndex - 1;

                if (_rope[headIndex].X > _rope[tailIndex].X + 1)
                {
                    if (_rope[headIndex].Y == _rope[tailIndex].Y)
                    {
                        _rope[tailIndex] = _rope[tailIndex].Add(new Coordinate2D(1, 0));
                        return;
                    }

                    if (_rope[headIndex].Y > _rope[tailIndex].Y)
                    {
                        _rope[tailIndex] = _rope[tailIndex].Add(new Coordinate2D(1, 1));
                        return;
                    }

                    if (_rope[headIndex].Y < _rope[tailIndex].Y)
                    {
                        _rope[tailIndex] = _rope[tailIndex].Add(new Coordinate2D(1, -1));
                        return;
                    }
                }


                if (_rope[headIndex].X < _rope[tailIndex].X - 1)
                {
                    if (_rope[headIndex].Y == _rope[tailIndex].Y)
                    {
                        _rope[tailIndex] = _rope[tailIndex].Add(new Coordinate2D(-1, 0));
                        return;
                    }

                    if (_rope[headIndex].Y > _rope[tailIndex].Y)
                    {
                        _rope[tailIndex] = _rope[tailIndex].Add(new Coordinate2D(-1, 1));
                        return;
                    }

                    if (_rope[headIndex].Y < _rope[tailIndex].Y)
                    {
                        _rope[tailIndex] = _rope[tailIndex].Add(new Coordinate2D(-1, -1));
                        return;
                    }
                }


                if (_rope[headIndex].Y > _rope[tailIndex].Y + 1)
                {
                    if (_rope[headIndex].X == _rope[tailIndex].X)
                    {
                        _rope[tailIndex] = _rope[tailIndex].Add(new Coordinate2D(0, 1));
                        return;
                    }

                    if (_rope[headIndex].X > _rope[tailIndex].X)
                    {
                        _rope[tailIndex] = _rope[tailIndex].Add(new Coordinate2D(1, 1));
                        return;
                    }

                    if (_rope[headIndex].X < _rope[tailIndex].X)
                    {
                        _rope[tailIndex] = _rope[tailIndex].Add(new Coordinate2D(-1, 1));
                        return;
                    }
                }


                if (_rope[headIndex].Y < _rope[tailIndex].Y - 1)
                {
                    if (_rope[headIndex].X == _rope[tailIndex].X)
                    {
                        _rope[tailIndex] = _rope[tailIndex].Add(new Coordinate2D(0, -1));
                        return;
                    }

                    if (_rope[headIndex].X > _rope[tailIndex].X)
                    {
                        _rope[tailIndex] = _rope[tailIndex].Add(new Coordinate2D(1, -1));
                        return;
                    }

                    if (_rope[headIndex].X < _rope[tailIndex].X)
                    {
                        _rope[tailIndex] = _rope[tailIndex].Add(new Coordinate2D(-1, -1));
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


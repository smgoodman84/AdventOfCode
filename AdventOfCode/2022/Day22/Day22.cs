using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;
using static System.Net.Mime.MediaTypeNames;

namespace AdventOfCode._2022.Day22
{
    public class Day22 : Day
    {
        public Day22() : base(2022, 22, "Day22/input_2022_22.txt", "50412", "")
        {

        }

        private List<IInstruction> _instructions;
        private Grid2D<Tile> _map;
        private Bearing _bearing;
        private Coordinate2D _initialPosition;
        private Coordinate2D _position;

        public override void Initialise()
        {
            var instructions = InputLines.Last();
            _instructions = ParseInstructions(instructions);

            var mapLines = InputLines.Take(InputLines.Count - 2).ToList();

            var height = mapLines.Count;
            var width = mapLines.Max(l => l.Length);

            _map = new Grid2D<Tile>(width, height);

            var setPosition = false;

            var y = 0;
            foreach (var line in mapLines)
            {
                var x = 0;
                foreach (var c in line)
                {
                    if (c == '.')
                    {
                        if (!setPosition && y == 0)
                        {
                            _initialPosition = new Coordinate2D(x, y);
                            _position = _initialPosition;
                            setPosition = true;
                        }

                        _map.Write(x, y, Tile.Open);
                    }

                    if (c == '#')
                    {
                        _map.Write(x, y, Tile.Wall);
                    }

                    x += 1;
                }

                y += 1;
            }

            _bearing = Bearing.Right;
        }

        public override string Part1()
        {
            foreach (var instruction in _instructions)
            {
                var turn = instruction as Turn;
                var move = instruction as Move;
                if (turn != null)
                {
                    if (turn.Direction == Direction.Left)
                    {
                        _bearing = BearLeft(_bearing);
                    }
                    if (turn.Direction == Direction.Right)
                    {
                        _bearing = BearRight(_bearing);
                    }
                }

                if (move != null)
                {
                    var moved = 0;
                    var hitWall = false;
                    while (moved < move.Distance && !hitWall)
                    {
                        var nextCoordinate = GetNextNonVoid();
                        var nextValue = _map.Read(nextCoordinate);
                        if (nextValue == Tile.Wall)
                        {
                            hitWall = true;
                        }
                        else if (nextValue == Tile.Open)
                        {
                            _position = nextCoordinate;
                            moved += 1;
                        }
                    }
                }
            }

            var result = (1000 * (_position.Y + 1))
                + (4 * (_position.X + 1))
                + BearingValue(_bearing);

            return result.ToString();
        }

        public override string Part2()
        {
            _bearing = Bearing.Right;

            var totalSpace = _map.ReadAll().Count(x => x != Tile.Void);
            var spacePerFace = totalSpace / 6;
            var faceLength = (int)Math.Sqrt(spacePerFace);

            return "";
        }

        private class CubeNetMap
        {

        }


        private int BearingValue(Bearing bearing)
        {
            switch (bearing)
            {
                case Bearing.Right: return 0;
                case Bearing.Down: return 1;
                case Bearing.Left: return 2;
                case Bearing.Up: return 3;
            }

            throw new Exception("Invalid bearing");
        }

        private Coordinate2D GetNextNonVoid()
        {
            var position = _position;

            try
            {
                do
                {
                    switch (_bearing)
                    {
                        case Bearing.Up:
                            position = position.Down(); // Inverse coordinates
                            break;
                        case Bearing.Down:
                            position = position.Up(); // Inverse coordinates
                            break;
                        case Bearing.Left:
                            position = position.Left();
                            break;
                        case Bearing.Right:
                            position = position.Right();
                            break;
                    }

                    if (!_map.IsInGrid(position))
                    {
                        position = new Coordinate2D(
                            (position.X + _map.Width) % _map.Width,
                            (position.Y + _map.Height) % _map.Height);
                    }
                }
                while (_map.Read(position) == Tile.Void);
            }
            catch(Exception ex)
            {
                var stop = "here";
            }

            return position;
        }

        private enum Tile
        {
            Void,
            Wall,
            Open
        }

        private List<IInstruction> ParseInstructions(string instructions)
        {
            var result = new List<IInstruction>();
            string current = string.Empty;
            foreach (var c in instructions)
            {
                if (c == 'L')
                {
                    result.Add(new Move(int.Parse(current)));
                    current = string.Empty;
                    result.Add(new Turn(Direction.Left));
                }
                else if (c == 'R')
                {
                    result.Add(new Move(int.Parse(current)));
                    current = string.Empty;
                    result.Add(new Turn(Direction.Right));
                }
                else
                {
                    current = $"{current}{c}";
                }
            }

            result.Add(new Move(int.Parse(current)));

            return result;
        }

        private interface IInstruction
        {

        }

        private class Turn : IInstruction
        {
            public Direction Direction { get; }

            public Turn(Direction direction)
            {
                Direction = direction;
            }
        }

        private class Move : IInstruction
        {
            public int Distance { get; }

            public Move(int distance)
            {
                Distance = distance;
            }
        }

        private enum Direction
        {
            Left,
            Right
        }

        private enum Bearing
        {
            Up,
            Right,
            Down,
            Left
        }

        private Bearing BearLeft(Bearing bearing)
        {
            switch(bearing)
            {
                case Bearing.Up: return Bearing.Left;
                case Bearing.Left: return Bearing.Down;
                case Bearing.Down: return Bearing.Right;
                case Bearing.Right: return Bearing.Up;
            }

            throw new Exception("Invalid bearing");
        }

        private Bearing BearRight(Bearing bearing)
        {
            switch (bearing)
            {
                case Bearing.Up: return Bearing.Right;
                case Bearing.Right: return Bearing.Down;
                case Bearing.Down: return Bearing.Left;
                case Bearing.Left: return Bearing.Up;
            }

            throw new Exception("Invalid bearing");
        }
    }
}

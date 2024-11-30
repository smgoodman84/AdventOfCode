using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2021.Day02;

public class Day02 : Day
{
    private List<Movement> _movements;
    public Day02() : base(2021, 2, "Day02/input_2021_02.txt", "1990000", "1975421260")
    {
    }

    public override void Initialise()
    {
        _movements = InputLines
            .Select(l => new Movement(l))
            .ToList();
    }

    public override string Part1()
    {
        var depth = _movements.Sum(m => m.DepthEffect);
        var distance = _movements.Sum(m => m.ForwardEffect);
        return (depth * distance).ToString();
    }

    public override string Part2()
    {
        var aim = 0.0;
        var distance = 0.0;
        var depth = 0.0;

        foreach (var movement in _movements)
        {
            switch(movement.Direction)
            {
                case Direction.Forward:
                    distance += movement.Distance;
                    depth += aim * movement.Distance;
                    break;
                case Direction.Down:
                    aim += movement.Distance;
                    break;
                case Direction.Up:
                    aim -= movement.Distance;
                    break;
            }
        }

        return (depth * distance).ToString();
    }

    private class Movement
    {
        public Direction Direction { get; }
        public int Distance { get; } = 0;
        public int DepthEffect { get; } = 0;
        public int ForwardEffect { get; } = 0;

        public Movement(string input)
        {
            var elements = input.Split(' ');

            Distance = int.Parse(elements[1]);

            switch (elements[0].ToLower())
            {
                case "forward":
                    Direction = Direction.Forward;
                    ForwardEffect = Distance;
                    break;
                case "down":
                    Direction = Direction.Down;
                    DepthEffect = Distance;
                    break;
                case "up":
                    Direction = Direction.Up;
                    DepthEffect = -Distance;
                    break;
            }
        }
    }

    private enum Direction
    {
        Forward,
        Down,
        Up
    }
}
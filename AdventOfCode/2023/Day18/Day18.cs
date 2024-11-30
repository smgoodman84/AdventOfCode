using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2023.Day18;

public class Day18 : Day
{
    public Day18() : base(2023, 18, "Day18/input_2023_18.txt", "47045", "147839570293376", true)
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
        var result = CalculateArea(ParseLineSegmentPart1);

        return result.ToString();
    }

    public override string Part2()
    {
        var result = CalculateArea(ParseLineSegmentPart2);

        return result.ToString();
    }

    private long CalculateArea(Func<Coordinate2D, Instruction, (LineSegment LineSegment, int Distance)> parseLineSegment)
    {
        var current = Coordinate2D.Origin;

        var lineSegments = new List<LineSegment>();
        long lineArea = 0;
        foreach (var instruction in _instructions)
        {
            (var lineSegment, var lineDistance) = parseLineSegment(current, instruction);
            lineSegments.Add(lineSegment);
            lineArea += lineDistance;
            current = lineSegment.End;
        }

        var polygon = new Polygon(lineSegments);

        var area = polygon.CalculateArea();
        var longArea = (long)area;

        var totalArea = longArea + (lineArea / 2) + 1;

        return totalArea;
    }

    private (LineSegment LineSegment, int Distance) ParseLineSegmentPart1(Coordinate2D start, Instruction instruction)
    {
        var distance = instruction.Distance;
        var direction = instruction.Direction;

        switch (direction)
        {
            case Direction.Right: // Right
                return (new LineSegment(start, new Coordinate2D(start.X + distance, start.Y)), distance);
            case Direction.Down: // Down
                return (new LineSegment(start, new Coordinate2D(start.X, start.Y - distance)), distance);
            case Direction.Left: // Left
                return (new LineSegment(start, new Coordinate2D(start.X - distance, start.Y)), distance);
            case Direction.Up: // Up
                return (new LineSegment(start, new Coordinate2D(start.X, start.Y + distance)), distance);
        }

        throw new Exception($"Unexpected direction {direction}");
    }

    private (LineSegment LineSegment, int Distance) ParseLineSegmentPart2(Coordinate2D start, Instruction instruction)
    {
        var hexDistance = instruction.Colour.Substring(1, 5);
        var distance = ParseHex(hexDistance);
        var direction = instruction.Colour[6];

        switch (direction)
        {
            case '0': // Right
                return (new LineSegment(start, new Coordinate2D(start.X + distance, start.Y)), distance);
            case '1': // Down
                return (new LineSegment(start, new Coordinate2D(start.X, start.Y - distance)), distance);
            case '2': // Left
                return (new LineSegment(start, new Coordinate2D(start.X - distance, start.Y)), distance);
            case '3': // Up
                return (new LineSegment(start, new Coordinate2D(start.X, start.Y + distance)), distance);
        }

        throw new Exception($"Unexpected direction {direction}");
    }

    private int ParseHex(string input)
    {
        var result = 0;
        foreach(var c in input)
        {
            result *= 16;
            result += ParseHex(c);
        }

        return result;
    }

    private int ParseHex(char input)
    {
        if (input >= '0' && input <= '9')
        {
            return input - '0';
        }

        if (input >= 'a' && input <= 'f')
        {
            return input - 'a' + 10;
        }

        if (input >= 'A' && input <= 'F')
        {
            return input - 'A' + 10;
        }

        throw new Exception($"{input} is not a valid hex character");
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
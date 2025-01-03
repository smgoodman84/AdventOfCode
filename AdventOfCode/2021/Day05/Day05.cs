﻿using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2021.Day05;

public class Day05 : Day
{
    private List<Line> _lines;
    public Day05() : base(2021, 5, "Day05/input_2021_05.txt", "6267", "20196")
    {
    }

    public override void Initialise()
    {
        _lines = InputLines
            .Select(l => new Line(l))
            .ToList();
    }

    public override string Part1()
    {
        var lines = _lines
            .Where(l => l.IsHorizontal || l.IsVertical)
            .ToList();

        var allPoints = lines
            .SelectMany(l => l.GetPoints())
            .ToList();

        var counts = allPoints
            .GroupBy(c => c.ToString())
            .ToList();

        var result = counts
            .Where(g => g.Count() >= 2)
            .Count();

        return result.ToString();
    }

    public override string Part2()
    {
        var allPoints = _lines
            .SelectMany(l => l.GetPoints())
            .ToList();

        var counts = allPoints
            .GroupBy(c => c.ToString())
            .ToList();

        var result = counts
            .Where(g => g.Count() >= 2)
            .Count();

        return result.ToString();
    }

    private class Line
    {
        public Coordinate2D Start { get; private set; }
        public Coordinate2D End { get; private set; }

        public Line(string lineDefinition)
        {
            var coordinates = lineDefinition.Split(" -> ");
            Start = new Coordinate2D(coordinates[0]);
            End = new Coordinate2D(coordinates[1]);
        }

        public bool IsHorizontal => Start.X == End.X;
        public bool IsVertical => Start.Y == End.Y;

        public IEnumerable<Coordinate2D> GetPoints()
        {
            var dX = 0;
            if (Start.X > End.X)
            {
                dX = -1;
            }
            if (Start.X < End.X)
            {
                dX = 1;
            }

            var dY = 0;
            if (Start.Y > End.Y)
            {
                dY = -1;
            }
            if (Start.Y < End.Y)
            {
                dY = 1;
            }

            var currentX = Start.X;
            var currentY = Start.Y;

            yield return new Coordinate2D(currentX, currentY);
            while (currentX != End.X || currentY != End.Y)
            {
                currentX += dX;
                currentY += dY;
                yield return new Coordinate2D(currentX, currentY);
            }
        }


        public override string ToString()
        {
            var direction = "Unknown";
            if (IsHorizontal)
            {
                direction = "Horizontal";
            }
            if (IsVertical)
            {
                direction = "Vertical";
            }
            return $"{Start} -> {End} [{direction}]";
        }
    }
}
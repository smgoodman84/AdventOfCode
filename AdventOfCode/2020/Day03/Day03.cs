using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2020.Day03;

public class Day03 : Day
{
    public Day03() : base(2020, 3, "Day03/input_2020_03.txt", "203", "3316272960")
    {

    }

    private const char OpenSquare = '.';
    private const char Tree = '#';

    private MapLine[] _lines;

    public override void Initialise()
    {
        _lines = InputLines
            .Select(ParseLine)
            .ToArray();
    }

    private static MapLine ParseLine(string line)
    {
        return new MapLine(line.ToCharArray());
    }

    private char GetLocation(int x, int y)
    {
        var line = _lines[y];
        var location = line.GetLocation(x);
        return location;
    }

    public int CountTrees(int xStep, int yStep)
    {
        var x = 0;
        var y = 0;
        var treeCount = 0;

        while (y < _lines.Length)
        {
            if (GetLocation(x, y) == Tree)
            {
                treeCount += 1;
            }

            x += xStep;
            y += yStep;
        }

        return treeCount;
    }

    public override string Part1() => CountTrees(3, 1).ToString();

    public override string Part2()
    {
        var treeCounts = new List<decimal>
        {
            CountTrees(1, 1),
            CountTrees(3, 1),
            CountTrees(5, 1),
            CountTrees(7, 1),
            CountTrees(1, 2)
        };

        decimal result = 1;
        foreach (var treeCount in treeCounts)
        {
            result *= treeCount;
        }

        return result.ToString();
    }

    private class MapLine
    {
        private char[] _line;
        public MapLine(char[] line)
        {
            _line = line;
        }

        public char GetLocation(int location)
        {
            var x = location % _line.Length;
            return _line[x];
        }
    }
}
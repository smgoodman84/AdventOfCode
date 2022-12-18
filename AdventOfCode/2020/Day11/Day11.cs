using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2020.Day11
{
    public class Day11 : Day
    {
        public Day11() : base(2020, 11, "Day11/input_2020_11.txt", "2468", "2214")
        {

        }

        private char[][] _initialLayout;
        public override void Initialise()
        {
            _initialLayout = InputLines
                .Select(lines => lines.ToCharArray())
                .ToArray();
        }

        public override string Part1() => FindOccupiedSeatEquilibrium(4, CountOccupiedNeighbours).ToString();

        public override string Part2() => FindOccupiedSeatEquilibrium(5, CountVisibleNeighbours).ToString();


        private int FindOccupiedSeatEquilibrium(int maxTolerableNeighbours, Func<char[][], int, int, int> neighbourCounter)
        {
            var currentGeneration = new Generation
            {
                Layout = _initialLayout
            };

            do
            {
                currentGeneration = GetNextGeneration(currentGeneration, maxTolerableNeighbours, neighbourCounter);
                // Trace(currentGeneration);
            }
            while (currentGeneration.AnyChange);

            return currentGeneration.Layout
                .Sum(row => row.Count(x => x == '#'));
        }

        private Generation GetNextGeneration(Generation currentGeneration, int maxTolerableNeighbours, Func<char[][], int, int, int> neighbourCounter)
        {
            var layout = currentGeneration.Layout;

            var height = layout.Length;
            var width = layout[0].Length;
            var anyChange = false;

            var nextLayout = new char[height][];
            for(var y=0; y < height; y++)
            {
                nextLayout[y] = new char[width];
                for (var x = 0; x < width; x++)
                {
                    var currentState = layout[y][x];
                    var count = neighbourCounter(layout, x, y);
                    var nextState = '.';
                    switch (currentState)
                    {
                        case '#':
                            nextState = count >= maxTolerableNeighbours ? 'L' : '#';
                            break;
                        case 'L':
                            nextState = count == 0 ? '#' : 'L';
                            break;
                    }

                    nextLayout[y][x] = nextState;
                    if (nextState != currentState)
                    {
                        anyChange = true;
                    }
                }
            }

            return new Generation
            {
                Layout = nextLayout,
                AnyChange = anyChange,
                PreviousGeneration = currentGeneration
            };
        }

        private int CountOccupiedNeighbours(char[][] layout, int x, int y)
        {
            return new List<bool>
            {
                IsOccupied(layout, x-1, y-1),
                IsOccupied(layout, x-1, y),
                IsOccupied(layout, x-1, y+1),
                IsOccupied(layout, x, y-1),
                IsOccupied(layout, x, y+1),
                IsOccupied(layout, x+1, y-1),
                IsOccupied(layout, x+1, y),
                IsOccupied(layout, x+1, y+1),
            }.Count(b => b);
        }


        private int CountVisibleNeighbours(char[][] layout, int x, int y)
        {
            return new List<bool>
            {
                IsVisibleOccupied(layout, x, y, -1, -1),
                IsVisibleOccupied(layout, x, y, -1, 0),
                IsVisibleOccupied(layout, x, y, -1, +1),
                IsVisibleOccupied(layout, x, y, 0, -1),
                IsVisibleOccupied(layout, x, y, 0, +1),
                IsVisibleOccupied(layout, x, y, +1, -1),
                IsVisibleOccupied(layout, x, y, +1, 0),
                IsVisibleOccupied(layout, x, y, +1, +1),
            }.Count(b => b);
        }

        private bool IsVisibleOccupied(char[][] layout, int x, int y, int dx, int dy)
        {
            while (true)
            {
                x += dx;
                y += dy;


                if (y < 0 || y >= layout.Length)
                {
                    return false;
                }

                if (x < 0 || x >= layout[0].Length)
                {
                    return false;
                }

                var c = layout[y][x];
                switch (c)
                {
                    case 'L': return false;
                    case '#': return true;
                }
            }
        }

        private bool IsOccupied(char[][] layout, int x, int y)
        {
            if (y < 0 || y >= layout.Length)
            {
                return false;
            }

            if (x < 0 || x >= layout[0].Length)
            {
                return false;
            }

            var c = layout[y][x];

            return c == '#';
        }

        private class Generation
        {
            public char[][] Layout { get; set; }
            public bool AnyChange { get; set; }
            public Generation PreviousGeneration { get; set; }
            public int GenerationCount => PreviousGeneration == null ? 0 : PreviousGeneration.GenerationCount + 1;

            public override string ToString()
            {
                var layout = string.Join(Environment.NewLine, Layout.Select(row => string.Join("", row)));
                return $"Generation {GenerationCount}:{Environment.NewLine}{layout}";
            }
        }
    }
}

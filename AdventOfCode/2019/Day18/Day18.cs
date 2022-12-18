using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2019.Day18
{
    public class Day18 : Day
    {
        public Day18() : base(2019, 18, "Day18/input_2019_18.txt", "", "")
        {

        }

        private char[][] _vault;

        public override void Initialise()
        {
            _vault = InputLines
                .Select(l => l.ToCharArray())
                .ToArray();
        }

        public override string Part1()
        {
            return "";
        }

        public override string Part2()
        {
            return "";
        }

        public int GetShortestPath()
        {
            return 0;
        }

        private class Context
        {
            public List<char> keys = new List<char>();
            public Point location;
            public Point previousLocation;
        }
    }
}

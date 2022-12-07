using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
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
            List<char> keys = new List<char>();
            Point location;
            Point previousLocation;
        }
    }
}

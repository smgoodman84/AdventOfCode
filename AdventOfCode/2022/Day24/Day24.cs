using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Shared;
using AdventOfCode.Shared.DataStructures;
using AdventOfCode.Shared.Geometry;
using static System.Net.Mime.MediaTypeNames;

namespace AdventOfCode._2022.Day24
{
    public class Day24 : Day
    {
        public Day24() : base(2022, 24, "Day24/input_2022_24.txt", "", "")
        {

        }

        public override void Initialise()
        {
        }

        public override string Part1()
        {
            return "";
        }

        public override string Part2()
        {
            return "";
        }

        private class Map
        {
            public Coordinate2D Location { get; }
        }

        private class Blizzard
        {
            public Direction Direction { get; }
            public Coordinate2D Location { get; }

            public Blizzard(int x, int y, Direction direction)
            {
                Direction = direction;
                Location = new Coordinate2D(x, y);
            }


        }
    }
}

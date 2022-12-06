using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;
using AdventOfCode.Shared.PrimitiveExtensions;

namespace AdventOfCode._2022.Day02
{
    public class Day06 : Day
    {
        public Day06() : base(2022, 6, "Day06/input_2022_06.txt", "1816", "2625")
        {

        }

        private string _signal;
        public override void Initialise()
        {
            _signal = InputLines.Single();
        }

        public override string Part1()
        {
            for (var index = 4; index <= _signal.Length; index++)
            {
                var lastFour = _signal.Substring(index - 4, 4);

                if (lastFour.ToCharArray().Distinct().Count() == 4)
                {
                    return index.ToString();
                }
            }

            return "";
        }

        public override string Part2()
        {
            for (var index = 14; index <= _signal.Length; index++)
            {
                var lastFour = _signal.Substring(index - 14, 14);

                if (lastFour.ToCharArray().Distinct().Count() == 14)
                {
                    return index.ToString();
                }
            }

            return "";
        }

        private class Move
        {
            public int Count { get; set; }
            public int Source { get; set; }
            public int Destination { get; set; }

            public Move(string move)
            {
                var split = move.Split(" ");
                Count = int.Parse(split[1]);
                Source = int.Parse(split[3]);
                Destination = int.Parse(split[5]);
            }
        }
    }
}


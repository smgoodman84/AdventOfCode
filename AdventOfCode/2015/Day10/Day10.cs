using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2015.Day10
{
    public class Day10 : Day
    {
        public Day10() : base(2015, 10, "Day10/input_2015_10.txt", "492982", "")
        {
        }

        public override string Part1()
        {
            var value = InputLines.Single();

            for(var i = 0; i < 40; i++)
            {
                value = Iterate(value);
                // Console.WriteLine(value);
            }

            return value.Length.ToString();
        }

        public override string Part2()
        {
            return "";
        }

        private string Iterate(string value)
        {
            var index = 0;
            var currentChar = ' ';
            int runLength = 0;
            var output = new StringBuilder();

            foreach(var c in value)
            {
                if (c == currentChar)
                {
                    runLength += 1;
                }
                else
                {
                    if (currentChar != ' ')
                    {
                        output.Append($"{runLength}{currentChar}");
                    }

                    currentChar = c;
                    runLength = 1;
                }
            }

            output.Append($"{runLength}{currentChar}");
            return output.ToString();
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2015.Day08
{
    public class Day08 : Day
    {
        public Day08() : base(2015, 8, "Day08/input_2015_08.txt", "1371", "")
        {
        }

        private List<StringParser> _strings = new List<StringParser>();

        public override void Initialise()
        {
            _strings = InputLines
                .Select(l => new StringParser(l.Trim()))
                .ToList();
        }

        public override string Part1()
        {
            var result = _strings
                .Sum(s => s.LengthDifference);

            return result.ToString();
        }

        public override string Part2()
        {
            return "";
        }

        private class StringParser
        {
            private readonly string _input;

            public int StringLength { get; }
            public int MemoryLength { get; }
            public int LengthDifference => StringLength - MemoryLength;

            public StringParser(string input)
            {
                _input = input;

                StringLength = input.Length;

                var withoutQuotes = input.Substring(1, input.Length - 2);
                var memoryLength = 0;
                var inEscape = false;
                var remainingHexEscapes = 0;
                foreach (var c in withoutQuotes)
                {
                    if (!inEscape)
                    {
                        if (c == '\\')
                        {
                            inEscape = true;
                        }

                        memoryLength += 1;
                    }
                    else
                    {
                        if (remainingHexEscapes > 0)
                        {
                            remainingHexEscapes -= 1;
                            inEscape = remainingHexEscapes > 0;
                        }
                        else if (c == 'x')
                        {
                            remainingHexEscapes = 2;
                        }
                        else
                        {
                            inEscape = false;
                        }
                    }
                }
                MemoryLength = memoryLength;
            }
        }
    }
}
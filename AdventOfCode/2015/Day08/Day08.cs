using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2015.Day08
{
    public class Day08 : Day
    {
        public Day08() : base(2015, 8, "Day08/input_2015_08.txt", "1371", "2117")
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
            var result = _strings
                .Sum(s => s.EncodedDifference);

            return result.ToString();
        }

        private class StringParser
        {
            public int StringLength { get; }
            public int MemoryLength { get; }
            public int EncodedLength { get; }
            public int LengthDifference => StringLength - MemoryLength;
            public int EncodedDifference => EncodedLength - StringLength;

            public StringParser(string input)
            {
                StringLength = input.Length;
                MemoryLength = GetMemoryLength(input);
                EncodedLength = GetEncodedLength(input);
            }

            private static int GetEncodedLength(string input)
            {
                var encodedLength = 2; // Surrounding quotes
                foreach (var c in input)
                {
                    if (c == '\\')
                    {
                        encodedLength += 2;
                    }
                    else if (c == '"')
                    {
                        encodedLength += 2;
                    }
                    else
                    {
                        encodedLength += 1;
                    }
                }
                return encodedLength;
            }

            private static int GetMemoryLength(string input)
            {
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

                return memoryLength;
            }
        }
    }
}
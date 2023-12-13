using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2023.Day13
{
    public class Day13 : Day
    {
        public Day13() : base(2023, 13, "Day13/input_2023_13.txt", "", "", true)
        {

        }

        private List<Pattern> _patterns;
        public override void Initialise()
        {
            var patterns = LineGrouper.GroupLinesBySeperator(InputLines);

            _patterns = patterns
                .Select(p => new Pattern(p))
                .ToList();
        }

        public override string Part1()
        {
            var reflectionValues = _patterns
                .Select(p => p.GetReflectionValue())
                .ToList();


            return reflectionValues.Sum().ToString();
        } 


        public override string Part2()
        {
            return string.Empty;
        }

        private class Pattern
        {
            private Grid2D<char> _map;
            public Pattern(List<string> lines)
            {
                var width = lines[0].Length;
                var height = lines.Count;
                _map = new Grid2D<char>(width, height);
                for (var y = 0; y < height; y += 1)
                {
                    for (var x = 0; x < width; x += 1)
                    {
                        _map.Write(x, y, lines[y][x]);
                    }
                }
            }

            public int GetReflectionValue()
            {
                for (var reflectionIndex = 1; reflectionIndex < _map.Width; reflectionIndex += 1)
                {
                    if (ReflectsVertically(reflectionIndex))
                    {
                        return reflectionIndex;
                    }
                }

                for (var reflectionIndex = 1; reflectionIndex < _map.Height; reflectionIndex += 1)
                {
                    if (ReflectsHorizontally(reflectionIndex))
                    {
                        return reflectionIndex * 100;
                    }
                }

                return 0;
            }

            public bool ReflectsVertically(int reflectionIndex)
            {
                foreach(var y in _map.YIndexes())
                {
                    var leftIndex = reflectionIndex - 1;
                    var rightIndex = reflectionIndex;

                    while (leftIndex >= _map.MinX && rightIndex <= _map.MaxX)
                    {
                        if (_map.Read(leftIndex, y) != _map.Read(rightIndex, y))
                        {
                            return false;
                        }

                        leftIndex -= 1;
                        rightIndex += 1;
                    }
                }

                return true;
            }

            public bool ReflectsHorizontally(int reflectionIndex)
            {
                foreach (var x in _map.XIndexes())
                {
                    var topIndex = reflectionIndex - 1;
                    var bottomIndex = reflectionIndex;

                    while (topIndex >= _map.MinY && bottomIndex <= _map.MaxY)
                    {
                        if (_map.Read(x, topIndex) != _map.Read(x, bottomIndex))
                        {
                            return false;
                        }

                        topIndex -= 1;
                        bottomIndex += 1;
                    }
                }

                return true;
            }
        }
    }
}

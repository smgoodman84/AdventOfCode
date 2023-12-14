using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2023.Day14
{
    public class Day14 : Day
    {
        private const char Rock = 'O';
        private const char Space = '.';
        public Day14() : base(2023, 14, "Day14/input_2023_14.txt", "108840", "", true)
        {

        }

        private Grid2D<char> _dish;
        public override void Initialise()
        {
            _dish = new Grid2D<char>(InputLines[0].Length, InputLines.Count);
            foreach (var y in _dish.YIndexes())
            {
                foreach (var x in _dish.XIndexes())
                {
                    _dish.Write(x, y, InputLines[(int)y][(int)x]);
                }
            }
        }

        public override string Part1()
        {
            var rolledNorth = RollNorth();

            var load = _dish.Height;
            var totalLoad = 0L;
            foreach (var y in rolledNorth.YIndexes())
            {
                foreach (var x in rolledNorth.XIndexes())
                {
                    var current = rolledNorth.Read(x, y);
                    if (current == Rock)
                    {
                        totalLoad += load;
                    }
                }

                load -= 1;
            }

            return totalLoad.ToString();
        }

        public override string Part2()
        {
            return string.Empty;
        }

        private Grid2D<char> RollNorth()
        {
            var rolledNorth = _dish.Clone();
            foreach (var x in rolledNorth.XIndexes())
            {
                foreach (var startY in rolledNorth.YIndexes())
                {
                    var current = rolledNorth.Read(x, startY);
                    if (current == Rock)
                    {
                        var newY = startY;
                        var rolling = true;
                        while (newY - 1 >= rolledNorth.MinY && rolling)
                        {
                            var north = rolledNorth.Read(x, newY - 1);
                            if (north == Space)
                            {
                                newY -= 1;
                            }
                            else
                            {
                                rolling = false;
                            }
                        }

                        if (newY != startY)
                        {
                            rolledNorth.Write(x, newY, Rock);
                            rolledNorth.Write(x, startY, Space);
                        }
                    }
                }
            }

            return rolledNorth;
        }
    }
}

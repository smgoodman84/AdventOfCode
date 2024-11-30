using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2023.Day14;

public class Day14 : Day
{
    private const char Rock = 'O';
    private const char Space = '.';
    public Day14() : base(2023, 14, "Day14/input_2023_14.txt", "108840", "103445", false)
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
        var rolledNorth = RollNorth(_dish);

        long load = CalculateLoad(rolledNorth);

        return load.ToString();
    }

    private static long CalculateLoad(Grid2D<char> dish)
    {
        var load = dish.Height;
        var totalLoad = 0L;
        foreach (var y in dish.YIndexes())
        {
            foreach (var x in dish.XIndexes())
            {
                var current = dish.Read(x, y);
                if (current == Rock)
                {
                    totalLoad += load;
                }
            }

            load -= 1;
        }

        return totalLoad;
    }

    public override string Part2()
    {
        var totalCycles = 1000000000;
        var skipped = false;
        var cycleNumberCache = new Dictionary<string, int>();
        var cycleCache = new Dictionary<int, Grid2D<char>>();
        var current = _dish;

        var description = ToString(current);
        cycleNumberCache.Add(description, 0);
        cycleCache.Add(0, current);
        for (var cycle = 1; cycle <= totalCycles; cycle += 1)
        {
            current = Cycle(current);

            if (skipped)
            {
                continue;
            }

            description = ToString(current);

            if (cycleNumberCache.ContainsKey(description))
            {
                var previousInstance = cycleNumberCache[description];
                TraceLine($"Found loop {previousInstance}..{cycle}");
                var loopLength = cycle - previousInstance;
                var loopsFromFirstInstance = (totalCycles - previousInstance) / loopLength;

                TraceLine($"Loop length {loopLength}");
                TraceLine($"Total loops {loopsFromFirstInstance}");

                cycle = previousInstance + (loopLength * loopsFromFirstInstance);
                TraceLine($"Skipped to {cycle}");
                skipped = true;
            }
            else
            {
                cycleNumberCache.Add(description, cycle);
                cycleCache.Add(cycle, current);
            }
        }

        long load = CalculateLoad(current);

        return load.ToString();
    }

    private Grid2D<char> Cycle(Grid2D<char> dish)
    {
        var north = RollNorth(dish);
        // Draw(north);
        var west = RollWest(north);
        // Draw(west);
        var south = RollSouth(west);
        // Draw(south);
        var east = RollEast(south);
        // Draw(east);
        return east;
    }

    private void Draw(Grid2D<char> dish)
    {
        foreach (var y in dish.YIndexes())
        {
            foreach (var x in dish.XIndexes())
            {
                Trace(dish.Read(x, y));
            }

            TraceLine();
        }
        TraceLine();
    }

    private string ToString(Grid2D<char> dish)
    {
        var lines = new List<string>((int)dish.Height);
        foreach (var y in dish.YIndexes())
        {
            var line = string.Join("", dish.XIndexes().Select(x => dish.Read(x, y)));
            lines.Add(line);
        }
        return string.Join("", lines);
    }

    private Grid2D<char> RollNorth(Grid2D<char> dish)
    {
        var rolledNorth = dish.Clone();
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


    private Grid2D<char> RollEast(Grid2D<char> dish)
    {
        var rolledEast = dish.Clone();
        foreach (var y in rolledEast.YIndexes())
        {
            foreach (var startX in rolledEast.XIndexes().OrderByDescending(x => x))
            {
                var current = rolledEast.Read(startX, y);
                if (current == Rock)
                {
                    var newX = startX;
                    var rolling = true;
                    while (newX + 1 <= rolledEast.MaxX && rolling)
                    {
                        var east = rolledEast.Read(newX + 1, y);
                        if (east == Space)
                        {
                            newX += 1;
                        }
                        else
                        {
                            rolling = false;
                        }
                    }

                    if (newX != startX)
                    {
                        rolledEast.Write(newX, y, Rock);
                        rolledEast.Write(startX, y, Space);
                    }
                }
            }
        }

        return rolledEast;
    }

    private Grid2D<char> RollSouth(Grid2D<char> dish)
    {
        var rolledSouth = dish.Clone();
        foreach (var x in rolledSouth.XIndexes())
        {
            foreach (var startY in rolledSouth.YIndexes().OrderByDescending(x => x))
            {
                var current = rolledSouth.Read(x, startY);
                if (current == Rock)
                {
                    var newY = startY;
                    var rolling = true;
                    while (newY + 1 <= rolledSouth.MaxY && rolling)
                    {
                        var south = rolledSouth.Read(x, newY + 1);
                        if (south == Space)
                        {
                            newY += 1;
                        }
                        else
                        {
                            rolling = false;
                        }
                    }

                    if (newY != startY)
                    {
                        rolledSouth.Write(x, newY, Rock);
                        rolledSouth.Write(x, startY, Space);
                    }
                }
            }
        }

        return rolledSouth;
    }

    private Grid2D<char> RollWest(Grid2D<char> dish)
    {
        var rolledWest = dish.Clone();
        foreach (var y in rolledWest.YIndexes())
        {
            foreach (var startX in rolledWest.XIndexes())
            {
                var current = rolledWest.Read(startX, y);
                if (current == Rock)
                {
                    var newX = startX;
                    var rolling = true;
                    while (newX - 1 >= rolledWest.MinX && rolling)
                    {
                        var west = rolledWest.Read(newX - 1, y);
                        if (west == Space)
                        {
                            newX -= 1;
                        }
                        else
                        {
                            rolling = false;
                        }
                    }

                    if (newX != startX)
                    {
                        rolledWest.Write(newX, y, Rock);
                        rolledWest.Write(startX, y, Space);
                    }
                }
            }
        }

        return rolledWest;
    }
}
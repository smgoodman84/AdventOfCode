using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.PrimitiveExtensions;

namespace AdventOfCode._2024.Day13;

public class Day13 : Day
{
    public Day13() : base(2024, 13, "Day13/input_2024_13.txt", "33427", "", true)
    {
    }

    private List<ClawMachine> _machines = new ();
    public override void Initialise()
    {
        var lineGroups = InputLines.GroupsOfSize(4);
        foreach (var lineGroup in lineGroups)
        {
            var buttonA = ParseButton(lineGroup[0]);
            var buttonB = ParseButton(lineGroup[1]);
            var prize = ParsePrize(lineGroup[2]);

            _machines.Add(new ClawMachine
            {
                ButtonA = buttonA,
                ButtonB = buttonB,
                Prize = prize,
            });
        }
    }

    private Coordinate2D ParseButton(string buttonLine) => ParseLine(buttonLine, '+');
    private Coordinate2D ParsePrize(string buttonLine) => ParseLine(buttonLine, '=');

    private Coordinate2D ParseLine(string line, char searchChar)
    {
        var xStart = line.IndexOf(searchChar) + 1;
        var xEnd = line.IndexOf(',');
        var xString = line.Substring(xStart, xEnd - xStart);
        var x = long.Parse(xString);

        var yStart = line.LastIndexOf(searchChar) + 1;
        var yString = line.Substring(yStart);
        var y = long.Parse(yString);

        return new Coordinate2D(x, y);
    }

    public override string Part1()
    {
        /*
            p - prize
            A - a presses
            B - b presses

            px = A * ax + B * bx;
            py = A * ay + B * by;

            px = A * ax + B * bx;
            px - B * bx = A * ax;
            A = (px - B * bx) / ax;

            py = A * ay + B * by;
            py - A * ay = B * by;
            B = (py - A * ay) / by;

            A = (px - B * bx) / ax;
            A = (px - ((py - A * ay) / by) * bx) / ax;
            A * ax = px - ((py - A * ay) / by) * bx;
            A * ax - px = - ((py - A * ay) / by) * bx;
            (A * ax - px) / bx = - ((py - A * ay) / by);
        */

        var totalCost = 0L;
        foreach (var machine in _machines)
        {
            totalCost += machine.GetMinimumWinningCost();
        }

        return totalCost.ToString();
    }

    public override string Part2()
    {
        return string.Empty;
    }

    private class ClawMachine
    {
        public Coordinate2D ButtonA { get; set;}
        public Coordinate2D ButtonB { get; set;}
        public Coordinate2D Prize { get; set;}

        public long WinningCost { get; set; }

        public List<PressCount> WinningPressCounts { get; set; } = new ();

        public long GetMinimumWinningCost()
        {
            FindWinningPressCounts();

            if (!WinningPressCounts.Any())
            {
                return 0;
            }

            WinningCost = WinningPressCounts.Min(x => x.Cost);
            return WinningCost;
        }

        public Coordinate2D PositionAfterPresses(PressCount pressCount)
        {
            var x = ButtonA.X * pressCount.APresses + ButtonB.X * pressCount.BPresses;
            var y = ButtonA.Y * pressCount.APresses + ButtonB.Y * pressCount.BPresses;

            return new Coordinate2D(x, y);
        }

        public void FindWinningPressCounts()
        {
            var maxAPressesForX = Prize.X / ButtonA.X;
            var maxAPressesForY = Prize.Y / ButtonA.Y;
            var maxAPresses = (int)Math.Min(maxAPressesForX, maxAPressesForY);

            var maxBPressesForX = Prize.X / ButtonB.X;
            var maxBPressesForY = Prize.Y / ButtonB.Y;
            var maxBPresses = (int)Math.Min(maxBPressesForX, maxBPressesForY);

            foreach (var aPresses in Enumerable.Range(0, maxAPresses + 1))
            {
                foreach (var bPresses in Enumerable.Range(0, maxBPresses + 1))
                {
                    var pressCount = new PressCount
                    {
                        APresses = aPresses,
                        BPresses = bPresses,
                    };

                    var position = PositionAfterPresses(pressCount);

                    if (position.Equals(Prize))
                    {
                        WinningPressCounts.Add(pressCount);
                    }
                }
            }
        }
    }

    private class PressCount
    {
        public int APresses { get; set; }
        public int BPresses { get; set; }

        public int Cost => 3 * APresses + BPresses;
    }
}
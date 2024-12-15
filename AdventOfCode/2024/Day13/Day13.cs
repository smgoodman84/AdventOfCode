using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.PrimitiveExtensions;

namespace AdventOfCode._2024.Day13;

public class Day13 : Day
{
    public Day13() : base(2024, 13, "Day13/input_2024_13.txt", "33427", "91649162972270", true)
    {
    }

    private List<ClawMachine> _machines = new ();
    private List<ClawMachine> _machinesPartTwo = new ();
    public override void Initialise()
    {
        var lineGroups = InputLines.GroupsOfSize(4);
        foreach (var lineGroup in lineGroups)
        {
            var buttonA = ParseButton(lineGroup[0]);
            var buttonB = ParseButton(lineGroup[1]);
            var prize = ParsePrize(lineGroup[2]);

            _machines.Add(new ClawMachine(buttonA, buttonB, prize));

            _machinesPartTwo.Add(new ClawMachine(buttonA, buttonB,
                new Coordinate2D(prize.X + 10000000000000L, prize.Y + 10000000000000L)));
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
        var totalCost = 0L;
        foreach (var machine in _machines)
        {
            totalCost += machine.GetMinimumWinningCost2();
        }

        return totalCost.ToString();
    }

    public override string Part2()
    {
        var totalCost = 0L;
        foreach (var machine in _machinesPartTwo)
        {
            totalCost += machine.GetMinimumWinningCost2();
        }

        return totalCost.ToString();
    }

    private class ClawMachine
    {
        public Coordinate2D ButtonA { get; }
        public Coordinate2D ButtonB { get; }
        public Coordinate2D Prize { get; }
        public Vector2D SwapAForB { get; }
        public Vector2D SwapBForA { get; }

        public ClawMachine(
            Coordinate2D buttonA,
            Coordinate2D buttonB,
            Coordinate2D prize)
        {
            ButtonA = buttonA;
            ButtonB = buttonB;
            Prize = prize;

            SwapAForB = buttonB.Subtract(buttonA);
            SwapBForA = buttonA.Subtract(buttonB);
        }

        public long GetMinimumWinningCost()
        {
            var maxAPressesForX = Prize.X / ButtonA.X;
            var maxAPressesForY = Prize.Y / ButtonA.Y;
            var maxAPresses = (int)Math.Min(maxAPressesForX, maxAPressesForY) + 1;

            var maxBPressesForX = Prize.X / ButtonB.X;
            var maxBPressesForY = Prize.Y / ButtonB.Y;
            var maxBPresses = (int)Math.Min(maxBPressesForX, maxBPressesForY) + 1;

            var winningPressCounts = new List<PressCount>();
            foreach (var aPresses in Enumerable.Range(0, maxAPresses))
            {
                foreach (var bPresses in Enumerable.Range(0, maxBPresses))
                {
                    var pressCount = new PressCount(this, aPresses, bPresses);

                    if (pressCount.Winning)
                    {
                        winningPressCounts.Add(pressCount);
                    }
                }
            }

            if (!winningPressCounts.Any())
            {
                return 0;
            }

            return winningPressCounts.Min(c => c.Cost);
        }


        public static long GetMinimumWinningCostStatic(
            Coordinate2D buttonA,
            Coordinate2D buttonB,
            Coordinate2D prize,
            int buttonACost,
            int buttonBCost)
        {
            double prizeTan = 1.0 * prize.Y / prize.X;
            double prizeAngle = Math.Atan(prizeTan);

            double prizeDistance = Math.Sqrt(1.0 * prize.X * prize.X + 1.0 * prize.Y * prize.Y);
            
            double buttonATan = 1.0 * buttonA.Y / buttonA.X;
            double buttonAAngle = Math.Atan(buttonATan);
            double buttonBTan = 1.0 * buttonB.Y / buttonB.X;
            double buttonBAngle = Math.Atan(buttonBTan);

            if (buttonAAngle < buttonBAngle)
            {
                throw new Exception("Button A should have greate angle than button B");
            }

            var angleA = buttonAAngle - prizeAngle;
            var angleB = prizeAngle - buttonBAngle;
            var angleADash = Math.PI / 2 - angleA;
            var angleBDash = Math.PI / 2 - angleB;

            var lpb = prizeDistance * Math.Tan(angleA) / (Math.Tan(angleA) + Math.Tan(angleB));
            var lpa = prizeDistance - lpb;

            var la = lpa / Math.Cos(angleA);
            var ay = la * Math.Sin(buttonAAngle);
            var aPresses = ay / buttonA.Y;
            var aPressesLong = (long)aPresses;

            var yRemaining = prize.Y - (aPressesLong * buttonA.Y);
            var bPresses = yRemaining / buttonB.Y;
            var bPressesLong = (long)bPresses;

            for (var aPress = aPressesLong - 100; aPress <= aPressesLong + 100; aPress += 1)
            {
                for (var bPress = bPressesLong - 100; bPress <= bPressesLong + 100; bPress += 1)
                {
                    if (aPress * buttonA.X + bPress * buttonB.X == prize.X
                     && aPress * buttonA.Y + bPress * buttonB.Y == prize.Y)
                    {
                        return aPress * buttonACost + bPress * buttonBCost;
                    }
                }
            }

            return 0;
        }

        public long GetMinimumWinningCost2()
        {
            double buttonATan = 1.0 * ButtonA.Y / ButtonA.X;
            double buttonAAngle = Math.Atan(buttonATan);
            double buttonBTan = 1.0 * ButtonB.Y / ButtonB.X;
            double buttonBAngle = Math.Atan(buttonBTan);

            if (buttonAAngle > buttonBAngle)
            {
                return GetMinimumWinningCostStatic(ButtonA, ButtonB, Prize, 3, 1);
            }

            return GetMinimumWinningCostStatic(ButtonB, ButtonA, Prize, 1, 3);
        }

        public Coordinate2D PositionAfterPresses(PressCount pressCount)
        {
            var x = ButtonA.X * pressCount.APresses + ButtonB.X * pressCount.BPresses;
            var y = ButtonA.Y * pressCount.APresses + ButtonB.Y * pressCount.BPresses;

            return new Coordinate2D(x, y);
        }
    }

    private class PressCount
    {
        public PressCount(
            ClawMachine clawMachine,
            long aPresses,
            long bPresses)
        {
            APresses = aPresses;
            BPresses = bPresses;

            var xLocation = aPresses * clawMachine.ButtonA.X
                          + bPresses * clawMachine.ButtonB.X;

            var yLocation = aPresses * clawMachine.ButtonA.Y
                          + bPresses * clawMachine.ButtonB.Y;

            Location = new Coordinate2D(xLocation, yLocation);
            Cost = 3 * aPresses + bPresses;
            Winning = Location.Equals(clawMachine.Prize);
        }
        public long APresses { get; }
        public long BPresses { get; }

        public Coordinate2D Location { get; }
        public long Cost { get; }

        public bool Winning { get; }
    }
}
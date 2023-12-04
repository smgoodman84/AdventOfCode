using AdventOfCode.Shared;

namespace AdventOfCode._2023.Day04
{
    public class Day04 : Day
    {
        public Day04() : base(2023, 4, "Day04/input_2023_04.txt", "", "")
        {

        }

        private List<ScratchCard> _scratchCards;
        public override void Initialise()
        {
            _scratchCards = InputLines
                .Select(l => new ScratchCard(l))
                .ToList();
        }

        public override string Part1()
        {
            var totalPoints = _scratchCards
                .Sum(sc => sc.Points());

            return totalPoints.ToString();
        }

        public override string Part2()
        {
            return string.Empty;
        }

        private class ScratchCard
        {
            public int CardNumber { get; private set; }
            public List<int> Numbers { get; private set; }
            public List<int> WinningNumbers { get; private set; }
            public ScratchCard(string line)
            {
                var gameSplit = line.Split(":", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                CardNumber = int.Parse(gameSplit[0].Replace("Card", "").Trim());

                var numbersSplit = gameSplit[1].Split("|", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                Numbers = numbersSplit[0].Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                WinningNumbers = numbersSplit[1].Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            }

            public int Points()
            {
                var winningNumberCount = Numbers.Count(WinningNumbers.Contains);

                if (winningNumberCount == 0)
                {
                    return 0;
                }

                return (int)Math.Pow(2, winningNumberCount - 1);
            }
        }
    }
}


using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2023.Day12
{
    public class Day12 : Day
    {
        public Day12() : base(2023, 12, "Day12/input_2023_12.txt", "7402", "", true)
        {

        }

        public override void Initialise()
        {
        }

        public override string Part1()
        {
            return GetPossibilities(1).ToString();
        } 


        public override string Part2()
        {
            return GetPossibilities(5).ToString();
        }

        private int GetPossibilities(int times)
        {
            var rows = InputLines
                .Select(l => MultiplyLine(l, times))
                .Select(l => new SpringRow(l))
                .ToList();

            var sum = 0;
            var rowNumber = 1;
            foreach (var row in rows)
            {
                // Console.WriteLine(row.Description);
                var possibilities = row.GetPossibleRows().ToList();

                var validPossibilities = possibilities
                    .Where(r => r.IsValid())
                    .ToList();

                var count = validPossibilities.Count;

                TraceLine($"{rowNumber}: {count}");

                sum += count;
                rowNumber += 1;
            }

            return sum;
        }

        private string MultiplyLine(string line, int times)
        {
            if (times == 1)
            {
                return line;
            }

            var split = line.Split(' ');

            var springMultiplied = string.Join("?", Enumerable.Range(1, times).Select(l => split[0]));
            var lengthsMultiplied = string.Join(",", Enumerable.Range(1, times).Select(l => split[1]));

            return $"{springMultiplied} {lengthsMultiplied}";
        }

        private class SpringRow
        {
            public SpringState[] SpringStates { get; set; }
            public List<int> RowLengths { get; }
            public string RowLengthString { get; }
            public int ExpectedDamagedSprings { get; }
            public int CurrentDamagedSprings { get; }
            public int UnknownSprings { get; }
            public string Description { get; }

            public SpringRow(string description)
            {
                var split = description.Split(' ');

                SpringStates = split[0]
                    .Select(Parse)
                    .ToArray();

                RowLengthString = split[1];
                RowLengths = RowLengthString.Split(',')
                    .Select(int.Parse)
                    .ToList();

                ExpectedDamagedSprings = RowLengths.Sum();
                CurrentDamagedSprings = SpringStates.Count(s => s == SpringState.Damaged);
                UnknownSprings = SpringStates.Count(s => s == SpringState.Unknown);
                Description = description;
            }

            public SpringRow(
                SpringState[] springStates,
                string rowLengthString,
                List<int> rowLengths,
                int totalSprings,
                int currentSprings,
                int unknownSprings)
            {
                SpringStates = springStates;
                RowLengthString = rowLengthString;
                RowLengths = rowLengths;
                ExpectedDamagedSprings = totalSprings;
                CurrentDamagedSprings = currentSprings;
                UnknownSprings = unknownSprings;
            }

            public bool IsValid()
            {
                if (CurrentDamagedSprings != ExpectedDamagedSprings)
                {
                    return false;
                }

                var runLengths = GetDamagedRunLengths().ToList();
                var runLengthString = string.Join(",", runLengths);
                return runLengthString == RowLengthString;
            }

            public IEnumerable<int> GetDamagedRunLengths() => GetDamagedRunLength(SpringStates);

            public IEnumerable<int> GetDamagedRunLengthSoFar()
            {
                if (!SpringStates.Any(x => x == SpringState.Unknown))
                {
                    return GetDamagedRunLengths();
                }

                var untilUnknown = SpringStates
                    .TakeWhile(x => x != SpringState.Unknown)
                    .ToList();

                var lengthsUntilUnknown = GetDamagedRunLength(untilUnknown).ToList();

                var length = untilUnknown.Count;
                if (SpringStates[length - 1] != SpringState.Damaged)
                {
                    return lengthsUntilUnknown;
                }

                return lengthsUntilUnknown.Take(lengthsUntilUnknown.Count - 1);
            }

            public IEnumerable<int> GetDamagedRunLength(IEnumerable<SpringState> states)
            {
                var currentDamagedCount = 0;

                foreach (var state in states)
                {
                    if (state == SpringState.Damaged)
                    {
                        currentDamagedCount += 1;
                    }
                    else
                    {
                        if (currentDamagedCount > 0)
                        {
                            yield return currentDamagedCount;
                            currentDamagedCount = 0;
                        }
                    }
                }

                if (currentDamagedCount > 0)
                {
                    yield return currentDamagedCount;
                }
            }

            private SpringState Parse(char c)
            {
                switch (c)
                {
                    case '.': return SpringState.Operational;
                    case '#': return SpringState.Damaged;
                }

                return SpringState.Unknown;
            }

            public string GetDescription()
            {
                return string.Join("",
                    SpringStates.Select(c => c == SpringState.Damaged ? '#' : c == SpringState.Operational ? '.' : '?')
                    );
            }


            public SpringRow SetState(int index, SpringState state)
            {
                var newStates = SpringStates.ToArray();
                newStates[index] = state;

                var newCurrentDamagedSprings = CurrentDamagedSprings + (state == SpringState.Damaged ? 1 : 0);

                return new SpringRow(
                    newStates,
                    RowLengthString,
                    RowLengths,
                    ExpectedDamagedSprings,
                    newCurrentDamagedSprings,
                    UnknownSprings - 1);
            }

            public IEnumerable<SpringRow> GetPossibleRows()
            {
                if (UnknownSprings == 0)
                {
                    yield return this;
                }

                var description = GetDescription();

                for (var index = 0; index < SpringStates.Length; index += 1)
                {
                    if (SpringStates[index] == SpringState.Unknown)
                    {
                        var operational = SetState(index, SpringState.Operational);
                        var operationalDescription = operational.GetDescription();

                        var operationalDamagedLengthsSoFar = operational.GetDamagedRunLengthSoFar();
                        var operationalDamagedLengthsSoFarString = string.Join(",", operationalDamagedLengthsSoFar);
                        if (RowLengthString.StartsWith(operationalDamagedLengthsSoFarString))
                        {
                            // Console.WriteLine($"{description} - Set {index} to operational - {operationalDescription} - still valid ({operationalDamagedLengthsSoFarString})");
                            foreach (var possibilitity in operational.GetPossibleRows())
                            {
                                yield return possibilitity;
                            }
                        }
                        else
                        {
                            // Console.WriteLine($"{description} - Set {index} to operational - {operationalDescription} - NOT valid ({operationalDamagedLengthsSoFarString})");
                        }

                        if (CurrentDamagedSprings < ExpectedDamagedSprings)
                        {
                            var damaged = SetState(index, SpringState.Damaged);
                            var damagedDescription = damaged.GetDescription();
                            var damagedDamagedLengthsSoFar = damaged.GetDamagedRunLengthSoFar();
                            var damagedDamagedLengthsSoFarString = string.Join(",", damagedDamagedLengthsSoFar);
                            if (RowLengthString.StartsWith(damagedDamagedLengthsSoFarString))
                            {
                                // Console.WriteLine($"{description} - Set {index} to damaged - {damagedDescription} - still valid ({damagedDamagedLengthsSoFarString})");
                                foreach (var possibilitity in damaged.GetPossibleRows())
                                {
                                    yield return possibilitity;
                                }
                            }
                            else
                            {
                                // Console.WriteLine($"{description} - Set {index} to damaged - {damagedDescription} - NOT valid ({damagedDamagedLengthsSoFarString})");
                            }
                        }

                        break;
                    }
                }
            }
        }

        private enum SpringState
        {
            Operational,
            Damaged,
            Unknown
        }
    }
}

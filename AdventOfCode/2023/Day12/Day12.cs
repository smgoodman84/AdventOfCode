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

        private long GetPossibilities(int times)
        {
            var targets = InputLines
                .Select(l => MultiplyLine(l, times))
                .Select(l => new TargetSpringRow(l))
                .ToList();

            long sum = 0;
            var rowNumber = 1;
            foreach (var target in targets)
            {
                TraceLine(target.Description);
                var baseSpringRow = new SpringRow(target);
                var possibilities = baseSpringRow.GetPossibilities().ToList();

                long count = 0;
                foreach (var possibility in possibilities)
                {
                    var matches = possibility.MatchesTarget();
                    // TraceLine($"{possibility}: {matches}");
                    if (matches)
                    {
                        count += 1;
                    }
                }
                // var matching = possibilities.Where(x => x.MatchesTarget()).ToList();

                // var count = matching.Count;

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

        private class TargetSpringRow
        {
            public SpringState[] SpringStates { get; }
            public List<int> DamagedRowLengths { get; }
            public string DamagedRowLengthString { get; }
            public string Description { get; }

            public int Length => SpringStates.Length;

            public TargetSpringRow(string description)
            {
                var split = description.Split(' ');

                SpringStates = split[0]
                    .Select(Parse)
                    .ToArray();

                DamagedRowLengthString = split[1];
                DamagedRowLengths = DamagedRowLengthString.Split(',')
                    .Select(int.Parse)
                    .ToList();

                Description = description;
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
        }

        private class SpringRow
        {
            public SpringState[] CurrentStates { get; set; }
            public TargetSpringRow Target { get; set; }

            public SpringRow(TargetSpringRow target)
            {
                Target = target;
                CurrentStates = Enumerable.Range(0, target.Length)
                    .Select(_ => SpringState.Unknown)
                    .ToArray();
            }

            public SpringRow(TargetSpringRow target, SpringState[] currentStates)
            {
                Target = target;
                CurrentStates = currentStates;
            }

            private bool TryPositionUndamaged(int startIndex, int runLength, out SpringRow newRow)
            {
                var newStates = CurrentStates.ToArray();
                for (var index = 0; index < startIndex + runLength; index += 1)
                {
                    var targetState = Target.SpringStates[index];
                    if (newStates[index] == SpringState.Unknown)
                    {
                        if (index < startIndex)
                        {
                            newStates[index] = SpringState.Operational;
                        }
                        else
                        {
                            newStates[index] = SpringState.Damaged;
                        }
                    }

                    if (targetState != SpringState.Unknown
                        && targetState != newStates[index])
                    {
                        newRow = null;
                        return false;
                    }
                }

                newRow = new SpringRow(Target, newStates);
                return true;
            }

            public IEnumerable<SpringRow> GetPossibilities(List<int> unpositionedDamagedRuns = null)
            {
                unpositionedDamagedRuns ??= Target.DamagedRowLengths;
                var toPosition = unpositionedDamagedRuns.FirstOrDefault();

                if (toPosition == default)
                {
                    for (var index = 0; index < CurrentStates.Length; index += 1)
                    {
                        if (CurrentStates[index] == SpringState.Unknown)
                        {
                            CurrentStates[index] = SpringState.Operational;
                        }
                    }

                    yield return this;
                    yield break;
                }

                var remaining = unpositionedDamagedRuns.Skip(1).ToList();
                var totalRemainingDamaged = remaining.Sum(r => r);
                var minimumRemainingUndamaged = remaining.Count();

                var firstStartIndex = CurrentStates
                    .Select((x, i) => (x, i))
                    .First(x => x.x == SpringState.Unknown)
                    .i;

                if (firstStartIndex > 0 && CurrentStates[firstStartIndex - 1] == SpringState.Damaged)
                {
                    firstStartIndex += 1;
                }

                var lastStartIndex = Target.Length - totalRemainingDamaged - minimumRemainingUndamaged - toPosition;

                for (var startIndex = firstStartIndex; startIndex <= lastStartIndex; startIndex += 1)
                {
                    if (TryPositionUndamaged(startIndex, toPosition, out var newRow))
                    {
                        foreach(var possibility in newRow.GetPossibilities(remaining))
                        {
                            yield return possibility;
                        }
                    }
                }

            }

            public bool MatchesTarget()
            {
                for (var targetIndex = 0; targetIndex < Target.Length; targetIndex += 1)
                {
                    var target = Target.SpringStates[targetIndex];
                    var actual = CurrentStates[targetIndex];

                    if (target != SpringState.Unknown && actual != target)
                    {
                        return false;
                    }
                }

                return true;
            }

            public override string ToString()
            {
                return string.Join("", CurrentStates.Select(GetChar));
            }
        }

        private static char GetChar(SpringState state)
        {
            switch (state)
            {
                case SpringState.Operational: return '.';
                case SpringState.Damaged: return '#';
                default: return '?';
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

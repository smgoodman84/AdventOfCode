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
            return string.Empty; GetPossibilities(5).ToString();
        }

        private long GetPossibilities(int times)
        {
            var targets = InputLines
                .Select(l => MultiplyLine(l, times))
                .Select(l => new TargetSpringRow(l))
                .ToList();

            long sum = 0;
            var row = 1;
            foreach (var target in targets)
            {
                var possibilities = target.GetPossibilities();
                sum += possibilities;

                TraceLine($"RESULT {row}: {target.Description} = {possibilities}");
                row += 1;
            }
            /*
            var possibilities = targets
                .Select(t => t.GetPossibilities())
                .ToList();

            var sum = possibilities.Sum();
            
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
                    TraceLine($"{possibility}: {matches}");
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
            */
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

            private static Dictionary<string, long> _possibilityCache = new Dictionary<string, long>();

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

            public TargetSpringRow(
                SpringState[] springStates,
                List<int> damagedRowLengths)
            {
                SpringStates = springStates;
                DamagedRowLengths = damagedRowLengths;
                DamagedRowLengthString = string.Join(",", damagedRowLengths);

                var springStateDescription = string.Join("", springStates.Select(GetChar));
                Description = $"{springStateDescription} {DamagedRowLengthString}";
            }

            public long GetPossibilities()
            {
                if (!_possibilityCache.ContainsKey(Description))
                {
                    // Console.WriteLine($"Calculating {Description}");
                    var result = GetPossibilitiesUncached();

                    // Console.WriteLine($"{Description} = {result}");
                    _possibilityCache.Add(Description, result);
                    return result;
                }

                return _possibilityCache[Description];
            }

            private long GetPossibilitiesUncached()
            {
                if (!DamagedRowLengths.Any())
                {
                    return 1;
                }

                if (!SpringStates.Any(x => x != SpringState.Operational))
                {
                    return 0;
                }

                var firstStartIndex = SpringStates
                    .Select((x, i) => (x, i))
                    .First(x => x.x != SpringState.Operational)
                    .i;


                var toPosition = DamagedRowLengths.FirstOrDefault();
                var remainingRowLengths = DamagedRowLengths.Skip(1).ToList();
                var totalRemainingDamaged = remainingRowLengths.Sum();
                var minimumRemainingUndamaged = remainingRowLengths.Count();

                var lastStartIndex = SpringStates.Length - totalRemainingDamaged - minimumRemainingUndamaged - toPosition;

                if (SpringStates.Any(x => x == SpringState.Damaged))
                {
                    var firstDamaged = SpringStates
                    .Select((x, i) => (x, i))
                    .First(x => x.x == SpringState.Damaged)
                    .i;

                    if (firstDamaged < lastStartIndex)
                    {
                        lastStartIndex = firstDamaged;
                    }
                }

                long totalPossibilities = 0;
                for (var startIndex = firstStartIndex; startIndex <= lastStartIndex; startIndex += 1)
                {
                    if (CanPositionUndamaged(startIndex, toPosition))
                    {
                        if (!remainingRowLengths.Any())
                        {
                            totalPossibilities += 1;
                        }
                        else
                        {
                            var remainingStatesStartIndex = startIndex + toPosition + 1;
                            var remainingStates = SpringStates.AsSpan().Slice(remainingStatesStartIndex).ToArray();
                            var remaining = new TargetSpringRow(remainingStates, remainingRowLengths);

                            var remainingPossibilities = remaining.GetPossibilities();
                            totalPossibilities += remainingPossibilities;
                        }
                    }
                }
                return totalPossibilities;
            }

            private bool CanPositionUndamaged(int startIndex, int runLength)
            {
                var previous = startIndex - 1;
                var next = startIndex + runLength;
                if (previous >= 0 && SpringStates[previous] == SpringState.Damaged)
                {
                    return false;
                }

                if (next < SpringStates.Length && SpringStates[next] == SpringState.Damaged)
                {
                    return false;
                }

                for (var index = startIndex; index < startIndex + runLength; index += 1)
                {
                    if (SpringStates[index] == SpringState.Operational)
                    {
                        return false;
                    }
                }

                return true;
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
            private static Dictionary<string, List<SpringRow>> _cache = new Dictionary<string, List<SpringRow>>();

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

            public List<SpringRow> GetPossibilities(List<int> unpositionedDamagedRuns = null)
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

                    var thisResult = new List<SpringRow> { this };
                    // _cache.Add(cacheKey, thisResult);
                    return thisResult;
                }



                var unPositionedCsv = string.Join(",", unpositionedDamagedRuns);
                var firstUnknown = CurrentStates
                    .Select((x, i) => (x, i))
                    .First(x => x.x == SpringState.Unknown)
                    .i;

                var targetString = $"{string.Join("", Target.SpringStates.Select(GetChar))}";
                var targetCacheKey = targetString.Substring(firstUnknown);
                if (firstUnknown > 0)
                {
                    targetCacheKey = $"{GetChar(CurrentStates[firstUnknown - 1])}{targetCacheKey}";
                }
                else
                {
                    targetCacheKey = $"S{targetCacheKey}";
                }
                var cacheKey = $"{targetCacheKey}:{unPositionedCsv}";

                if (_cache.ContainsKey(cacheKey))
                {
                    var cachedResult = _cache[cacheKey];
                    return cachedResult;
                }



                var remaining = unpositionedDamagedRuns.Skip(1).ToList();
                var totalRemainingDamaged = remaining.Sum(r => r);
                var minimumRemainingUndamaged = remaining.Count();


                var firstStartIndex = firstUnknown;
                if (firstStartIndex > 0 && CurrentStates[firstStartIndex - 1] == SpringState.Damaged)
                {
                    firstStartIndex += 1;
                }

                var lastStartIndex = Target.Length - totalRemainingDamaged - minimumRemainingUndamaged - toPosition;

                var results = new List<SpringRow>();
                for (var startIndex = firstStartIndex; startIndex <= lastStartIndex; startIndex += 1)
                {
                    if (TryPositionUndamaged(startIndex, toPosition, out var newRow))
                    {
                        var currentResults = newRow.GetPossibilities(remaining);
                        results.AddRange(currentResults);
                    }
                }

                _cache.Add(cacheKey, results);
                return results;
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

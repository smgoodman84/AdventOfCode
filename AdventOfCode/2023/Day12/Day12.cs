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
            return string.Empty; // GetPossibilities(1).ToString();
        } 


        public override string Part2()
        {
            return GetPossibilities(5).ToString();
        }

        private int GetPossibilities(int times)
        {
            var targets = InputLines
                .Select(l => MultiplyLine(l, times))
                .Select(l => new TargetSpringRow(l))
                .ToList();

            var sum = 0;
            var rowNumber = 1;
            foreach (var target in targets)
            {
                TraceLine(target.Description);
                var baseSpringRow = new SpringRow(target);
                var possibilities = baseSpringRow.GetPossibilities().ToList();

                var count = 0;
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
            public List<SpringRowRun> Row { get; set; }
            public int CurrentLength { get; set; }
            public TargetSpringRow Target { get; set; }

            public SpringRow(TargetSpringRow target)
            {
                Target = target;

                Row = new List<SpringRowRun>();
                Row.Add(new SpringRowRun
                {
                    SpringState = SpringState.Operational,
                    Length = 0
                });

                CurrentLength = 0;
                foreach(var rowLength in target.DamagedRowLengths)
                {

                    Row.Add(new SpringRowRun
                    {
                        SpringState = SpringState.Damaged,
                        Length = rowLength
                    });
                    CurrentLength += rowLength;

                    Row.Add(new SpringRowRun
                    {
                        SpringState = SpringState.Operational,
                        Length = 1
                    });
                    CurrentLength += 1;
                }

                Row.Last().Length = 0;
                CurrentLength -= 1;
            }

            private SpringRow()
            {

            }

            public IEnumerable<SpringRow> GetPossibilities()
            {
                var unallocatedCount = Target.Length - CurrentLength;
                if (unallocatedCount == 0)
                {
                    yield return this;
                    yield break;
                }

                var index = 0;
                foreach (var rowSection in Row)
                {
                    if (rowSection.SpringState == SpringState.Operational)
                    {
                        var expanded = Expand(index);
                        foreach(var expandedPossibility in expanded.GetPossibilities())
                        {
                            yield return expandedPossibility;
                        }
                    }

                    index += 1;
                }
            }

            private SpringRow Expand(int index)
            {
                var newRow = Row.ToList();
                newRow[index] = new SpringRowRun
                {
                    Length = Row[index].Length + 1,
                    SpringState = Row[index].SpringState
                };

                var result = new SpringRow()
                {
                    Target = Target,
                    Row = newRow,
                    CurrentLength = CurrentLength + 1
                };

                return result;
            }

            public bool MatchesTarget()
            {
                var rowRunIndex = Row[0].Length == 0 ? 1 : 0;
                var rowRunCount = 0;
                for (var targetIndex = 0; targetIndex < Target.Length; targetIndex += 1)
                {
                    var target = Target.SpringStates[targetIndex];
                    var row = Row[rowRunIndex];
                    var actual = row.SpringState;

                    if (target != SpringState.Unknown && actual != target)
                    {
                        return false;
                    }

                    rowRunCount += 1;
                    if (rowRunCount >= row.Length)
                    {
                        rowRunIndex += 1;
                        rowRunCount = 0;
                    }
                }

                return true;
            }

            public override string ToString()
            {
                return string.Join("", Row.Select(x => x.ToString()));
            }
        }

        private class SpringRowRun
        {
            public SpringState SpringState { get; set; }
            public int Length { get; set; }

            public override string ToString()
            {
                return new string(GetChar(SpringState), Length);
            }

            private char GetChar(SpringState state)
            {
                switch (state)
                {
                    case SpringState.Operational: return '.';
                    case SpringState.Damaged: return '#';
                    default: return '?';
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

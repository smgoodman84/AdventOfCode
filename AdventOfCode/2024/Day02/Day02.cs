using AdventOfCode.Shared;

namespace AdventOfCode._2024.Day02;

public class Day02 : Day
{
    public Day02() : base(2024, 2, "Day02/input_2024_02.txt", "314", "373")
    {

    }

    private List<Report> _reports;

    public override void Initialise()
    {
        _reports = InputLines
            .Select(x => new Report(x))
            .ToList();
    }

    public override string Part1()
    {
        var safeCount = _reports.Count(r => r.CheckSafe(0));
        return safeCount.ToString();
    }

    public override string Part2()
    {
        var safeCount = _reports.Count(r => r.CheckSafe(1));
        return safeCount.ToString();
    }

    private class Report
    {
        public Report(string report)
        {
            Levels = report
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(int.Parse)
                .ToList();
            
            ReverseLevels = Levels.ToList();
            ReverseLevels.Reverse();
        }

        public List<int> Levels {get;set;}
        public List<int> ReverseLevels {get;set;}

        public bool CheckSafe(int removeCount)
        {
            return CheckSafe(Levels, removeCount)
             || CheckSafe(ReverseLevels, removeCount);
        }

        private bool CheckSafe(List<int> levels, int removeCount)
        {
            // Removing first element is special cased.
            if (removeCount < 0)
            {
                return false;
            }

            if (CheckSafe(levels.Skip(1).ToList(), removeCount - 1))
            {
                return true;
            }

            // Handles removing any element but the first
            var previous = levels.First();
            foreach(var level in levels.Skip(1))
            {
                var removing = false;
                if (level <= previous)
                {
                    removing = true;
                }

                if (level > previous + 3)
                {
                    removing = true;
                }

                if (removing)
                {
                    if (removeCount == 0)
                    {
                        return false;
                    }
                    else
                    {
                        removeCount -= 1;
                    }
                }
                else
                {
                    previous = level;
                }
            }

            return true;
        }
    }
}
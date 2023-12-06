using AdventOfCode.Shared;
using AdventOfCode.Shared.Mathematics;

namespace AdventOfCode._2023.Day06
{
    public class Day06 : Day
    {
        public Day06() : base(2023, 6, "Day06/input_2023_06.txt", "140220", "39570184")
        {

        }

        public override void Initialise()
        {
        }

        public override string Part1()
        {
            var races = new List<Race>();

            var times = InputLines[0]
                .Replace("Time:", "")
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();

            var distances = InputLines[1]
                .Replace("Distance:", "")
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();

            for (var i = 0; i < times.Length; i++)
            {
                races.Add(new Race
                {
                    Time = times[i],
                    DistanceRecord = distances[i]
                });
            }

            var result = 1;
            foreach (var race in races)
            {
                var possibleHoldTimes = Enumerable.Range(1, race.Time - 2);
                var possibleDistances = possibleHoldTimes
                    .Select(t => (race.Time - t) * t)
                    .ToList();

                var waysToWin = possibleDistances.Count(d => d > race.DistanceRecord);
                result *= waysToWin;
            }

            return result.ToString();
        }

        public override string Part2()
        {
            var time = long.Parse(InputLines[0].Replace("Time:", "").Replace(" ", ""));
            var distanceRecord = long.Parse(InputLines[1].Replace("Distance:", "").Replace(" ", ""));

            // ht = hold time
            // tt = total time
            // dr = distance record
            // dt = distance traveled

            // dt = (tt - ht) * ht
            // win when dt > dr
            // (tt - ht) * ht > dr
            // (tt - ht) * ht - dr > 0
            // tt*ht - ht^2 - dr > 0

            // Find quadratic roots for
            // tt*ht - ht^2 - dr == 0
            // a = -1
            // b = tt
            // c = -dr
            var roots = MathematicsHelper.SolveQuadratic(-1.0, time, -distanceRecord);

            // Can win in all scenarios between roots
            var difference = (int)Math.Abs(roots.Soluction2 - roots.Solution1);

            return difference.ToString();
        }

        private class Race
        {
            public int Time { get; set; }
            public int DistanceRecord { get; set; }
        }
    }
}


using AdventOfCode.Shared;

namespace AdventOfCode._2023.Day06
{
    public class Day06 : Day
    {
        public Day06() : base(2023, 6, "Day06/input_2023_06.txt", "140220", "")
        {

        }

        private List<Race> _races;
        public override void Initialise()
        {
            _races = new List<Race>();

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
                _races.Add(new Race
                {
                    Time = times[i],
                    DistanceRecord = distances[i]
                });
            }
        }

        public override string Part1()
        {
            var result = 1;
            foreach (var race in _races)
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
            return string.Empty;
        }

        private class Race
        {
            public int Time { get; set; }
            public int DistanceRecord { get; set; }
        }
    }
}


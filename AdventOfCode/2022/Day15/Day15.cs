using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.Numbers;

namespace AdventOfCode._2022.Day15
{
    public class Day15 : Day
    {
        private static readonly Regex InputParser = new Regex("Sensor at x=(?<sensorx>[-]?[0-9]*), y=(?<sensory>[-]?[0-9]*): closest beacon is at x=(?<beaconx>[-]?[0-9]*), y=(?<beacony>[-]?[0-9]*)");

        public Day15() : base(2022, 15, "Day15/input_2022_15.txt", "5511201", "11318723411840")
        {

        }

        private List<Sensor> _sensors;
        public override void Initialise()
        {
            _sensors = InputLines
                .Select(ParseLine)
                .ToList();
        }

        private Sensor ParseLine(string input)
        {
            var match = InputParser.Match(input);
            var sensorx = long.Parse(match.Groups["sensorx"].Value);
            var sensory = long.Parse(match.Groups["sensory"].Value);
            var beaconx = long.Parse(match.Groups["beaconx"].Value);
            var beacony = long.Parse(match.Groups["beacony"].Value);

            return new Sensor(new Coordinate2D(sensorx, sensory), new Coordinate2D(beaconx, beacony));
        }

        public override string Part1()
        {
            var targetRow = 2000000;

            var merged = GetCoverageOfRow(targetRow);

            var covered = merged.Sum(r => r.End - r.Start + 1);

            var beaconsOnRow = _sensors
                .Select(s => s.ClosestBeacon)
                .Where(b => b.Y == targetRow)
                .DistinctBy(b => b.ToString());

            var beaconsCovered = beaconsOnRow
                .Where(b => merged.Any(r => r.Contains(b.X)))
                .Count();

            return (covered - beaconsCovered).ToString();
        }

        public override string Part2()
        {
            var size = 4000000;
            foreach (var y in Enumerable.Range(0, size))
            {
                var ranges = GetCoverageOfRow(y).ToList();
                if (ranges.Count() > 1)
                {
                    var uncovered = new NumberRange(0, size);
                    foreach (var range in ranges)
                    {
                        uncovered = uncovered.Subtract(range);
                    }
                    var x = uncovered.Start;

                    return (4000000 * x + y).ToString();
                }
            }

            return "";
        }

        private IEnumerable<NumberRange> GetCoverageOfRow(int targetRow)
        {
            var ranges = _sensors
                .Select(s => s.CoverageOfRow(targetRow))
                .Where(r => r != null)
                .ToList();

            var merged = NumberRangeMerger.Merge(ranges);

            return merged;
        }

        private class Sensor
        {
            public Coordinate2D Location { get; }
            public Coordinate2D ClosestBeacon { get; }
            public long DistanceCovered { get; }

            public Sensor(Coordinate2D location, Coordinate2D closestBeacon)
            {
                Location = location;
                ClosestBeacon = closestBeacon;
                DistanceCovered = Location.ManhattanDistanceTo(ClosestBeacon);
            }

            public NumberRange CoverageOfRow(long y)
            {
                var distanceToRow = Math.Abs(Location.Y - y);
                var distanceRemaining = DistanceCovered - distanceToRow;

                if (distanceRemaining < 0)
                {
                    return null;
                }

                return new NumberRange(Location.X - distanceRemaining, Location.X + distanceRemaining);
            }
        }
    }
}

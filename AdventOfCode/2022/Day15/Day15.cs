using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.PrimitiveExtensions;

namespace AdventOfCode._2022.Day15
{
    public class Day15 : Day
    {
        private static Regex InputParser = new Regex("Sensor at x=(?<sensorx>[-]?[0-9]*), y=(?<sensory>[-]?[0-9]*): closest beacon is at x=(?<beaconx>[-]?[0-9]*), y=(?<beacony>[-]?[0-9]*)");

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
            // Console.WriteLine($"Parsing {input}");
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
            foreach (var y in Enumerable.Range(0, 4000000))
            {
                var ranges = GetCoverageOfRow(y).ToList();
                if (ranges.Count() > 1)
                {
                    var x = ranges[0].End + 1;

                    return (4000000 * x + y).ToString();
                }
            }

            return "";
        }

        private IEnumerable<Range> GetCoverageOfRow(int targetRow)
        {
            var ranges = _sensors
                .Select(s => s.CoverageOfRow(targetRow))
                .Where(r => r != null)
                .ToList();

            var merged = RangeMerger.Merge(ranges);

            return merged;
        }

        private class Sensor
        {
            public Sensor(Coordinate2D location, Coordinate2D closestBeacon)
            {
                Location = location;
                ClosestBeacon = closestBeacon;

                DistanceCovered = Math.Abs(Location.X - ClosestBeacon.X)
                    + Math.Abs(Location.Y - ClosestBeacon.Y);
            }

            public Range CoverageOfRow(long y)
            {
                var distanceToRow = Math.Abs(Location.Y - y);
                var distanceRemaining = DistanceCovered - distanceToRow;

                if (distanceRemaining < 0)
                {
                    return null;
                }

                return new Range(Location.X - distanceRemaining, Location.X + distanceRemaining);
            }

            public Coordinate2D Location { get; }
            public Coordinate2D ClosestBeacon { get; }

            public long DistanceCovered { get; }

            public bool IsCovered(long x, long y)
            {
                var distance = Math.Abs(Location.X - x)
                    + Math.Abs(Location.Y - y);

                return distance <= DistanceCovered;
            }
        }

        private static class RangeMerger
        {
            public static IEnumerable<Range> Merge(IEnumerable<Range> ranges)
            {
                return Merge(ranges.ToList());
            }

            private static List<Range> Merge(List<Range> ranges)
            {
                var initialCount = ranges.Count;
                var reduced = PartialMerge(ranges);
                var reducedCount = reduced.Count;

                if (reducedCount == initialCount)
                {
                    return ranges;
                }

                return Merge(reduced);
            }

            private static List<Range> PartialMerge(List<Range> ranges)
            {
                var merged = new List<Range>();

                foreach (var range in ranges.OrderBy(r => r.Start))
                {
                    if (!merged.Any())
                    {
                        merged.Add(range);
                    }
                    else
                    {
                        var mergedIn = false;
                        for (var i = 0; i < merged.Count && !mergedIn; i++)
                        {
                            var mergedRange = merged[i];
                            if (mergedRange.Contains(range))
                            {
                                mergedIn = true;
                            }
                            else if (mergedRange.CanAdd(range))
                            {
                                merged[i] = mergedRange.Add(range);
                                mergedIn = true;
                            }
                        }

                        if (!mergedIn)
                        {
                            merged.Add(range);
                        }
                    }
                }

                return merged;
            }
        }

        private class Range
        {
            public Range(long start, long end)
            {
                Start = start;
                End = end;
            }

            public long Start { get; }
            public long End { get; }

            public bool CanAdd(Range range)
            {
                return Contains(range)
                    || range.Contains(this)
                    || Contains(range.Start)
                    || Contains(range.End)
                    || range.End + 1 == Start
                    || End + 1 == range.Start;
            }

            public bool Contains(long value)
            {
                return Start <= value && value <= End;
            }

            public bool Contains(Range range)
            {
                return Contains(range.Start) && Contains(range.End);
            }

            public Range Add(Range range)
            {
                if (Contains(range))
                {
                    return this;
                }

                if (range.Contains(this))
                {
                    return range;
                }

                if (Contains(range.Start))
                {
                    return new Range(Start, range.End);
                }

                if (Contains(range.End))
                {
                    return new Range(range.Start, End);
                }

                if (range.End + 1 == Start)
                {
                    return new Range(range.Start, End);
                }

                if (End + 1 == range.Start)
                {
                    return new Range(Start, range.End);
                }

                throw new Exception("Cannot Add Range");
            }

            public override string ToString()
            {
                return $"{Start}..{End} ({End - Start})";
            }
        }
    }
}

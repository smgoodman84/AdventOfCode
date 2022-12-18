using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Shared;

namespace AdventOfCode._2015.Day09
{
    public class Day09 : Day
    {
        public Day09() : base(2015, 9, "Day09/input_2015_09.txt", "117", "909")
        {
        }

        private static Regex _inputParser = new Regex("^(?<source>.*) to (?<destination>.*) = (?<distance>[0-9]*)$", RegexOptions.Compiled);

        private Dictionary<string, Dictionary<string, int>> _distances
            = new Dictionary<string, Dictionary<string, int>>();

        public override void Initialise()
        {
            foreach (var line in InputLines)
            {
                var parsed = _inputParser.Match(line);

                var source = parsed.Groups["source"].Value;
                var destination = parsed.Groups["destination"].Value;
                var distance = int.Parse(parsed.Groups["distance"].Value);

                if (!_distances.ContainsKey(source))
                {
                    _distances.Add(source, new Dictionary<string, int>());
                }

                if (!_distances.ContainsKey(destination))
                {
                    _distances.Add(destination, new Dictionary<string, int>());
                }

                _distances[source][destination] = distance;
                _distances[destination][source] = distance;
            }
        }

        private int GetDistance(IEnumerable<string> path)
        {
            var distance = 0;
            var previous = path.First();

            foreach (var stop in path.Skip(1))
            {
                distance += _distances[previous][stop];
                previous = stop;
            }

            return distance;
        }

        private IEnumerable<IEnumerable<string>> Combinations(IEnumerable<string> values)
        {
            if (!values.Any())
            {
                yield return Enumerable.Empty<string>();
            }

            foreach (var value in values)
            {
                var others = values.Where(v => v != value);

                foreach (var combination in Combinations(others))
                {
                    yield return new[] { value }.Concat(combination).ToList();
                }
            }
        }

        public override string Part1()
        {
            var locations = _distances.Keys.Select(k => k).ToList();
            var paths = Combinations(locations).ToList();
            var distances = paths.Select(GetDistance).ToList();
            var min = distances.Min();
            return min.ToString();
        }

        public override string Part2()
        {
            var locations = _distances.Keys.Select(k => k).ToList();
            var paths = Combinations(locations).ToList();
            var distances = paths.Select(GetDistance).ToList();
            var max = distances.Max();
            return max.ToString();
        }
    }
}
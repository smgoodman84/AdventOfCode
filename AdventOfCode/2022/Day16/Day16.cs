using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Shared;

namespace AdventOfCode._2022.Day16
{
    public class Day16 : Day
    {
        private static readonly Regex InputParser = new Regex("Valve (?<valve>.*) has flow rate=(?<flowrate>[0-9]*); tunnel[s]* lead[s]* to valve[s]* (?<leadsto>.*)");

        public Day16() : base(2022, 16, "Day16/input_2022_16.txt", "1488", "")
        {

        }

        private List<Valve> _valves;
        public override void Initialise()
        {
            _valves = InputLines
                .Select(ParseLine)
                .ToList();
        }

        private Valve ParseLine(string input)
        {
            var match = InputParser.Match(input);
            var valve = match.Groups["valve"].Value;
            var flowrate = int.Parse(match.Groups["flowrate"].Value);
            var leadsto = match.Groups["leadsto"].Value.Split(", ");

            return new Valve(valve, flowrate, leadsto, false);
        }

        public override string Part1()
        {
            var valvemap = new ValveMap(_valves, "AA", null, 30);

            // var example = valvemap.FollowPath("DD", "BB", "JJ", "HH", "EE", "CC");
            var possibleOutcomes = valvemap.GetTopOutcomes(15).ToList();
            var result = possibleOutcomes.Max(x => x.GetPressureReleased());

            return result.ToString();
        }

        public override string Part2()
        {
            var valvemap = new ValveMap(_valves, "AA", "AA", 26);

            var possibleOutcomes = valvemap.GetTopOutcomes(15).ToList();
            var result = possibleOutcomes.Max(x => x.GetPressureReleased());

            return result.ToString();
        }

        private class Valve
        {
            public string Name { get; }
            public int FlowRate { get; }
            public IEnumerable<string> LeadsTo { get; }
            public bool IsOpen { get; }

            public Valve(string name, int flowRate, IEnumerable<string> leadsTo, bool isOpen)
            {
                Name = name;
                FlowRate = flowRate;
                LeadsTo = leadsTo;
                IsOpen = isOpen;
            }
        }

        private class ValveMap
        {
            public int MinutesPassed { get; }

            private readonly ValveMap _base;
            private readonly List<string> _allLocations;
            private readonly Dictionary<string, Valve> _overrides;
            private readonly ShortestPaths _shortestPaths;
            private readonly string _elephantlocation;
            private readonly string _location;
            private int _pressureReleased = 0;
            private int _timeAvailable;
            private readonly string _logMessage;

            public ValveMap(
                IEnumerable<Valve> valves,
                string location,
                string elephantLocation,
                int timeAvailable)
            {
                var possibleMoves = valves.SelectMany(v => v.LeadsTo.Select(lt => (v.Name, lt))).ToList();
                _shortestPaths = new ShortestPaths(possibleMoves);
                _overrides = valves.ToDictionary(v => v.Name, v => v);
                _allLocations = valves.Select(v => v.Name).ToList();
                _location = location;
                _elephantlocation = elephantLocation;
                MinutesPassed = 0;
                _timeAvailable = timeAvailable;
                _logMessage = "Start";
            }

            private ValveMap(
                ValveMap baseMap,
                IEnumerable<Valve> overrideValves,
                string location,
                string elephantLocation)
            {
                var logs = overrideValves.Select(o => $"Open {o.Name}").ToList();
                if (_location != location)
                {
                    logs.Add($"Move to {location}");
                }

                if (_elephantlocation != elephantLocation)
                {
                    logs.Add($"Elephant move to {location}");
                }

                _logMessage =string.Join(", ", logs);

                _shortestPaths = baseMap._shortestPaths;
                _base = baseMap;
                _overrides = overrideValves.ToDictionary(o => o.Name, o => o);
                _location = location;
                _elephantlocation = elephantLocation;
                _allLocations = baseMap._allLocations;
                MinutesPassed = baseMap.MinutesPassed + 1;
                _timeAvailable = baseMap._timeAvailable;
            }

            private ValveMap(ValveMap baseMap)
                : this(
                      baseMap,
                      Enumerable.Empty<Valve>(),
                      baseMap._location,
                      baseMap._elephantlocation)
            {

            }

            private ValveMap(ValveMap baseMap, IEnumerable<Valve> overrideValves)
                : this(
                      baseMap,
                      overrideValves,
                      baseMap._location,
                      baseMap._elephantlocation)
            {

            }

            private ValveMap(ValveMap baseMap, string location, string elephantLocation)
                : this(
                      baseMap,
                      Enumerable.Empty<Valve>(),
                      location,
                      elephantLocation)
            {

            }

            public ValveMap FollowPath(params string[] locations)
            {
                if (MinutesPassed == _timeAvailable)
                {
                    return this;
                }

                if (!locations.Any())
                {
                    return SitOutTheClock();
                }

                return NavigateToLocationAndOpenValve((locations.First(), null))
                    .FollowPath(locations.Skip(1).ToArray());
            }

            public IEnumerable<ValveMap> GetTopOutcomes(int depth)
            {
                var topDestinations = TopDestinations(_location, _elephantlocation, depth).ToList();

                if (!topDestinations.Any())
                {
                    yield return SitOutTheClock();
                }

                foreach (var topDestination in topDestinations)
                {
                    foreach (var outcome in NavigateToLocationAndOpenValve(topDestination).GetTopOutcomes(depth))
                    {
                        yield return outcome;
                    }
                }
            }

            public ValveMap SitOutTheClock()
            {
                if (MinutesPassed == _timeAvailable)
                {
                    return this;
                }


                // Console.WriteLine($"{MinutesPassed} Sitting out the clock");
                ReleasePressure();
                return new ValveMap(this).SitOutTheClock();
            }


            private Valve GetCurrentValve() => GetValve(_location);

            private Valve GetValve(string location)
            {
                if (_overrides.ContainsKey(location))
                {
                    return _overrides[location];
                }

                return _base.GetValve(location);
            }

            public IEnumerable<string> CanMoveTo()
            {
                return GetCurrentValve().LeadsTo;
            }

            public IEnumerable<string> TopDestinations(string location, int count)
            {
                var potentialValves = _allLocations
                    .Where(l => l != location)
                    .Select(GetValve)
                    .Where(v => !v.IsOpen && v.FlowRate > 0)
                    .Select(v => (Location: v.Name, PotentialFlowRate: GetPotentialFlowRate(location, v.Name)))
                    .ToList();

                var locations = potentialValves
                    .Where(x => x.PotentialFlowRate > 0)
                    .OrderByDescending(x => x.PotentialFlowRate)
                    .Take(count)
                    .Select(v => v.Location)
                    .ToList();

                return locations;
            }

            public IEnumerable<(string, string)> TopDestinations(string location, string elephantLocation, int count)
            {
                var locations = TopDestinations(location, count);
                var elephantLocations = TopDestinations(elephantLocation, count);

                foreach(var l in locations)
                {
                    foreach (var e in elephantLocations)
                    {
                        yield return (l, e);
                    }
                }
            }

            private int GetPotentialFlowRate(string currentLocation, string location)
            {
                var shortestPath = _shortestPaths.GetShortestPath(currentLocation, location, new List<string>());
                var timeUntilOpen = shortestPath.Stops.Count + 2;
                var flowDuration = _timeAvailable - MinutesPassed - timeUntilOpen;
                var valveFlow = GetValve(location).FlowRate;
                var flowValue = flowDuration > 0 ? flowDuration * valveFlow : 0;
                return flowValue;
            }

            public ValveMap NavigateToLocationAndOpenValve((string, string) locations)
            {
                var myLocation = locations.Item1;

                if (CanMoveDirectlyTo(myLocation))
                {
                    return MoveTo(myLocation).OpenValve();
                }

                var nextStep = _shortestPaths.GetNextStep(_location, myLocation);
                return MoveTo(nextStep).NavigateToLocationAndOpenValve(locations);
            }

            public bool CanMoveDirectlyTo(string location)
            {
                return GetCurrentValve().LeadsTo.Contains(location);
            }

            public ValveMap MoveTo(string location)
            {
                if (!CanMoveDirectlyTo(location))
                {
                    throw new Exception($"Cannot move to {location}");
                }

                // Console.WriteLine($"{MinutesPassed} Moving to {location}");
                ReleasePressure();
                return new ValveMap(this, location, _elephantlocation);
            }

            public bool CanOpenValve()
            {
                var currentValve = GetCurrentValve();
                return !currentValve.IsOpen && currentValve.FlowRate != 0;
            }

            public ValveMap OpenValve()
            {
                var currentValve = GetCurrentValve();
                if (!CanOpenValve())
                {
                    throw new Exception($"Cannot open valve {_location}");
                }

                // Console.WriteLine($"{MinutesPassed} Opening {_location}");
                var replacementValve = new Valve(currentValve.Name, currentValve.FlowRate, currentValve.LeadsTo, true);

                ReleasePressure();
                return new ValveMap(this, new [] { replacementValve });
            }

            public int GetPressureReleased()
            {
                return _pressureReleased + (_base?.GetPressureReleased() ?? 0);
            }

            private void ReleasePressure()
            {
                _pressureReleased = _allLocations
                    .Sum(GetPressureReleasedAtLocation);

                // Console.WriteLine($"{MinutesPassed} Released Pressure {_pressureReleased}");
            }

            private int GetPressureReleasedAtLocation(string location)
            {
                var valve = GetValve(location);

                if (valve.IsOpen)
                {
                    return valve.FlowRate;
                }

                return 0;
            }
        }

        private class ShortestPaths
        {
            private readonly Dictionary<string, Dictionary<string, Path>> _shortestPaths;
            private readonly List<(string from, string to)> _possibleMoves;

            public ShortestPaths(List<(string from, string to)> possibleMoves)
            {
                _possibleMoves = possibleMoves.ToList();

                var allLocations = _possibleMoves.Select(pm => pm.from)
                    .Concat(possibleMoves.Select(pm => pm.to))
                    .Distinct()
                    .ToList();

                _shortestPaths = allLocations.ToDictionary(x => x, x => new Dictionary<string, Path>());
                foreach (var source in allLocations)
                {
                    foreach (var destination in allLocations)
                    {
                        if (source != destination)
                        {
                            _shortestPaths[source][destination] = GetShortestPath(source, destination, Enumerable.Empty<string>().ToList());
                        }
                    }
                }
            }

            public Path GetShortestPath(
                string source,
                string destination,
                List<string> visited)
            {
                if (_shortestPaths[source].ContainsKey(destination))
                {
                    return _shortestPaths[source][destination];
                }

                Path shortestPath = null;
                if (_possibleMoves.Any(p => p.from == source && p.to == destination))
                {
                    shortestPath = new Path(source, destination, Enumerable.Empty<string>());
                    _shortestPaths[source][destination] = shortestPath;
                    return shortestPath;
                }


                var possibleNextSteps = _possibleMoves
                    .Where(x => x.from == source)
                    .Select(x => x.to)
                    .ToList();

                foreach (var nextStep in possibleNextSteps)
                {
                    if (!visited.Contains(nextStep))
                    {
                        var pathForStep = GetShortestPath(nextStep, destination, visited.Concat(new[] { source }).ToList());
                        if (pathForStep != null)
                        {
                            if (shortestPath == null)
                            {
                                shortestPath = pathForStep;
                            }
                            else
                            {
                                if (pathForStep.Stops.Count < shortestPath.Stops.Count)
                                {
                                    shortestPath = pathForStep;
                                }
                            }
                        }
                    }
                }

                if (shortestPath == null)
                {
                    return null;
                }

                _shortestPaths[source][destination] = new Path(source, destination, new[] { shortestPath.Start }.Concat(shortestPath.Stops).ToList());
                return _shortestPaths[source][destination];
            }

            public string GetNextStep(string currentLocation, string destination)
            {
                var shortestPath = _shortestPaths[currentLocation][destination];

                if (shortestPath.Stops.Any())
                {
                    return shortestPath.Stops.First();
                }

                return shortestPath.End;
            }
        }

        private class Path
        {
            public string Start { get; }
            public string End { get; }
            public List<string> Stops { get; }

            public Path(string start, string end, IEnumerable<string> stops)
            {
                Start = start;
                End = end;
                Stops = stops.ToList();
            }

            public override string ToString()
            {
                if (Stops.Any())
                {
                    var stops = string.Join(",", Stops);
                    return $"{Start},{stops},{End} ({Stops.Count + 1})";
                }

                return $"{Start},{End} ({1})";
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Shared;

namespace AdventOfCode._2022.Day16
{
    public class Day16 : Day
    {
        private static Regex InputParser = new Regex("Valve (?<valve>.*) has flow rate=(?<flowrate>[0-9]*); tunnel[s]* lead[s]* to valve[s]* (?<leadsto>.*)");

        public Day16() : base(2022, 16, "Day16/input_2022_16.txt", "", "")
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
            var possibleMoves = _valves.SelectMany(v => v.LeadsTo.Select(lt => (v.Name, lt))).ToList();
            var shortestPaths = new ShortestPaths(possibleMoves);


            var findBestPath = new FindBestPath(shortestPaths, _valves, "AA");
            var maximumFlow = findBestPath.GetMaximumFlow();

            return maximumFlow.ToString();


            var valvemap = new ValveMap(shortestPaths, _valves, "AA");
            var possibleOutcomes = valvemap.GetAllPossibleOutcomes(30, null).ToList();
            var result = possibleOutcomes.Max(x => x.GetPressureReleased());
            return result.ToString();
        }

        private class FindBestPath
        {
            private Dictionary<string, Valve> _valves;
            private List<string> _allLocations;
            private readonly ShortestPaths _shortestPaths;
            private string _location;

            public FindBestPath(ShortestPaths shortestPaths, IEnumerable<Valve> valves, string location)
            {
                _shortestPaths = shortestPaths;
                _allLocations = valves.Select(v => v.Name).ToList();
                _valves = valves.ToDictionary(v => v.Name, v => v);
                _location = location;
            }

            public int GetMaximumFlow()
            {
                int timeAvailable = 30;
                int timeUsed = 0;
                int totalFlowDelivered = 0;

                while (timeUsed < timeAvailable)
                {
                    var maximumFlowValue = 0;
                    Path pathForMaximum = null;
                    int flowDurationForMaximum = 0;
                    int flowForMaximum = 0;

                    var worthAimingFor = WorthAimingFor();

                    foreach (var location in worthAimingFor)
                    {
                        var shortestPath = _shortestPaths.GetShortestPath(_location, location, new List<string>());
                        var timeUntilOpen = shortestPath.Stops.Count + 2;
                        var flowDuration = timeAvailable - timeUsed - timeUntilOpen;
                        var valveFlow = _valves[location].FlowRate;
                        var flowValue = flowDuration > 0 ? flowDuration * valveFlow : 0;
                        if (flowValue > maximumFlowValue)
                        {
                            maximumFlowValue = flowValue;
                            pathForMaximum = shortestPath;
                            flowDurationForMaximum = flowDuration;
                            flowForMaximum = valveFlow;
                        }
                    }

                    if (pathForMaximum != null)
                    {
                        foreach(var stop in pathForMaximum.Stops.Concat(new[] { pathForMaximum.End }))
                        {
                            timeUsed += 1;
                            _location = stop;
                            Console.WriteLine($"{timeUsed} Moved to {stop}");
                        }
                        timeUsed += 1;
                        OpenValve(pathForMaximum.End);
                        Console.WriteLine($"{timeUsed} Opened {pathForMaximum.End} to gain {maximumFlowValue} ({flowForMaximum} * {flowDurationForMaximum})");
                        totalFlowDelivered += maximumFlowValue;
                    }
                    else
                    {
                        timeUsed += 1;
                        Console.WriteLine($"{timeUsed} Do nothing");
                    }
                }


                return totalFlowDelivered;
            }

            private void OpenValve(string location)
            {
                var valve = _valves[location];
                _valves[location] = new Valve(valve.Name, valve.FlowRate, valve.LeadsTo, true);
            }

            public IEnumerable<string> WorthAimingFor()
            {
                foreach (var location in _allLocations)
                {
                    if (location != _location)
                    {
                        var valve = _valves[location];
                        if (!valve.IsOpen && valve.FlowRate > 0)
                        {
                            yield return valve.Name;
                        }
                    }
                }
            }

        }

        public override string Part2()
        {
            return "";
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

            private ValveMap _base;
            private List<string> _allLocations;
            private Dictionary<string, Valve> _overrides;
            private readonly ShortestPaths _shortestPaths;
            private string _location;
            private int _pressureReleased = 0;
            private string _logMessage;

            public ValveMap(ShortestPaths shortestPaths, IEnumerable<Valve> valves, string location)
            {
                _shortestPaths = shortestPaths;
                _overrides = valves.ToDictionary(v => v.Name, v => v);
                _allLocations = valves.Select(v => v.Name).ToList();
                _location = location;
                MinutesPassed = 0;
                _logMessage = "Start";
            }

            private ValveMap(ShortestPaths shortestPaths, ValveMap baseMap, Valve overrideValve)
            {
                _shortestPaths = shortestPaths;
                _base = baseMap;
                _overrides = new Dictionary<string, Valve>
                {
                    { overrideValve.Name, overrideValve}
                };
                _location = baseMap._location;
                _allLocations = baseMap._allLocations;
                MinutesPassed = baseMap.MinutesPassed + 1;
                _logMessage = $"Open {overrideValve.Name}";
            }

            private ValveMap(ShortestPaths shortestPaths, ValveMap baseMap, string location)
            {
                _shortestPaths = shortestPaths;
                _base = baseMap;
                _overrides = new Dictionary<string, Valve>();
                _location = location;
                _allLocations = baseMap._allLocations;
                MinutesPassed = baseMap.MinutesPassed + 1;
                _logMessage = $"Move to {location}";
            }

            public IEnumerable<ValveMap> GetAllPossibleOutcomes(int duration, string aimingFor)
            {
                if (MinutesPassed < duration)
                {
                    if (CanOpenValve() && aimingFor == _location)
                    {
                        // Console.WriteLine($"M{MinutesPassed} - Opening {_location}");
                        var openValve = OpenValve();
                        foreach (var outcome in openValve.GetAllPossibleOutcomes(duration, null))
                        {
                            yield return outcome;
                        }
                    }

                    if (aimingFor != null && aimingFor != _location)
                    {
                        var nextStep = _shortestPaths.GetNextStep(_location, aimingFor);
                        var movedTo = MoveTo(nextStep);
                        if (nextStep == aimingFor)
                        {
                            aimingFor = null;
                        }

                        foreach (var outcome in movedTo.GetAllPossibleOutcomes(duration, aimingFor))
                        {
                            yield return outcome;
                        }
                    }
                    else
                    {
                        foreach (var aimFor in WorthAimingFor())
                        {
                            Console.WriteLine($"M{MinutesPassed} - Aiming for {aimFor}");
                            var destination = _shortestPaths.GetNextStep(_location, aimFor);
                            var movedTo = MoveTo(destination);
                            foreach (var outcome in movedTo.GetAllPossibleOutcomes(duration, aimFor))
                            {
                                yield return outcome;
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Possibility {GetPressureReleased()}");
                }
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

            public IEnumerable<string> WorthMovingTo()
            {
                foreach(var location in _allLocations)
                {
                    if (location != _location)
                    {
                        var valve = GetValve(location);
                        if (!valve.IsOpen && valve.FlowRate > 0)
                        {
                            var nextStep = _shortestPaths.GetNextStep(_location, location);
                            yield return nextStep;
                        }
                    }
                }
            }

            public IEnumerable<string> WorthAimingFor()
            {
                foreach (var location in _allLocations)
                {
                    if (location != _location)
                    {
                        var valve = GetValve(location);
                        if (!valve.IsOpen && valve.FlowRate > 0)
                        {
                            yield return valve.Name;
                        }
                    }
                }
            }

            public ValveMap MoveTo(string location)
            {
                if (!GetCurrentValve().LeadsTo.Contains(location))
                {
                    throw new Exception($"Cannot move to {location}");
                }

                ReleasePressure();
                return new ValveMap(_shortestPaths, this, location);
            }

            public bool CanOpenValve()
            {
                var currentValve = GetCurrentValve();
                return !currentValve.IsOpen && currentValve.FlowRate != 0;
            }

            public ValveMap OpenValve()
            {
                var currentValve = GetCurrentValve();
                if (currentValve.IsOpen)
                {
                    throw new Exception($"Valve {_location} is already open");
                }
                var replacementValve = new Valve(currentValve.Name, currentValve.FlowRate, currentValve.LeadsTo, true);

                ReleasePressure();
                return new ValveMap(_shortestPaths, this, replacementValve);
            }

            public int GetPressureReleased()
            {
                return _pressureReleased + (_base?.GetPressureReleased() ?? 0);
            }

            private void ReleasePressure()
            {
                _pressureReleased = _allLocations
                    .Select(location => GetPressureReleasedAtLocation(location))
                    .Sum();
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
            Dictionary<string, Dictionary<string, Path>> _shortestPaths;
            List<(string from, string to)> _possibleMoves;

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

                foreach(var nextStep in possibleNextSteps)
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
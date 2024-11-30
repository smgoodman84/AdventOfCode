using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Shared;

namespace AdventOfCode._2022.Day16;

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
        var valvemap = new ValveMap(_valves, "AA", null, 30, new Objective());

        // var example = valvemap.FollowPath("DD", "BB", "JJ", "HH", "EE", "CC");
        var possibleOutcomes = valvemap.GetTopOutcomes(0).ToList();
        var result = possibleOutcomes.Max(x => x.GetPressureReleased());

        return result.ToString();
    }

    public override string Part2()
    {
        var valvemap = new ValveMap(_valves, "AA", "AA", 26, new Objective());

        var possibleOutcomes = valvemap.FollowObjective(15).ToList();
        var best = possibleOutcomes.OrderByDescending(x => x.GetPressureReleased()).First();
        /*
        var ordered = possibleOutcomes.OrderBy(x => x.ToString()).ToList();

        var log = best.GetFullLog();
        Console.WriteLine(log);


// JJ,BB,CC | DD,HH,EE
        var minutes = best.GetWhenMinutesPassed(3);
        var next = minutes.GetNextObjectiveStep(100);
        */
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

    private class Objective
    {
        public string NavigateToAndOpen { get; }
        public string NavigateElephantToAndOpen { get; }

        public Objective() : this(string.Empty, string.Empty)
        {
        }

        public Objective(string navigateToAndOpen, string navigateElephantToAndOpen)
        {
            NavigateToAndOpen = navigateToAndOpen;
            NavigateElephantToAndOpen = navigateElephantToAndOpen;
        }
    }

    private class ValveMap
    {
        public int MinutesPassed { get; }

        private readonly ValveMap _base;
        private readonly List<string> _allLocations;
        private readonly Dictionary<string, Valve> _overrides;
        private readonly ShortestPaths _shortestPaths;
        private readonly string _elephantLocation;
        private readonly string _location;
        private int _pressureReleased = 0;
        private int _timeAvailable;
        private readonly Objective _objective;
        private readonly string _logMessage;

        public ValveMap(
            IEnumerable<Valve> valves,
            string location,
            string elephantLocation,
            int timeAvailable,
            Objective objective)
        {
            var possibleMoves = valves.SelectMany(v => v.LeadsTo.Select(lt => (v.Name, lt))).ToList();
            _shortestPaths = new ShortestPaths(possibleMoves);
            _overrides = valves.ToDictionary(v => v.Name, v => v);
            _allLocations = valves.Select(v => v.Name).ToList();
            _location = location;
            _elephantLocation = elephantLocation;
            MinutesPassed = 0;
            _timeAvailable = timeAvailable;
            _objective = objective;
            _logMessage = "Start";
        }

        private ValveMap(
            ValveMap baseMap,
            IEnumerable<Valve> overrideValves,
            string location,
            string elephantLocation,
            Objective objective)
        {
            MinutesPassed = baseMap.MinutesPassed + 1;

            var logs = new List<string>
            {
                $"{baseMap.GetPressureReleased()} Pressure Released"
            };
            logs.AddRange(overrideValves.Select(o => $"Open {o.Name}"));
            if (baseMap._location != location)
            {
                logs.Add($"Move to {location}");
            }

            if (baseMap._elephantLocation != elephantLocation)
            {
                logs.Add($"Elephant move to {elephantLocation}");
            }

            _logMessage = logs.Any() ? string.Join(", ", logs) : "Do Nothing";
            _logMessage = $"{MinutesPassed} {_logMessage}";

            _shortestPaths = baseMap._shortestPaths;
            _base = baseMap;
            _overrides = overrideValves.ToDictionary(o => o.Name, o => o);
            _location = location;
            _elephantLocation = elephantLocation;
            _allLocations = baseMap._allLocations;
            _timeAvailable = baseMap._timeAvailable;
            _objective = objective;
        }

        private ValveMap(ValveMap baseMap)
            : this(
                baseMap,
                Enumerable.Empty<Valve>(),
                baseMap._location,
                baseMap._elephantLocation,
                baseMap._objective)
        {

        }

        private ValveMap(
            ValveMap baseMap,
            IEnumerable<Valve> overrideValves,
            Objective objective)
            : this(
                baseMap,
                overrideValves,
                baseMap._location,
                baseMap._elephantLocation,
                objective)
        {

        }

        private ValveMap(
            ValveMap baseMap,
            string location,
            string elephantLocation,
            Objective objective)
            : this(
                baseMap,
                Enumerable.Empty<Valve>(),
                location,
                elephantLocation,
                objective)
        {

        }

        public ValveMap GetWhenMinutesPassed(int minutes)
        {
            var current = this;
            while (current.MinutesPassed > minutes)
            {
                current = current._base;
            }

            return current;
        }

        private string GetElephantObjective()
        {
            var objectives = new List<string>();

            var currentObjective = string.Empty;
            var current = this;
            while (current != null)
            {
                var objective = current._objective.NavigateElephantToAndOpen;
                if (objective != string.Empty
                    && objective != currentObjective)
                {
                    currentObjective = objective;
                    objectives.Add(objective);
                }
                current = current._base;
            }

            objectives.Reverse();
            return string.Join(",", objectives);
        }

        private string GetObjective()
        {
            var objectives = new List<string>();

            var currentObjective = string.Empty;
            var current = this;
            while (current != null)
            {
                var objective = current._objective.NavigateToAndOpen;
                if (objective != string.Empty
                    && objective != currentObjective)
                {
                    currentObjective = objective;
                    objectives.Add(objective);
                }
                current = current._base;
            }

            objectives.Reverse();
            return string.Join(",", objectives);
        }

        public override string ToString()
        {
            return $"{GetObjective()} | {GetElephantObjective()}";
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

            return NavigateToLocationAndOpenValve(locations.First())
                .FollowPath(locations.Skip(1).ToArray());
        }

        public IEnumerable<ValveMap> GetTopOutcomes(int depth)
        {
            var topDestinations = TopDestinations(_location, depth).ToList();

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
        private Valve GetCurrentElephantValve() => GetValve(_elephantLocation);

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

        public IEnumerable<string> TopDestinations(string location, int count, IEnumerable<string> exclusions = null)
        {
            var potentialValves = _allLocations
                .Where(l => l != location)
                .Where(l => exclusions == null || !exclusions.Contains(l))
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

        private int GetPotentialFlowRate(string currentLocation, string location)
        {
            var shortestPath = _shortestPaths.GetShortestPath(currentLocation, location, new List<string>());
            var timeUntilOpen = shortestPath.Stops.Count + 2;
            var flowDuration = _timeAvailable - MinutesPassed - timeUntilOpen;
            var valveFlow = GetValve(location).FlowRate;
            var flowValue = flowDuration > 0 ? flowDuration * valveFlow : 0;
            return flowValue;
        }

        public IEnumerable<ValveMap> FollowObjective(int count)
        {
            var nextSteps = GetNextObjectiveStep(count).ToList();
            var completedSteps = nextSteps.Where(x => x.MinutesPassed == _timeAvailable);
            var incompleteSteps = nextSteps.Where(x => x.MinutesPassed < _timeAvailable);
            var completedIncomplete = incompleteSteps.SelectMany(x => x.FollowObjective(count));

            return completedSteps.Concat(completedIncomplete);
        }

        public IEnumerable<ValveMap> GetNextObjectiveStep(int count)
        {
            ReleasePressure();


            // JJ,BB,CC | DD,HH,EE
            var destination = _objective.NavigateToAndOpen;
            var elephantDestination = _objective.NavigateElephantToAndOpen;
            var openedValve = false;
            var elephantOpenedValve = false;
            var openedValves = new List<string>();

            if (GetObjective().StartsWith("JJ,BB") && GetElephantObjective().StartsWith("DD"))
            {
                var log = GetFullLog();
                var stop = "here";
            }

            var valveOverrides = new List<Valve>();
            if (destination != string.Empty &&
                _location == destination)
            {
                var currentValve = GetCurrentValve();
                if (CanOpenValve())
                {
                    // Console.WriteLine($"{MinutesPassed} Opening {_location}");
                    var replacementValve = new Valve(currentValve.Name, currentValve.FlowRate, currentValve.LeadsTo, true);
                    valveOverrides.Add(replacementValve);
                    openedValves.Add(currentValve.Name);
                    destination = string.Empty;
                    openedValve = true;
                }
            }

            if (elephantDestination != string.Empty &&
                _elephantLocation == elephantDestination)
            {
                var currentValve = GetCurrentElephantValve();
                if (CanElephantOpenValve())
                {
                    // Console.WriteLine($"{MinutesPassed} Opening {_location}");
                    var replacementValve = new Valve(currentValve.Name, currentValve.FlowRate, currentValve.LeadsTo, true);
                    valveOverrides.Add(replacementValve);
                    openedValves.Add(currentValve.Name);
                    elephantDestination = string.Empty;
                    elephantOpenedValve = true;
                }
            }

            if (destination == string.Empty && elephantDestination == string.Empty)
            {
                var locations = TopDestinations(_location, count, openedValves).ToList();
                var elephantLocations = TopDestinations(_elephantLocation, count, openedValves).ToList();

                var anyLocations = false;
                var anyElephantLocations = false;

                var possibilities = new List<(ValveMap, int)>();
                foreach (var l in locations)
                {
                    anyLocations = true;
                    foreach (var e in elephantLocations)
                    {
                        if (l != e)
                        {
                            anyElephantLocations = true;
                            var nextStep = _shortestPaths.GetNextStep(_location, l);
                            var newLocation = openedValve ? _location : nextStep;

                            var elephantNextStep = _shortestPaths.GetNextStep(_elephantLocation, e);
                            var newElephantLocation = elephantOpenedValve ? _elephantLocation : elephantNextStep;

                            var potentialFlow = GetPotentialFlowRate(_location, l)
                                                + GetPotentialFlowRate(_elephantLocation, e);

                            possibilities.Add((new ValveMap(this, valveOverrides, newLocation, newElephantLocation, new Objective(l, e)), potentialFlow));
                        }
                    }
                }

                foreach(var possibility in possibilities.OrderByDescending(x => x.Item2).Take(count))
                {
                    yield return possibility.Item1;
                }

                if (!anyLocations)
                {
                    foreach (var e in elephantLocations)
                    {
                        anyElephantLocations = true;

                        var elephantNextStep = _shortestPaths.GetNextStep(_elephantLocation, e);
                        var newElephantLocation = elephantOpenedValve ? _elephantLocation : elephantNextStep;

                        yield return new ValveMap(this, valveOverrides, _location, newElephantLocation, new Objective(_location, e));
                    }
                }

                if (!anyElephantLocations)
                {
                    foreach (var l in locations)
                    {
                        var nextStep = _shortestPaths.GetNextStep(_location, l);
                        var newLocation = openedValve ? _location : nextStep;

                        yield return new ValveMap(this, valveOverrides, newLocation, _elephantLocation, new Objective(l, _elephantLocation));
                    }
                }
            }
            else if (destination == string.Empty)
            {
                var locations = TopDestinations(_location, count, openedValves);
                var anyLocations = false;

                var newElephantLocation = _elephantLocation;
                if (!elephantOpenedValve && elephantDestination != _elephantLocation)
                {
                    newElephantLocation = _shortestPaths.GetNextStep(_elephantLocation, elephantDestination);
                }

                foreach (var l in locations.Where(l => l != elephantDestination))
                {
                    anyLocations = true;
                    var nextStep = _shortestPaths.GetNextStep(_location, l);
                    var newLocation = openedValve ? _location : nextStep;

                    yield return new ValveMap(this, valveOverrides, newLocation, newElephantLocation, new Objective(l, elephantDestination));
                }

                if (!anyLocations)
                {
                    yield return new ValveMap(this, valveOverrides, _location, newElephantLocation, new Objective(_location, elephantDestination));
                }
            }
            else if (elephantDestination == string.Empty)
            {
                var elephantLocations = TopDestinations(_elephantLocation, count, openedValves);
                var anyLocations = false;

                var newLocation = _location;
                if (!openedValve && destination != _location)
                {
                    newLocation = _shortestPaths.GetNextStep(_location, destination);
                }

                foreach (var e in elephantLocations.Where(e => e != destination))
                {
                    anyLocations = true;
                    var elephantNextStep = _shortestPaths.GetNextStep(_elephantLocation, e);
                    var newElephantLocation = elephantOpenedValve ? _elephantLocation : elephantNextStep;

                    yield return new ValveMap(this, valveOverrides, newLocation, newElephantLocation, new Objective(destination, e));
                }

                if (!anyLocations)
                {
                    yield return new ValveMap(this, valveOverrides, newLocation, _elephantLocation, new Objective(destination, _elephantLocation));
                }
            }
            else
            {
                var newLocation = _location;
                if (!openedValve && destination != _location)
                {
                    newLocation = _shortestPaths.GetNextStep(_location, destination);
                }

                var newElephantLocation = _elephantLocation;
                if (!elephantOpenedValve && elephantDestination != _elephantLocation)
                {
                    newElephantLocation = _shortestPaths.GetNextStep(_elephantLocation, elephantDestination);
                }

                yield return new ValveMap(this, valveOverrides, newLocation, newElephantLocation, new Objective(destination, elephantDestination));
            }
        }

        public ValveMap NavigateToLocationAndOpenValve(string location)
        {
            if (CanMoveDirectlyTo(location))
            {
                return MoveTo(location).OpenValve();
            }

            var nextStep = _shortestPaths.GetNextStep(_location, location);
            return MoveTo(nextStep).NavigateToLocationAndOpenValve(location);
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
            return new ValveMap(this, location, _elephantLocation, this._objective);
        }

        public bool CanElephantOpenValve()
        {
            var currentValve = GetCurrentElephantValve();
            return !currentValve.IsOpen && currentValve.FlowRate != 0;
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
            return new ValveMap(this, new [] { replacementValve }, this._objective);
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

        internal string GetFullLog()
        {
            return $"{_base?.GetFullLog()}{Environment.NewLine}{_logMessage}";
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
            if (source == null)
            {
                var stop = true;
            }

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
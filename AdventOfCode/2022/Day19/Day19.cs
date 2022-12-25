using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2022.Day19
{
    public class Day19 : Day
    {
        private static readonly Regex BlueprintRegex = new Regex("Blueprint (?<blueprint>[0-9]*): (?<robotcosts>.*)");
        private static readonly Regex RobotRegex = new Regex("Each (?<produces>.*) robot costs (?<cost>.*)");

        public Day19() : base(2022, 19, "Day19/input_2022_19.txt", "", "")
        {

        }

        private List<Blueprint> _blueprints;
        public override void Initialise()
        {
            _blueprints = InputLines
                .Select(ParseBlueprint)
                .ToList();
        }

        private Blueprint ParseBlueprint(string description)
        {
            var match = BlueprintRegex.Match(description);
            var id = int.Parse(match.Groups["blueprint"].Value);

            var robots = match
                .Groups["robotcosts"]
                .Value
                .Split(".")
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => ParseRobot(x.Trim()))
                .ToList();

            return new Blueprint(id, robots);
        }

        private Robot ParseRobot(string description)
        {
            var match = RobotRegex.Match(description);
            var produces = ParseMineral(match.Groups["produces"].Value);
            var costs = match
                .Groups["cost"]
                .Value
                .Split(" and ")
                .Select(ParseCost)
                .ToList();

            return new Robot(produces, costs);
        }

        private Mineral ParseMineral(string mineral)
        {
            switch (mineral.Trim().ToLower())
            {
                case "ore": return Mineral.Ore;
                case "clay": return Mineral.Clay;
                case "obsidian": return Mineral.Obsidian;
                case "geode": return Mineral.Geode;
            }

            throw new Exception($"Unrecognised mineral: {mineral}");
        }

        private MineralCost ParseCost(string description)
        {
            var split = description.Split(" ");

            var cost = int.Parse(split[0]);
            var material = ParseMineral(split[1]);

            return new MineralCost(material, cost);
        }

        public override string Part1()
        {
            var bestOutcome = _blueprints
                .Select(bp => (Blueprint: bp, Geodes: GetMaximumGeodesFromBlueprint(bp, 24)))
                .OrderByDescending(x => x.Geodes)
                .First();

            var result = bestOutcome.Blueprint.Id * bestOutcome.Geodes;

            return result.ToString();
        }

        public override string Part2()
        {
            return "";
        }

        private class Blueprint
        {
            public int Id { get; }
            public List<Robot> Robots { get; }

            public Blueprint(int id, IEnumerable<Robot> robots)
            {
                Id = id;
                Robots = robots.ToList();
            }
        }

        private class Robot
        {
            public Mineral Produces { get; }
            public List<MineralCost> Costs { get; }

            public Robot(Mineral produces, IEnumerable<MineralCost> costs)
            {
                Produces = produces;
                Costs = costs.ToList();
            }
        }

        private class MineralCost
        {
            public Mineral Mineral { get; }
            public int Cost { get; }

            public MineralCost(Mineral mineral, int cost)
            {
                Mineral = mineral;
                Cost = cost;
            }
        }

        private enum Mineral
        {
            Ore,
            Clay,
            Obsidian,
            Geode
        }

        private int GetMaximumGeodesFromBlueprint(Blueprint blueprint, int duration)
        {
            var initialState = new MineralState(blueprint, duration);

            var possibleOutcomes = initialState
                .TryAndProduce(Mineral.Geode)
                .ToList();

            var bestOutcome = possibleOutcomes
                .OrderByDescending(x => x.Minerals[Mineral.Geode])
                .First();

            return bestOutcome.Minerals[Mineral.Geode];
        }

        private class MineralState
        {
            public MineralState First => _previous?.First ?? this;

            private MineralState _previous;
            public Blueprint Blueprint { get; }
            public int TimeRemaining { get; }
            public Dictionary<Mineral, int> Minerals { get; }
            public Dictionary<Mineral, int> Robots { get; }

            public MineralState(Blueprint blueprint, int duration)
            {
                Blueprint = blueprint;
                TimeRemaining = duration;
                Minerals = new Dictionary<Mineral, int>()
                {
                    { Mineral.Clay, 0 },
                    { Mineral.Ore, 0 },
                    { Mineral.Obsidian, 0 },
                    { Mineral.Geode, 0 }
                };
                Robots = new Dictionary<Mineral, int>
                {
                    { Mineral.Clay, 0 },
                    { Mineral.Ore, 1 },
                    { Mineral.Obsidian, 0 },
                    { Mineral.Geode, 0 }
                };
            }

            public override string ToString()
            {
                return $"{TimeRemaining} | {Summarise(Minerals)} | {Summarise(Robots)}";
            }

            private string Summarise(Dictionary<Mineral, int> dict)
            {
                return string.Join(", ", dict.Select(d => Summarise(d.Key, d.Value)));
            }
            private string Summarise(Mineral mineral, int count)
            {
                switch (mineral)
                {
                    case Mineral.Ore: return $"Or {count}";
                    case Mineral.Obsidian: return $"Ob {count}";
                    case Mineral.Clay: return $"C {count}";
                    case Mineral.Geode: return $"G {count}";
                }

                return "";
            }

            private MineralState(
                MineralState previous,
                Blueprint blueprint,
                int duration,
                Dictionary<Mineral, int> minerals,
                Dictionary<Mineral, int> robots
                )
            {
                _previous = previous;
                Blueprint = blueprint;
                TimeRemaining = duration;
                Minerals = minerals;
                Robots = robots;
            }

            private MineralState ProduceMinerals()
            {
                var minerals = Minerals.ToDictionary(m => m.Key, m => m.Value);
                foreach (var mineral in Robots)
                {
                    if (minerals.ContainsKey(mineral.Key))
                    {
                        minerals[mineral.Key] += mineral.Value;
                    }
                    else
                    {
                        minerals[mineral.Key] = mineral.Value;
                    }
                }

                return new MineralState(this, Blueprint, TimeRemaining - 1, minerals, Robots);
            }


            private bool CanProduceRobot(Mineral mineral)
            {
                var robot = Blueprint.Robots.Single(x => x.Produces == mineral);

                foreach (var cost in robot.Costs)
                {
                    if (Minerals[cost.Mineral] < cost.Cost)
                    {
                        return false;
                    }
                }

                return true;
            }

            public IEnumerable<MineralState> TryAndProduce(params Mineral[] minerals)
            {
                //var mineral = minerals.First();

                var producedRobot = false;
                for (var i=0; i<minerals.Length; i++)
                {
                    var needs = NeededToProduceRobot(minerals[i]);

                    if (!needs.Any())
                    {
                        var next = ProduceRobot(minerals[i]).ProduceMinerals();
                        producedRobot = true;
                        if (next.TimeRemaining > 0)
                        {
                            foreach (var end in next.TryAndProduce(minerals.Take(i+1).ToArray()))
                            {
                                yield return end;
                            }
                        }
                        else
                        {
                            yield return next;
                        }
                    }
                }

                var additionalNeed = false;
                if (!producedRobot)
                {
                    for (var i = 0; i < minerals.Length; i++)
                    {
                        var needs = NeededToProduceRobot(minerals[i]);

                        if (needs.Any())
                        {
                            foreach (var need in needs.Where(n => !minerals.Contains(n.Key)).OrderByDescending(n => n.Value))
                            {
                                additionalNeed = true;
                                foreach (var end in TryAndProduce(minerals.Concat(new[] { need.Key }).ToArray()))
                                {
                                    yield return end;
                                }
                            }
                        }
                    }
                }

                if (!producedRobot && !additionalNeed)
                {
                    var next = ProduceMinerals();
                    if (next.TimeRemaining > 0)
                    {
                        foreach (var end in next.TryAndProduce(minerals))
                        {
                            yield return end;
                        }
                    }
                    else
                    {
                        yield return next;
                    }
                }
            }

            private Dictionary<Mineral, int> NeededToProduceRobot(Mineral mineral)
            {
                var result = new Dictionary<Mineral, int>();

                var robot = Blueprint.Robots.Single(x => x.Produces == mineral);

                foreach (var cost in robot.Costs)
                {
                    if (Minerals[cost.Mineral] < cost.Cost)
                    {
                        result.Add(cost.Mineral, cost.Cost - Minerals[cost.Mineral]);
                    }
                }

                return result;
            }

            private MineralState ProduceRobot(Mineral mineral)
            {
                if (!CanProduceRobot(mineral))
                {
                    throw new Exception($"Cannot produce {mineral}");
                }

                var minerals = Minerals.ToDictionary(m => m.Key, m => m.Value);
                var robots = Robots.ToDictionary(m => m.Key, m => m.Value);

                var robot = Blueprint.Robots.Single(x => x.Produces == mineral);

                foreach (var cost in robot.Costs)
                {
                    minerals[cost.Mineral] -= cost.Cost;
                }

                robots[mineral] += 1;

                return new MineralState(
                    this,
                    Blueprint,
                    TimeRemaining,
                    minerals,
                    robots);
            }

            public IEnumerable<MineralState> GetPossibleEndStates()
            {
                var any = false;
                foreach (var nextState in GetPossibleNextStates().Take(15))
                {
                    any = true;
                    foreach (var endState in nextState.GetPossibleEndStates())
                    {
                        yield return endState;
                    }
                }

                if (!any)
                {
                    Console.WriteLine($"Produced {Minerals[Mineral.Geode]} Geodes");
                    yield return this;
                }
            }

            public IEnumerable<MineralState> GetPossibleNextStates()
            {
                if (TimeRemaining == 0)
                {
                    return Enumerable.Empty<MineralState>();
                }

                var result = new List<MineralState>();

                foreach (var robot in Blueprint.Robots.OrderByDescending(r => r.Produces))
                {
                    if (CanProduceRobot(robot.Produces))
                    {
                        result.Add(ProduceRobot(robot.Produces).ProduceMinerals());
                    }
                }

                result.Add(ProduceMinerals()); // Don't build a robot

                return result;
            }
        }
    }
}

using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;
using AdventOfCode.Shared.Numbers;

namespace AdventOfCode._2023.Day19
{
    public class Day19 : Day
    {
        public Day19() : base(2023, 19, "Day19/input_2023_19.txt", "333263", "130745440937650", true)
        {

        }

        private List<Part> _parts;
        public override void Initialise()
        {
            var groups = LineGrouper.GroupLinesBySeperator(InputLines);

            foreach (var workflow in groups[0])
            {
                Workflow.AddWorkflow(workflow);
            }

            _parts = groups[1]
                .Select(l => new Part(l))
                .ToList();
        }

        public override string Part1()
        {
            var inWorkflow = Workflow.Workflows["in"];

            var result = 0;
            foreach (var part in _parts)
            {
                var ranges = part.Ratings.ToDictionary(kvp => kvp.Key, kvp => new NumberRange(kvp.Value, kvp.Value));

                long sum = GetAcceptedCount(ranges);
                if (sum > 0)
                {
                    result += part.RatingSum();
                }
            }

            return result.ToString();
        }

        public override string Part2()
        {
            var ranges = "xmas".ToDictionary(c => c.ToString(), kvp => new NumberRange(1, 4000));

            long sum = GetAcceptedCount(ranges);

            return sum.ToString();
        }

        private static long GetAcceptedCount(Dictionary<string, NumberRange> ranges)
        {
            var inWorkflow = Workflow.Workflows["in"];

            var partCollection = new PartRanges(ranges, State.Continue);
            var results = inWorkflow.ProcessPart(partCollection);

            long sum = 0;
            foreach (var result in results)
            {
                var acceptedCount = result.GetAcceptedCount();
                sum += acceptedCount;
            }

            return sum;
        }

        private class Workflow
        {
            public static Dictionary<string, Workflow> Workflows = new Dictionary<string, Workflow>();
            public string Name { get; set; }

            private List<Func<PartRanges, List<PartRanges>>> _predicates;
            private Workflow(string description)
            {
                var nameSplit = description.Split("{");
                Name = nameSplit[0];

                var predicates = nameSplit[1]
                    .Replace("}", "")
                    .Split(",");

                _predicates = predicates
                    .Select(ParsePredicate)
                    .ToList();
            }

            public static void AddWorkflow(string description)
            {
                var workflow = new Workflow(description);
                Workflows.Add(workflow.Name, workflow);
            }


            private Func<PartRanges, List<PartRanges>> SetState(State state)
            {
                return p => {
                    p.State = state;
                    return new List<PartRanges>
                        {
                            p
                        };
                };
            }
            private Func<PartRanges, List<PartRanges>> RunWorkflow(string workflow)
            {
                if (workflow == "A")
                {
                    return SetState(State.Accept);
                }

                if (workflow == "R")
                {
                    return SetState(State.Reject);
                }

                return p =>
                {
                    // Console.WriteLine($"Processing with workflow {destinationName}");
                    return Workflows[workflow].ProcessPart(p);
                };
            }

            private Func<PartRanges, List<PartRanges>> ParsePredicate(string predicate)
            {
                if (!predicate.Contains(":"))
                {
                    return RunWorkflow(predicate);
                }

                var split = predicate.Split(":");
                var trueWorkflowName = split[1];
                var trueWorkflow = RunWorkflow(trueWorkflowName);

                var condition = split[0];
                if (condition.Contains("<"))
                {
                    var ltSplit = condition.Split("<");
                    var ltProperty = ltSplit[0];
                    var ltValue = int.Parse(ltSplit[1]);
                    return p =>
                    {
                        // Console.WriteLine($"Splitting {ltProperty} Less Than {ltValue}");
                        var splitCollections = new List<PartRanges>();

                        var lessThan = p.LessThan(ltProperty, ltValue);
                        if (lessThan != null)
                        {
                            splitCollections.AddRange(trueWorkflow(lessThan));
                        }

                        var greaterThanOrEqual = p.GreaterThanOrEqual(ltProperty, ltValue);
                        if (greaterThanOrEqual != null)
                        {
                            splitCollections.Add(greaterThanOrEqual);
                        }

                        return splitCollections;
                    };
                }


                if (condition.Contains(">"))
                {
                    var gtSplit = condition.Split(">");
                    var gtProperty = gtSplit[0];
                    var gtValue = int.Parse(gtSplit[1]);
                    return p =>
                    {
                        // Console.WriteLine($"Splitting {gtProperty} Greater Than {gtValue}");
                        var splitCollections = new List<PartRanges>();

                        var lessThanOrEqual = p.LessThanOrEqual(gtProperty, gtValue);
                        if (lessThanOrEqual != null)
                        {
                            splitCollections.Add(lessThanOrEqual);
                        }

                        var greaterThan = p.GreaterThan(gtProperty, gtValue);
                        if (greaterThan != null)
                        {
                            splitCollections.AddRange(trueWorkflow(greaterThan));
                        }

                        return splitCollections;
                    };
                }

                return SetState(State.Unknown);
            }

            public List<PartRanges> ProcessPart(PartRanges part)
            {
                var incomplete = new List<PartRanges>()
                {
                    part
                };

                // Console.WriteLine($"Processing {part} with workflow {Name}");
                var complete = new List<PartRanges>();

                foreach (var predicate in _predicates)
                {
                    var newIncomplete = new List<PartRanges>();
                    foreach(var currentPart in incomplete)
                    {
                        var predicateResults = predicate(currentPart);
                        foreach (var predicateResult in predicateResults)
                        {
                            if (predicateResult.State == State.Continue)
                            {
                                newIncomplete.Add(predicateResult);
                            }
                            else
                            {
                                complete.Add(predicateResult);
                            }
                        }
                    }
                    incomplete = newIncomplete;
                }

                return complete;
            }
        }

        private class Part
        {
            public Dictionary<string, int> Ratings = new Dictionary<string, int>();
            public Part(string description)
            {
                var ratings = description
                    .Replace("{", "")
                    .Replace("}", "")
                    .Split(",");

                foreach (var rating in ratings)
                {
                    AddRating(rating);
                }
            }

            private void AddRating(string rating)
            {
                var split = rating.Split('=');
                Ratings.Add(split[0], int.Parse(split[1]));
            }

            public int RatingSum()
            {
                return Ratings.Values.Sum();
            }
        }

        private class PartRanges
        {
            public Dictionary<string, NumberRange> RatingRanges;
            public State State { get; set; }

            public PartRanges(Dictionary<string, NumberRange> ratingRanges, State state)
            {
                RatingRanges = ratingRanges;
                State = state;
            }

            public long GetAcceptedCount()
            {
                if (State != State.Accept)
                {
                    return 0;
                }

                long product = 1;
                foreach(var range in RatingRanges.Values)
                {
                    product *= range.Length;
                }
                return product;
            }

            public PartRanges? LessThan(string ratingName, int lessThan)
            {
                var range = RatingRanges[ratingName];
                if (range.Start >= lessThan)
                {
                    return null;
                }

                var newRanges = RatingRanges.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                newRanges[ratingName] = range.LessThan(lessThan);

                return new PartRanges(newRanges, State);
            }

            public PartRanges? GreaterThanOrEqual(string ratingName, int greaterThanOrEqual)
            {
                var range = RatingRanges[ratingName];
                if (range.End < greaterThanOrEqual)
                {
                    return null;
                }

                var newRanges = RatingRanges.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                newRanges[ratingName] = range.GreaterThanOrEqual(greaterThanOrEqual);

                return new PartRanges(newRanges, State);
            }

            public PartRanges? GreaterThan(string ratingName, int greaterThan)
            {
                var range = RatingRanges[ratingName];
                if (range.End <= greaterThan)
                {
                    return null;
                }

                var newRanges = RatingRanges.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                newRanges[ratingName] = range.GreaterThan(greaterThan);

                return new PartRanges(newRanges, State);
            }

            public PartRanges? LessThanOrEqual(string ratingName, int lessThanOrEqual)
            {
                var range = RatingRanges[ratingName];
                if (range.Start > lessThanOrEqual)
                {
                    return null;
                }

                var newRanges = RatingRanges.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                newRanges[ratingName] = range.LessThanOrEqual(lessThanOrEqual);

                return new PartRanges(newRanges, State);
            }

            public override string ToString()
            {
                var x = RatingRanges["x"];
                var m = RatingRanges["m"];
                var a = RatingRanges["a"];
                var s = RatingRanges["s"];
                return $"{State} - x: {x}, m: {m}, a: {a}, s: {s}";
            }
        }

        private enum State
        {
            Unknown,
            Continue,
            Accept,
            Reject
        }
    }
}

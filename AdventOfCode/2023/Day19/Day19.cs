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
                var accepted = inWorkflow.IsAccepted(part);
                if (accepted)
                {
                    result += part.RatingSum();
                }
            }

            return result.ToString();
        }

        public override string Part2()
        {
            var inWorkflow = Workflow.Workflows["in"];

            var ranges = new Dictionary<string, NumberRange>
            {
                { "x", new NumberRange(1, 4000) },
                { "m", new NumberRange(1, 4000) },
                { "a", new NumberRange(1, 4000) },
                { "s", new NumberRange(1, 4000) },
            };
            var partCollection = new PartCollection(ranges, State.Continue);
            var results = inWorkflow.ProcessPart(partCollection);

            long sum = 0;
            foreach(var result in results)
            {
                var acceptedCount = result.GetAcceptedCount();
                sum += acceptedCount;
            }

            return sum.ToString();
        }

        private class Workflow
        {
            public static Dictionary<string, Workflow> Workflows = new Dictionary<string, Workflow>();
            public string Name { get; set; }

            private List<Func<Part, State>> _predicates = new List<Func<Part, State>>();
            private List<Func<PartCollection, List<PartCollection>>> _collectionPredicates = new List<Func<PartCollection, List<PartCollection>>>();
            private Workflow(string description)
            {
                var nameSplit = description.Split("{");
                Name = nameSplit[0];

                var predicates = nameSplit[1].Replace("}", "").Split(",");
                foreach (var predicate in predicates)
                {
                    _predicates.Add(ParsePredicate(predicate));
                    _collectionPredicates.Add(ParsePredicateCollection(predicate));
                }
            }

            public static void AddWorkflow(string description)
            {
                var workflow = new Workflow(description);
                Workflows.Add(workflow.Name, workflow);
            }

            private Func<Part, State> GetDestinationProcess(string destinationName)
            {
                if (destinationName == "A")
                {
                    return p => State.Accept;
                }

                if (destinationName == "R")
                {
                    return p => State.Reject;
                }

                return p =>
                {
                    return Workflows[destinationName].ProcessPart(p);
                };
            }

            private Func<PartCollection, List<PartCollection>> GetDestinationProcessCollection(string destinationName)
            {
                if (destinationName == "A")
                {
                    return p => {
                        p.State = State.Accept;
                        Console.WriteLine($"Accepted {p}");
                        return new List<PartCollection>
                        {
                            p
                        };
                    };
                }

                if (destinationName == "R")
                {
                    return p => {
                        p.State = State.Reject;
                        Console.WriteLine($"Rejected {p}");
                        return new List<PartCollection>
                        {
                            p
                        };
                    };
                }

                return p =>
                {
                    // Console.WriteLine($"Processing with workflow {destinationName}");
                    return Workflows[destinationName].ProcessPart(p);
                };
            }

            private Func<Part, State> ParsePredicate(string predicate)
            {
                if (!predicate.Contains(":"))
                {
                    return GetDestinationProcess(predicate);
                }

                var split = predicate.Split(":");
                var trueDestinationName = split[1];
                var trueDestination = GetDestinationProcess(trueDestinationName);

                var condition = split[0];
                if (condition.Contains("<"))
                {
                    var ltSplit = condition.Split("<");
                    var ltProperty = ltSplit[0];
                    var ltValue = int.Parse(ltSplit[1]);
                    return p => LessThan(p, ltProperty, ltValue, trueDestination);
                }


                if (condition.Contains(">"))
                {
                    var gtSplit = condition.Split(">");
                    var gtProperty = gtSplit[0];
                    var gtValue = int.Parse(gtSplit[1]);
                    return p => GreaterThan(p, gtProperty, gtValue, trueDestination);
                }

                return p => State.Unknown;
            }



            private Func<PartCollection, List<PartCollection>> ParsePredicateCollection(string predicate)
            {
                if (!predicate.Contains(":"))
                {
                    return GetDestinationProcessCollection(predicate);
                }

                var split = predicate.Split(":");
                var trueDestinationName = split[1];
                var trueDestination = GetDestinationProcessCollection(trueDestinationName);

                var condition = split[0];
                if (condition.Contains("<"))
                {
                    var ltSplit = condition.Split("<");
                    var ltProperty = ltSplit[0];
                    var ltValue = int.Parse(ltSplit[1]);
                    return p =>
                    {
                        Console.WriteLine($"Splitting {ltProperty} Less Than {ltValue}");
                        var split = p.SplitLessThan(ltProperty, ltValue);
                        var splitCollections = new List<PartCollection>();

                        if (split.LessThan != null)
                        {
                            splitCollections.AddRange(trueDestination(split.LessThan));
                        }

                        if (split.GreaterThanOrEqual != null)
                        {
                            splitCollections.Add(split.GreaterThanOrEqual);
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
                        Console.WriteLine($"Splitting {gtProperty} Greater Than {gtValue}");
                        var split = p.SplitGreaterThan(gtProperty, gtValue);
                        var splitCollections = new List<PartCollection>();

                        if (split.LessThanOrEqual != null)
                        {
                            splitCollections.Add(split.LessThanOrEqual);
                        }

                        if (split.GreaterThan != null)
                        {
                            splitCollections.AddRange(trueDestination(split.GreaterThan));
                        }

                        return splitCollections;
                    };
                }

                return p => {
                    p.State = State.Unknown;
                    return new List<PartCollection>
                        {
                            p
                        };
                };
            }

            private State LessThan(Part part, string property, int value, Func<Part, State> trueDestination)
            {
                if (part.Ratings[property] < value)
                {
                    return trueDestination(part);
                }

                return State.Continue;
            }

            private State GreaterThan(Part part, string property, int value, Func<Part, State> trueDestination)
            {
                if (part.Ratings[property] > value)
                {
                    return trueDestination(part);
                }

                return State.Continue;
            }

            private State ProcessPart(Part part)
            {
                foreach (var predicate in _predicates)
                {
                    var predicateResult = predicate(part);
                    if (predicateResult != State.Continue)
                    {
                        return predicateResult;
                    }
                }

                return State.Unknown;
            }

            public List<PartCollection> ProcessPart(PartCollection part)
            {
                var incomplete = new List<PartCollection>()
                {
                    part
                };
                Console.WriteLine($"Processing {part} with workflow {Name}");
                var complete = new List<PartCollection>();

                foreach (var predicate in _collectionPredicates)
                {
                    var newIncomplete = new List<PartCollection>();
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

            public bool IsAccepted(Part part)
            {
                var result = ProcessPart(part);
                return result == State.Accept;
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

        private class PartCollection
        {
            public Dictionary<string, NumberRange> RatingRanges;
            public State State { get; set; }

            public PartCollection(Dictionary<string, NumberRange> ratingRanges, State state)
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

            public (PartCollection? LessThan, PartCollection? GreaterThanOrEqual) SplitLessThan(string ratingName, int lessThan)
            {
                var range = RatingRanges[ratingName];

                if (range.End < lessThan)
                {
                    return (this, null);
                }

                if (range.Start > lessThan)
                {
                    return (null, this);
                }

                var lessThanRanges = RatingRanges.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                lessThanRanges[ratingName] = new NumberRange(range.Start, lessThan - 1);


                var greaterThanOrEqualRanges = RatingRanges.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                greaterThanOrEqualRanges[ratingName] = new NumberRange(lessThan, range.End);

                return (new PartCollection(lessThanRanges, State), new PartCollection(greaterThanOrEqualRanges, State));
            }



            public (PartCollection? LessThanOrEqual, PartCollection? GreaterThan) SplitGreaterThan(string ratingName, int greaterThan)
            {
                var range = RatingRanges[ratingName];

                if (range.End < greaterThan)
                {
                    return (this, null);
                }

                if (range.Start > greaterThan)
                {
                    return (null, this);
                }

                var lessThanOrEqualRanges = RatingRanges.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                lessThanOrEqualRanges[ratingName] = new NumberRange(range.Start, greaterThan);


                var greaterThanRanges = RatingRanges.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                greaterThanRanges[ratingName] = new NumberRange(greaterThan + 1, range.End);

                return (new PartCollection(lessThanOrEqualRanges, State), new PartCollection(greaterThanRanges, State));
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

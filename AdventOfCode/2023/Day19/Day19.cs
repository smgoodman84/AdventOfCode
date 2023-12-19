using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;

namespace AdventOfCode._2023.Day19
{
    public class Day19 : Day
    {
        public Day19() : base(2023, 19, "Day19/input_2023_19.txt", "333263", "", true)
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
            return string.Empty;
        }

        private class Workflow
        {
            public static Dictionary<string, Workflow> Workflows = new Dictionary<string, Workflow>();
            public string Name { get; set; }

            private List<Func<Part, State>> _predicates = new List<Func<Part, State>>();
            private Workflow(string description)
            {
                var nameSplit = description.Split("{");
                Name = nameSplit[0];

                var predicates = nameSplit[1].Replace("}", "").Split(",");
                foreach (var predicate in predicates)
                {
                    _predicates.Add(ParsePredicate(predicate));
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

                return p => Workflows[destinationName].ProcessPart(p);
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

            public bool IsAccepted(Part part)
            {
                var result = ProcessPart(part);
                return result == State.Accept;
            }

            private enum State
            {
                Unknown,
                Continue,
                Accept,
                Reject
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
    }
}

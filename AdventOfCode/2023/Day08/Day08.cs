using AdventOfCode.Shared;
using AdventOfCode.Shared.Mathematics;

namespace AdventOfCode._2023.Day08
{
    public class Day08 : Day
    {
        public Day08() : base(2023, 8, "Day08/input_2023_08.txt", "18827", "20220305520997")
        {

        }

        private string _directions;
        private Dictionary<string, Node> _nodes;
        public override void Initialise()
        {
            _directions = InputLines[0];
            _nodes = InputLines
                .Skip(2)
                .Select(l => new Node(l))
                .ToDictionary(n => n.Name, n => n);
        }

        public override string Part1()
        {
            return GetPathLength("AAA", x => x == "ZZZ").Length.ToString();
        }

        public override string Part2()
        {
            var startNodes = _nodes.Keys.Where(n => n.EndsWith('A')).ToList();

            var pathLengths = startNodes
                .Select(n => (n, GetPathLength(n, x => x.EndsWith('Z'))))
                .ToList();

            long lowestCommonMultiple = 1L;
            foreach (var pathLength in pathLengths)
            {
                TraceLine($"{pathLength.n} -> {pathLength.Item2.EndNode} = {pathLength.Item2.Length}");
                lowestCommonMultiple = MathematicsHelper.LowestCommonMultiple(lowestCommonMultiple, pathLength.Item2.Length);
            }

            return lowestCommonMultiple.ToString();
        }


        private (int Length, string EndNode) GetPathLength(string startNode, Func<string, bool> endNodePredicate)
        {
            var currentNode = _nodes[startNode];
            var steps = 0;

            foreach (var direction in GetDirections())
            {
                if (direction == 'L')
                {
                    currentNode = _nodes[currentNode.Left];
                    steps += 1;
                }

                if (direction == 'R')
                {
                    currentNode = _nodes[currentNode.Right];
                    steps += 1;
                }

                if (endNodePredicate(currentNode.Name))
                {
                    return (steps, currentNode.Name);
                }
            }

            return (-1, "LOST");
        }

        private IEnumerable<char> GetDirections()
        {
            var index = 0;
            while (true)
            {
                yield return _directions[index];
                index += 1;
                if (index >= _directions.Length)
                {
                    index -= _directions.Length;
                }
            }
        }

        private class Node
        {
            public string Name { get; set; }
            public string Left { get; set; }
            public string Right { get; set; }
            public Node(string description)
            {
                var split = description.Split(" = ");
                Name = split[0];

                var leftRight = split[1]
                    .Replace("(", "")
                    .Replace(")", "")
                    .Split(",");

                Left = leftRight[0].Trim();
                Right = leftRight[1].Trim();
            }

            public override string ToString()
            {
                return $"{Name}: {Left},{Right}";
            }
        }
    }
}


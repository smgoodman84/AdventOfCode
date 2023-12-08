using AdventOfCode.Shared;

namespace AdventOfCode._2023.Day08
{
    public class Day08 : Day
    {
        public Day08() : base(2023, 8, "Day08/input_2023_08.txt", "18827", "")
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
            var currentNode = _nodes["AAA"];
            var steps = 0;

            foreach(var direction in GetDirections())
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

                if (currentNode.Name == "ZZZ")
                {
                    return steps.ToString();
                }
            }

            return "LOST";
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

        public override string Part2()
        {
            return string.Empty;
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


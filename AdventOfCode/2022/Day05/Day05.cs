using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;

namespace AdventOfCode._2022.Day05;

public class Day05 : Day
{
    public Day05() : base(2022, 5, "Day05/input_2022_05.txt", "WCZTHTMPS", "BLSGJSDTS")
    {

    }

    private List<Move> _moves;
    private Dictionary<int, Stack<char>> _stacks;
    public override void Initialise()
    {
        var groups = LineGrouper.GroupLinesBySeperator(InputLines)
            .ToArray();

        var stacks = groups[0];

        var stackIndexes = new Dictionary<int, int>();
        var lastLine = stacks.Last().ToCharArray();
        for (var llIndex = 0; llIndex < lastLine.Length; llIndex++)
        {
            if (lastLine[llIndex] != ' ')
            {
                stackIndexes.Add(int.Parse(lastLine[llIndex].ToString()), llIndex);
            }
        }

        _stacks = new Dictionary<int, Stack<char>>();
        foreach (var stackIndex in stackIndexes)
        {
            _stacks.Add(stackIndex.Key, new Stack<char>());
        }

        for (var stackLineIndex = stacks.Count() - 2; stackLineIndex >= 0; stackLineIndex--)
        {
            var line = stacks[stackLineIndex];
            foreach (var stackIndex in stackIndexes)
            {
                var value = line[stackIndex.Value];
                if (value != ' ')
                {
                    _stacks[stackIndex.Key].Push(value);
                }
            }
        }

        var moves = groups[1];
        _moves = moves
            .Select(m => new Move(m))
            .ToList();
    }

    public override string Part1()
    {
        foreach (var move in _moves)
        {
            var count = move.Count;
            while (count > 0)
            {
                var value = _stacks[move.Source].Pop();
                _stacks[move.Destination].Push(value);
                count -= 1;
            }
        }

        var result = new StringBuilder();
        foreach (var stack in _stacks)
        {
            result.Append(stack.Value.Peek());
        }

        return result.ToString();
    }

    public override string Part2()
    {
        Initialise();

        foreach (var move in _moves)
        {
            var count = move.Count;
            var tempStack = new Stack<char>();
            while (count > 0)
            {
                var value = _stacks[move.Source].Pop();
                tempStack.Push(value);
                count -= 1;
            }

            while (tempStack.Any())
            {
                var value = tempStack.Pop();
                _stacks[move.Destination].Push(value);
            }
        }

        var result = new StringBuilder();
        foreach (var stack in _stacks)
        {
            result.Append(stack.Value.Peek());
        }

        return result.ToString();
    }

    private class Move
    {
        public int Count { get; set; }
        public int Source { get; set; }
        public int Destination { get; set; }

        public Move(string move)
        {
            var split = move.Split(" ");
            Count = int.Parse(split[1]);
            Source = int.Parse(split[3]);
            Destination = int.Parse(split[5]);
        }
    }
}
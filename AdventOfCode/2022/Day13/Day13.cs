using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.PrimitiveExtensions;

namespace AdventOfCode._2022.Day13
{
    public class Day13 : Day
    {
        public Day13() : base(2022, 13, "Day13/input_2022_13.txt", "5905", "21691")
        {

        }

        private List<(IntTree Left, IntTree Right)> _pairs;
        public override void Initialise()
        {
            var pairs = LineGrouper.GroupLinesBySeperator(InputLines).ToList();

            _pairs = new List<(IntTree Left, IntTree Right)>();
            foreach (var pair in pairs)
            {
                _pairs.Add((new IntTree(pair[0]), new IntTree(pair[1])));
            }
        }

        public override string Part1()
        {
            var correctOrderInfo = _pairs
                .Select((x, i) => (i + 1, IsInCorrectOrder(x.Left, x.Right)))
                .ToList();

            var result = 0;
            foreach (var coi in correctOrderInfo)
            {
                if (coi.Item2)
                {
                    result += coi.Item1;
                }
            }

            return result.ToString();
        }

        public override string Part2()
        {
            var unorderedList = new List<IntTree>();
            unorderedList.AddRange(_pairs.Select(p => p.Left));
            unorderedList.AddRange(_pairs.Select(p => p.Right));

            var orderedList = new List<IntTree>()
            {
                new IntTree("[[2]]"),
                new IntTree("[[6]]"),
            };

            foreach (var item in unorderedList)
            {
                var inserted = false;
                var insertIndex = 0;
                while (!inserted)
                {
                    if (insertIndex == 0)
                    {
                        if (IsInCorrectOrder(item, orderedList[0]))
                        {
                            orderedList.Insert(0, item);
                            inserted = true;
                        }
                    }
                    else if (insertIndex == orderedList.Count)
                    {
                        orderedList.Add(item);
                        inserted = true;
                    }
                    else if (IsInCorrectOrder(orderedList[insertIndex - 1], item)
                        && IsInCorrectOrder(item, orderedList[insertIndex]))
                    {
                        orderedList.Insert(insertIndex, item);
                        inserted = true;
                    }

                    insertIndex += 1;
                }
            }

            var firstDivider = orderedList
                .Select((x, i) => (i + 1, x.RawInput == "[[2]]"))
                .Where(x => x.Item2)
                .Select(x => x.Item1)
                .Single();

            var secondDivider = orderedList
                .Select((x, i) => (i + 1, x.RawInput == "[[6]]"))
                .Where(x => x.Item2)
                .Select(x => x.Item1)
                .Single();

            var result = firstDivider * secondDivider;

            return result.ToString();
        }

        private bool IsInCorrectOrder(IntTree left, IntTree right)
        {
            return Compare(left, right) == ComparisonResult.Correct;
        }

        private ComparisonResult Compare(IntTree left, IntTree right)
        {
            if (left.IsInteger && right.IsInteger)
            {
                if (left.IntValue < right.IntValue)
                {
                    return ComparisonResult.Correct;
                }

                if (left.IntValue > right.IntValue)
                {
                    return ComparisonResult.Incorrect;
                }

                return ComparisonResult.Continue;
            }

            if (left.IsList && right.IsList)
            {
                var index = 0;
                while (true)
                {
                    var leftOutOfItems = index > left.ListValue.Count - 1;
                    var rightOutOfItems = index > right.ListValue.Count - 1;

                    if (leftOutOfItems)
                    {
                        if (rightOutOfItems)
                        {
                            return ComparisonResult.Continue;
                        }

                        return ComparisonResult.Correct;
                    }

                    if (rightOutOfItems)
                    {
                        return ComparisonResult.Incorrect;
                    }

                    var itemComparison = Compare(left.ListValue[index], right.ListValue[index]);

                    if (itemComparison == ComparisonResult.Correct || itemComparison == ComparisonResult.Incorrect)
                    {
                        return itemComparison;
                    }

                    index += 1;
                }
            }

            if (left.IsInteger && right.IsList)
            {
                return Compare(new IntTree($"[{left.IntValue}]"), right);
            }

            if (left.IsList && right.IsInteger)
            {
                return Compare(left, new IntTree($"[{right.IntValue}]"));
            }

            return ComparisonResult.Unknown;
        }

        private enum ComparisonResult
        {
            Continue,
            Correct,
            Incorrect,
            Unknown
        }

        private class IntTree
        {
            public string RawInput { get; }
            public bool IsInteger { get; private set; }
            public bool IsList => !IsInteger;

            public int IntValue { get; private set; }
            public List<IntTree> ListValue { get; private set; }

            public IntTree(string value)
            {
                RawInput = value;

                if (IsInteger = int.TryParse(value, out var intValue))
                {
                    IntValue = intValue;
                }
                else
                {
                    ListValue = Parse(value);
                }
            }

            private List<IntTree> Parse(string value)
            {
                var result = new List<IntTree>();

                var bracketDepth = 0;
                var start = 0;
                var current = 0;
                foreach (var c in value)
                {
                    if (c == '[')
                    {
                        bracketDepth += 1;
                        if (bracketDepth == 1)
                        {
                            start = current + 1;
                        }
                    }

                    if (c == ']')
                    {
                        bracketDepth -= 1;
                        if (bracketDepth == 0)
                        {
                            var element = value.Substring(start, current - start);
                            result.Add(new IntTree(element));
                            start = current + 1;
                        }
                    }

                    if (c == ',' && bracketDepth == 1)
                    {
                        var element = value.Substring(start, current - start);
                        result.Add(new IntTree(element));
                        start = current + 1;
                    }

                    current += 1;
                }

                return result;
            }

            public override string ToString()
            {
                if (IsInteger)
                {
                    return IntValue.ToString();
                }

                return $"[{string.Join(",", ListValue.Select(x => x.ToString()))}]";
            }
        }
    }
}


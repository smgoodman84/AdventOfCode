using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2021.Day10
{
    public class Day10 : Day
    {
        private List<NavigationParser> _lines;

        public Day10() : base(2021, 10, "Day10/input.txt", "268845", "4038824534")
        {
        }

        public override void Initialise()
        {
            _lines = InputLines
                .Select(l => new NavigationParser(l))
                .ToList();
        }

        public override string Part1()
        {
            var result = _lines.Sum(l => l.GetSyntaxErrorScore());

            return result.ToString();
        }

        public override string Part2()
        {
            var incompleteLines = _lines
                .Where(l => l.GetSyntaxErrorScore() == 0)
                .ToList();

            var completionScores = incompleteLines
                .Select(l => l.GetCompletionScore())
                .OrderBy(score => score)
                .ToArray();

            var middle = (completionScores.Length - 1) / 2;

            return completionScores[middle].ToString();
        }

        private class NavigationParser
        {
            private readonly string _input;

            public NavigationParser(string input)
            {
                _input = input;
            }

            public int GetSyntaxErrorScore()
            {
                var stack = new Stack<char>();

                foreach (var c in _input)
                {
                    if (new[] { '(', '[', '{', '<' }.Contains(c))
                    {
                        stack.Push(c);
                        continue;
                    }

                    var previous = stack.Pop();

                    if (c == ')' && previous != '(')
                    {
                        return 3;
                    }
                    if (c == ']' && previous != '[')
                    {
                        return 57;
                    }
                    if (c == '}' && previous != '{')
                    {
                        return 1197;
                    }
                    if (c == '>' && previous != '<')
                    {
                        return 25137;
                    }
                }

                return 0;
            }

            public long GetCompletionScore()
            {
                var stack = new Stack<char>();

                foreach (var c in _input)
                {
                    if (new[] { '(', '[', '{', '<' }.Contains(c))
                    {
                        stack.Push(c);
                        continue;
                    }

                    stack.Pop();
                }

                long score = 0;

                while (stack.TryPop(out var c))
                {
                    score *= 5;
                    switch (c)
                    {
                        case '(': score += 1; break;
                        case '[': score += 2; break;
                        case '{': score += 3; break;
                        case '<': score += 4; break;
                    }
                }

                return score;
            }
        }
    }
}

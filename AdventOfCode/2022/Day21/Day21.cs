using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2022.Day21
{
    public class Day21 : Day
    {
        public Day21() : base(2022, 21, "Day21/input_2022_21.txt", "", "")
        {

        }

        private Dictionary<string, Expression> _lookup;
        public override void Initialise()
        {
            _lookup = new Dictionary<string, Expression>();
            foreach (var line in InputLines)
            {
                var nameSplit = line.Split(": ");
                var name = nameSplit[0];
                var expression = nameSplit[1];
                _lookup.Add(name, new Expression(name, expression, _lookup));
            }
        }

        public override string Part1()
        {
            return _lookup["root"].Evaluate().ToString();
        }

        public override string Part2()
        {
            return "";
        }

        private class Expression
        {
            private readonly string _expression;
            private readonly Dictionary<string, Expression> _lookup;
            private bool _evaluated = false;
            private long _value = 0;

            public Expression(string name, string expression, Dictionary<string, Expression> lookup)
            {
                Name = name;
                _expression = expression;
                _lookup = lookup;
            }

            public string Name { get; }

            public long Evaluate()
            {
                if (!_evaluated)
                {
                    _value = UnacachedEvaluate();
                    Console.WriteLine($"{Name}: Evaluated {_expression} to be {_value}");
                    _evaluated = true;
                }

                return _value;
            }

            private long UnacachedEvaluate()
            {
                Console.WriteLine($"{Name}: Evaluating {_expression}");
                if (_expression.Contains("*"))
                {
                    var split = _expression.Split(" * ");
                    var leftName = split[0];
                    var rightName = split[1];
                    var left = _lookup[leftName];
                    var right = _lookup[rightName];
                    return left.Evaluate() * right.Evaluate();
                }

                if (_expression.Contains("/"))
                {
                    var split = _expression.Split(" / ");
                    var leftName = split[0];
                    var rightName = split[1];
                    var left = _lookup[leftName];
                    var right = _lookup[rightName];
                    return left.Evaluate() / right.Evaluate();
                }

                if (_expression.Contains("+"))
                {
                    var split = _expression.Split(" + ");
                    var leftName = split[0];
                    var rightName = split[1];
                    var left = _lookup[leftName];
                    var right = _lookup[rightName];
                    return left.Evaluate() + right.Evaluate();
                }

                if (_expression.Contains("-"))
                {
                    var split = _expression.Split(" - ");
                    var leftName = split[0];
                    var rightName = split[1];
                    var left = _lookup[leftName];
                    var right = _lookup[rightName];
                    return left.Evaluate() - right.Evaluate();
                }

                return long.Parse(_expression);
            }
        }
    }
}

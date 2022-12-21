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
        public Day21() : base(2022, 21, "Day21/input_2022_21.txt", "85616733059734", "")
        {

        }

        private Dictionary<string, IExpression> _lookup;
        public override void Initialise()
        {
            _lookup = new Dictionary<string, IExpression>();
            foreach (var line in InputLines)
            {
                var nameSplit = line.Split(": ");
                var name = nameSplit[0];
                var expression = nameSplit[1];
                _lookup.Add(name, CreateExpression(name, expression));
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

        private interface IExpression
        {

            public string Name { get; }

            public long Evaluate();
        }

        private IExpression CreateExpression(string name, string expression)
        {
            if (long.TryParse(expression, out var constant))
            {
                return new Constant(name, constant);
            }

            var split = expression.Split(" ");
            var left = new ExpressionWrapper(split[0], _lookup);
            var right = new ExpressionWrapper(split[2], _lookup);

            switch (split[1])
            {
                case "+": return new Add(name, left, right);
                case "-": return new Subtract(name, left, right);
                case "*": return new Multiply(name, left, right);
                case "/": return new Divide(name, left, right);
            }

            throw new Exception($"Failed to create expression {expression}");
        }

        private class ExpressionWrapper : IExpression
        {
            private readonly Dictionary<string, IExpression> _lookup;
            private bool _evaluated;
            private long _value;

            public ExpressionWrapper(string name, Dictionary<string, IExpression> lookup)
            {
                Name = name;
                _lookup = lookup;
            }

            public string Name { get; }

            public long Evaluate()
            {
                if (!_evaluated)
                {
                    _value = _lookup[Name].Evaluate();
                    _evaluated = true;
                }

                return _value;
            }
        }

        private class Constant : IExpression
        {
            private readonly long _value;

            public string Name { get; }

            public Constant(string name, long value)
            {
                Name = name;
                _value = value;
            }

            public long Evaluate()
            {
                return _value;
            }
        }

        private class Add : IExpression
        {
            private readonly IExpression _left;
            private readonly IExpression _right;

            public string Name { get; }

            public Add(string name, IExpression left, IExpression right)
            {
                Name = name;
                _left = left;
                _right = right;
            }

            public long Evaluate()
            {
                return _left.Evaluate() + _right.Evaluate();
            }
        }

        private class Subtract : IExpression
        {
            private readonly IExpression _left;
            private readonly IExpression _right;

            public string Name { get; }

            public Subtract(string name, IExpression left, IExpression right)
            {
                Name = name;
                _left = left;
                _right = right;
            }

            public long Evaluate()
            {
                return _left.Evaluate() - _right.Evaluate();
            }
        }

        private class Multiply : IExpression
        {
            private readonly IExpression _left;
            private readonly IExpression _right;

            public string Name { get; }

            public Multiply(string name, IExpression left, IExpression right)
            {
                Name = name;
                _left = left;
                _right = right;
            }

            public long Evaluate()
            {
                return _left.Evaluate() * _right.Evaluate();
            }
        }

        private class Divide : IExpression
        {
            private readonly IExpression _left;
            private readonly IExpression _right;

            public string Name { get; }

            public Divide(string name, IExpression left, IExpression right)
            {
                Name = name;
                _left = left;
                _right = right;
            }

            public long Evaluate()
            {
                return _left.Evaluate() / _right.Evaluate();
            }
        }
    }
}

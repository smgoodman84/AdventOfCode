using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2022.Day21;

public class Day21 : Day
{
    public Day21() : base(2022, 21, "Day21/input_2022_21.txt", "85616733059734", "3560324848168")
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
        var root = _lookup["root"] as Root;
        var humanNeedsToSay = root.FindHumanValueToProduce(0);
        return humanNeedsToSay.ToString();
    }

    private interface IExpression
    {
        public string Name { get; }

        public long Evaluate();

        public bool ReliesOnHuman();

        public long FindHumanValueToProduce(long needToProduce);
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

        if (name == "root")
        {
            return new Root("root", left, right);
        }

        switch (split[1])
        {
            case "+": return new Add(name, left, right);
            case "-": return new Subtract(name, left, right);
            case "*": return new Multiply(name, left, right);
            case "/": return new Divide(name, left, right);
        }

        throw new Exception($"Failed to create expression {expression}");
    }

    private class Root : IExpression
    {
        private readonly IExpression _left;
        private readonly IExpression _right;

        public string Name { get; }

        public Root(string name, IExpression left, IExpression right)
        {
            Name = name;
            _left = left;
            _right = right;
        }

        public long Evaluate()
        {
            return _left.Evaluate() + _right.Evaluate();
        }

        public bool ReliesOnHuman() => _left.ReliesOnHuman() || _right.ReliesOnHuman();

        public long FindHumanValueToProduce(long needToProduce)
        {
            if (_left.ReliesOnHuman() && _right.ReliesOnHuman())
            {
                throw new Exception("Hmmmmm");
            }

            if (_left.ReliesOnHuman())
            {
                var leftNeedsToEqual = _right.Evaluate();
                return _left.FindHumanValueToProduce(leftNeedsToEqual);
            }

            var rightNeedsToEqual = _left.Evaluate();
            return _right.FindHumanValueToProduce(rightNeedsToEqual);
        }
    }

    private class ExpressionWrapper : IExpression
    {
        private readonly Dictionary<string, IExpression> _lookup;
        private bool _evaluated;
        private long _value;
        private bool _evaluatedReliesOnHuman;
        private bool _reliesOnHumanValue;
        private bool _evaluatedNeedToProduce;
        private long _needToProduceValue;

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

        public bool ReliesOnHuman()
        {
            if (!_evaluatedReliesOnHuman)
            {
                _reliesOnHumanValue = _lookup[Name].ReliesOnHuman();
                _evaluatedReliesOnHuman = true;
            }

            return _reliesOnHumanValue;
        }

        public long FindHumanValueToProduce(long needToProduce)
        {
            if (!_evaluatedNeedToProduce)
            {
                _needToProduceValue = _lookup[Name].FindHumanValueToProduce(needToProduce);
                _evaluatedNeedToProduce = true;
            }

            return _needToProduceValue;
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

        public bool ReliesOnHuman() => Name == "humn";

        public long FindHumanValueToProduce(long needToProduce)
        {
            if (Name != "humn")
            {
                throw new Exception("Hmmmmm, should be humn");
            }

            return needToProduce;
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

        public bool ReliesOnHuman() => _left.ReliesOnHuman() || _right.ReliesOnHuman();

        public long FindHumanValueToProduce(long needToProduce)
        {
            if (_left.ReliesOnHuman() && _right.ReliesOnHuman())
            {
                throw new Exception("Hmmmmm");
            }

            if (_left.ReliesOnHuman())
            {
                var leftNeedsToEqual = needToProduce - _right.Evaluate();
                return _left.FindHumanValueToProduce(leftNeedsToEqual);
            }

            var rightNeedsToEqual = needToProduce - _left.Evaluate();
            return _right.FindHumanValueToProduce(rightNeedsToEqual);
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

        public bool ReliesOnHuman() => _left.ReliesOnHuman() || _right.ReliesOnHuman();

        public long FindHumanValueToProduce(long needToProduce)
        {
            if (_left.ReliesOnHuman() && _right.ReliesOnHuman())
            {
                throw new Exception("Hmmmmm");
            }

            if (_left.ReliesOnHuman())
            {
                var leftNeedsToEqual = needToProduce + _right.Evaluate();
                return _left.FindHumanValueToProduce(leftNeedsToEqual);
            }

            var rightNeedsToEqual = _left.Evaluate() - needToProduce;
            return _right.FindHumanValueToProduce(rightNeedsToEqual);
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

        public bool ReliesOnHuman() => _left.ReliesOnHuman() || _right.ReliesOnHuman();

        public long FindHumanValueToProduce(long needToProduce)
        {
            if (_left.ReliesOnHuman() && _right.ReliesOnHuman())
            {
                throw new Exception("Hmmmmm");
            }

            if (_left.ReliesOnHuman())
            {
                var leftNeedsToEqual = needToProduce / _right.Evaluate();
                return _left.FindHumanValueToProduce(leftNeedsToEqual);
            }

            var rightNeedsToEqual = needToProduce / _left.Evaluate();
            return _right.FindHumanValueToProduce(rightNeedsToEqual);
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

        public bool ReliesOnHuman() => _left.ReliesOnHuman() || _right.ReliesOnHuman();

        public long FindHumanValueToProduce(long needToProduce)
        {
            if (_left.ReliesOnHuman() && _right.ReliesOnHuman())
            {
                throw new Exception("Hmmmmm");
            }

            if (_left.ReliesOnHuman())
            {
                var leftNeedsToEqual = needToProduce * _right.Evaluate();
                return _left.FindHumanValueToProduce(leftNeedsToEqual);
            }

            var rightNeedsToEqual = _left.Evaluate() / needToProduce;
            return _right.FindHumanValueToProduce(rightNeedsToEqual);
        }
    }
}
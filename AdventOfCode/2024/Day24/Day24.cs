using AdventOfCode.Shared;

namespace AdventOfCode._2024.Day24;

public class Day24 : Day
{
    public Day24() : base(2024, 24, "Day24/input_2024_24.txt", "51745744348272", "", true)
    {
    }


    private Dictionary<string, IGate> _gates = new();
    public override void Initialise()
    {
        foreach (var line in InputLines)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                var gate = CreateGate(line);
                _gates.Add(gate.Name, gate);
            }
        }
    }

    private IGate CreateGate(string line)
    {
        if (line.Contains(':'))
        {
            var split = line.Split(": ");
            var constName = split[0];
            var value = split[1] == "1";
            return new ConstGate(constName, value);
        }

        var elements = line.Replace("-> ", "").Split(" ");
        var left = elements[0];
        var operand = elements[1];
        var right = elements[2];
        var name = elements[3];

        switch (operand)
        {
            case "AND": return new AndGate(name, left, right, _gates);
            case "OR": return new OrGate(name, left, right, _gates);
            case "XOR": return new XorGate(name, left, right, _gates);
        }

        throw new Exception($"Could not create gate for {line}");
    }

    public override string Part1()
    {
        var zKeys = _gates.Keys
            .Where(k => k.StartsWith('z'))
            .OrderBy(k => k)
            .ToList();

        var zValues = zKeys
            .Select(k => _gates[k].ReadValue())
            .ToList();

        long value = 0;
        long multiplier = 1;
        foreach (var zValue in zValues)
        {
            value += zValue ? multiplier : 0;
            multiplier *= 2;
        }

        return value.ToString();
    }

    public override string Part2()
    {
        return string.Empty;
    }

    private interface IGate
    {
        string Name { get; }
        bool ReadValue();
    }

    private class ConstGate : IGate
    {
        public string Name { get; }
        public bool Value { get; }

        public ConstGate(string name, bool value)
        {
            Name = name;
            Value = value;
        }

        public bool ReadValue() => Value;
    }

    private abstract class OperatorGate : IGate
    {
        public string Name { get; }
        private string _left { get; }
        private string _right { get; }
        private Dictionary<string, IGate> _gates { get; }
        
        public OperatorGate(string name, string left, string right, Dictionary<string, IGate> gates)
        {
            Name = name;
            _left = left;
            _right = right;
            _gates = gates;
        }

        protected bool ReadLeft()
        {
            return _gates[_left].ReadValue();
        }

        protected bool ReadRight()
        {
            return _gates[_right].ReadValue();
        }

        public abstract bool ReadValue();
    }

    private class AndGate : OperatorGate
    {
        public AndGate(string name, string left, string right, Dictionary<string, IGate> gates)
            : base(name, left, right, gates)
        {
        }

        public override bool ReadValue() => ReadLeft() && ReadRight(); 
    }

    private class OrGate : OperatorGate
    {
        public OrGate(string name, string left, string right, Dictionary<string, IGate> gates)
            : base(name, left, right, gates)
        {
        }

        public override bool ReadValue() => ReadLeft() || ReadRight(); 
    }

    private class XorGate : OperatorGate
    {
        public XorGate(string name, string left, string right, Dictionary<string, IGate> gates)
            : base(name, left, right, gates)
        {
        }

        public override bool ReadValue()
        {
            var left = ReadLeft();
            var right = ReadRight();

            return (left || right) && !(left && right);
        }
    }
}
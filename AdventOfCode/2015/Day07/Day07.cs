using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2015.Day07
{
    public class Day07 : Day
    {
        public Day07() : base(2015, 7, "Day07/input_2015_07.txt", "16076", "2797")
        {
        }

        private Dictionary<string, ISignal> _signals = new Dictionary<string, ISignal>();

        public override void Initialise()
        {
            foreach (var inputLine in InputLines)
            {
                var split = inputLine.Split(" -> ");
                var output = split[1];
                var input = CreateGate(split[0], output);
                _signals[output] = new SignalCache(input);
            }
        }

        public override string Part1()
        {
            return _signals["a"].Output().ToString();
        }

        public override string Part2()
        {
            var inputB = new Constant(_signals["a"].Output());

            _signals.Clear();
            foreach (var inputLine in InputLines)
            {
                var split = inputLine.Split(" -> ");
                var output = split[1];
                var input = CreateGate(split[0], output);
                _signals[output] = new SignalCache(input);
            }

            _signals["b"] = inputB;

            var result = _signals["a"].Output().ToString();
            return result;
        }

        private ISignal CreateGate(string gate, string output)
        {
            if (gate.Contains(" AND "))
            {
                var split = gate.Split(" AND ");
                var input1 = GetInput(split[0]);
                var input2 = GetInput(split[1]);

                return new AndGate(input1, input2, output);
            }

            if (gate.Contains(" OR "))
            {
                var split = gate.Split(" OR ");
                var input1 = GetInput(split[0]);
                var input2 = GetInput(split[1]);

                return new OrGate(input1, input2, output);
            }

            if (gate.Contains(" LSHIFT "))
            {
                var split = gate.Split(" LSHIFT ");
                var input1 = GetInput(split[0]);
                var input2 = GetInput(split[1]);

                return new LeftShiftGate(input1, input2, output);
            }

            if (gate.Contains(" RSHIFT "))
            {
                var split = gate.Split(" RSHIFT ");
                var input1 = GetInput(split[0]);
                var input2 = GetInput(split[1]);

                return new RightShiftGate(input1, input2, output);
            }

            if (gate.StartsWith("NOT "))
            {
                var input = GetInput(gate.Substring(4));

                return new NotGate(input, output);
            }

            return new NoopGate(GetInput(gate), output);
        }

        private ISignal GetInput(string input)
        {
            return new SignalCache(GetUncachedInput(input));
        }

        private ISignal GetUncachedInput(string input)
        {
            if (ushort.TryParse(input, out var constant))
            {
                return new Constant(constant);
            }

            return new DeferredSignal(_signals, input);
        }

        private interface ISignal
        {
            string OutputName { get; }
            ushort Output();
        }

        private class DeferredSignal : ISignal
        {
            private readonly Dictionary<string, ISignal> _signalLookup;
            private readonly string _input;

            public DeferredSignal(Dictionary<string, ISignal> signalLookup, string input)
            {
                _signalLookup = signalLookup;
                _input = input;
            }

            public string OutputName => FindSignal().OutputName;

            public ushort Output() => FindSignal().Output();

            private ISignal FindSignal()
            {
                if (_signalLookup.TryGetValue(_input, out var signal))
                {
                    return signal;
                }

                return new NoInputGate(_input);
            }

            public override string ToString()
            {
                return $"Deferred {_input}";
            }
        }

        private class SignalCache : ISignal
        {
            private readonly ISignal _signal;

            public SignalCache(ISignal signal)
            {
                _signal = signal;
            }

            public string OutputName => _signal.OutputName;

            private bool _gotOutput = false;
            private ushort _output = 0;
            public ushort Output()
            {
                if (!_gotOutput)
                {
                    _output = _signal.Output();
                    _gotOutput = true;
                }

                return _output;
            }

            public override string ToString()
            {
                if (_gotOutput)
                {
                    return $"Cached {_signal.ToString()} = {_output}";
                }

                return $"Cached {_signal.ToString()}";
            }
        }

        private class Constant : ISignal
        {
            private readonly ushort _value;
            public string OutputName => _value.ToString();

            public Constant(ushort value)
            {
                _value = value;
            }

            public ushort Output() => _value;

            public override string ToString()
            {
                return $"{_value}";
            }
        }

        private class AndGate : ISignal
        {
            public string OutputName { get; }
            private ISignal Input1 { get; }
            private ISignal Input2 { get; }

            public AndGate(ISignal input1, ISignal input2, string outputName)
            {
                OutputName = outputName;
                Input1 = input1;
                Input2 = input2;
            }

            public ushort Output()
            {
                // Console.WriteLine($"{OutputName} = {ToString()}");
                var result = (ushort)(Input1.Output() & Input2.Output());
                // Console.WriteLine($"{OutputName} := {result}");
                return result;
            }

            public override string ToString()
            {
                return $"{Input1.OutputName} AND {Input2.OutputName}";
            }
        }

        private class OrGate : ISignal
        {
            public string OutputName { get; }
            private ISignal Input1 { get; }
            private ISignal Input2 { get; }

            public OrGate(ISignal input1, ISignal input2, string outputName)
            {
                OutputName = outputName;
                Input1 = input1;
                Input2 = input2;
            }

            public ushort Output()
            {
                // Console.WriteLine($"{OutputName} = {ToString()}");
                var result = (ushort)(Input1.Output() | Input2.Output());
                // Console.WriteLine($"{OutputName} := {result}");
                return result;
            }

            public override string ToString()
            {
                return $"{Input1.OutputName} OR {Input2.OutputName}";
            }
        }

        private class LeftShiftGate : ISignal
        {
            public string OutputName { get; }
            private ISignal Input1 { get; }
            private ISignal Input2 { get; }

            public LeftShiftGate(ISignal input1, ISignal input2, string outputName)
            {
                OutputName = outputName;
                Input1 = input1;
                Input2 = input2;
            }

            public ushort Output()
            {
                // Console.WriteLine($"{OutputName} = {ToString()}");
                var result = (ushort)(Input1.Output() << Input2.Output());
                // Console.WriteLine($"{OutputName} := {result}");
                return result;
            }

            public override string ToString()
            {
                return $"{Input1.OutputName} LSHIFT {Input2.OutputName}";
            }
        }

        private class RightShiftGate : ISignal
        {
            public string OutputName { get; }
            private ISignal Input1 { get; }
            private ISignal Input2 { get; }

            public RightShiftGate(ISignal input1, ISignal input2, string outputName)
            {
                OutputName = outputName;
                Input1 = input1;
                Input2 = input2;
            }

            public ushort Output()
            {
                // Console.WriteLine($"{OutputName} = {ToString()}");
                var result = (ushort)(Input1.Output() >> Input2.Output());
                // Console.WriteLine($"{OutputName} := {result}");
                return result;
            }

            public override string ToString()
            {
                return $"{Input1.OutputName} RSHIFT {Input2.OutputName}";
            }
        }

        private class NotGate : ISignal
        {
            public string OutputName { get; }
            private ISignal Input { get; }

            public NotGate(ISignal input, string outputName)
            {
                OutputName = outputName;
                Input = input;
            }

            public ushort Output()
            {
                // Console.WriteLine($"{OutputName} = {ToString()}");
                var result = (ushort)(~Input.Output());
                // Console.WriteLine($"{OutputName} := {result}");
                return result;
            }

            public override string ToString()
            {
                return $"NOT {Input.OutputName}";
            }
        }

        private class NoopGate : ISignal
        {
            public string OutputName { get; }
            private ISignal Input { get; }

            public NoopGate(ISignal input, string outputName)
            {
                OutputName = outputName;
                Input = input;
            }

            public ushort Output()
            {
                // Console.WriteLine($"{OutputName} = {ToString()}");
                var result = Input.Output();
                // Console.WriteLine($"{OutputName} := {result}");
                return result;
            }

            public override string ToString()
            {
                return $"NOOP {Input.OutputName}";
            }
        }

        private class NoInputGate : ISignal
        {
            public string OutputName { get; }

            public NoInputGate(string outputName)
            {
                OutputName = outputName;
            }

            public ushort Output()
            {
                // Console.WriteLine($"{OutputName} = {ToString()}");
                throw new Exception($"No input for {OutputName}");
            }

            public override string ToString()
            {
                return "NoInput";
            }
        }
    }
}
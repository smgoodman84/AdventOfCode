using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2021.Day24
{
    public class Day24 : Day
    {
        public Day24() : base(2021, 24, "Day24/input_2021_24.txt", "", "")
        {

        }

        public override string Part1()
        {
            return "Runs too slowly";
            var alu = new ArithmeticLogicUnit(InputLines);

            var inputs = GetPart1Inputs();
            foreach(var input in inputs)
            {
                var i = input.ToArray();
                var stringInput = string.Join("", i);

                var (result, ip) = alu.ExecuteAndReturnZ(i);

                var valid = result == 0;
                Trace($"{stringInput} ({ip}): {result}");
                if (valid)
                {
                    return stringInput;
                }
            }

            return "";
        }

        public override string Part2()
        {
            return "";
        }

        private IEnumerable<IEnumerable<int>> GetPart1Inputs(int count = 14)
        {
            if (count <= 0)
            {
                yield return Enumerable.Empty<int>();
            }

            foreach (var head in GetPart1Inputs(count - 1))
            {
                for (var i = 9; i > 0; i--)
                {
                    yield return head.Concat(new int[] { i });
                }
            }
        }

        private class ArithmeticLogicUnit
        {
            private List<Instruction> _instructions;
            public ArithmeticLogicUnit(List<string> program)
            {
                _instructions = program.Select(i => new Instruction(i)).ToList();
            }

            private Dictionary<string, int> _zCache = new Dictionary<string, int>();

            public (int, int) ExecuteAndReturnZ(int[] input)
            {
                var keys = new List<string>();
                var context = new Context(input);
                var instructionPointer = 0;

                foreach(var instruction in _instructions)
                {
                    var cacheKey = context.CacheKey(instructionPointer);
                    if (_zCache.ContainsKey(cacheKey))
                    {
                        return (_zCache[cacheKey], instructionPointer);
                    }
                    keys.Add(cacheKey);
                    instructionPointer += 1;

                    switch (instruction.Operation)
                    {
                        case "inp":
                            context.Write(instruction.OperandOne, context.GetInput());
                            break;
                        case "add":
                            context.Write(instruction.OperandOne,
                                context.Read(instruction.OperandOne) + context.Read(instruction.OperandTwo));
                            break;
                        case "mul":
                            context.Write(instruction.OperandOne,
                                context.Read(instruction.OperandOne) * context.Read(instruction.OperandTwo));
                            break;
                        case "div":
                            context.Write(instruction.OperandOne,
                                context.Read(instruction.OperandOne) / context.Read(instruction.OperandTwo));
                            break;
                        case "mod":
                            context.Write(instruction.OperandOne,
                                context.Read(instruction.OperandOne) % context.Read(instruction.OperandTwo));
                            break;
                        case "eql":
                            context.Write(instruction.OperandOne,
                                context.Read(instruction.OperandOne) == context.Read(instruction.OperandTwo) ? 1 : 0);
                            break;
                    }
                }

                var result = context.Read("z");
                foreach (var key in keys)
                {
                    _zCache[key] = result;
                }

                return (result, instructionPointer);
            }

            private class Context
            {
                private int[] _input;
                public int _inputPointer = 0;
                public Dictionary<string, int> _variables { get; set; } = new Dictionary<string, int>();

                public Context(int[] input)
                {
                    _input = input;
                }

                public string CacheKey(int instructionPointer)
                {
                    var inputs = string.Join("", _input.Skip(_inputPointer));
                    var variables = string.Join(',', _variables.Select(kvp => $"{kvp.Key}={kvp.Value}"));
                    return $"{instructionPointer}|{inputs}|{variables}";
                }

                public void Write(string variable, int value)
                {
                    _variables[variable] = value;
                }

                public int Read(string variable)
                {
                    if (int.TryParse(variable, out var intValue))
                    {
                        return intValue;
                    }

                    if (!_variables.ContainsKey(variable))
                    {
                        return 0;
                    }

                    return _variables[variable];
                }

                public int GetInput()
                {
                    var result = _input[_inputPointer];
                    _inputPointer += 1;
                    return result;
                }
            }
        }

        private class Instruction
        {
            public string Operation { get; set; }
            public string OperandOne { get; set; }
            public string OperandTwo { get; set; }
            public Instruction(string instruction)
            {
                var split = instruction.Split(' ');
                Operation = split[0];
                OperandOne = split[1];

                if (split.Length > 2)
                {
                    OperandTwo = split[2];
                }
            }
        }
    }
}

﻿using AdventOfCode.Shared;
using AdventOfCode.Shared.PrimitiveExtensions;

namespace AdventOfCode._2024.Day17;

public class Day17 : Day
{
    public Day17() : base(2024, 17, "Day17/input_2024_17.txt", "5,1,4,0,5,1,0,2,6", "", true)
    {
    }

    private Computer _computer;
    private string _programRaw;
    public override void Initialise()
    {
        var registerA = int.Parse(InputLines[0].Replace("Register A: ", ""));
        var registerB = int.Parse(InputLines[1].Replace("Register B: ", ""));
        var registerC = int.Parse(InputLines[2].Replace("Register C: ", ""));
        _programRaw = InputLines[4].Replace("Program: ", "");

        var operations = _programRaw
            .Split(",")
            .GroupsOfSize(2)
            .Select(op => 
                new Operation
                {
                    Instruction = (Instruction)int.Parse(op[0]),
                    Operand = int.Parse(op[1])
                })
            .ToArray();

        _computer = new Computer(registerA, registerB, registerC, operations);
    }

    public override string Part1()
    {
        return string.Empty;
        var output = _computer.Clone().Execute();
        return output;
    }

    public override string Part2()
    {
        var registerA = (2L << (15 * 3));
        var registerAMax = (2L << (16 * 3));
        TraceLine($"registerAMax {registerAMax}");
        // while (registerA < registerAMax)
        {
            if (registerA % 1000 == 0)
            {
                TraceLine($"Trying {registerA}");
            }
            var output = _computer.Clone(registerA).Execute();
            TraceLine($"{registerA}: {output}");
            if (output.Equals(_programRaw))
            {
                return registerA.ToString();
            }

            registerA += 1;
        }

        return string.Empty;
    }

    private class Computer
    {
        private long _programCounter { get; set; }
        private long _registerA { get; set; }
        private long _registerB { get; set; }
        private long _registerC { get; set; }
        private Operation[] _program { get; set; }

        public Computer(
            long registerA,
            long registerB,
            long registerC,
            Operation[] program
            )
        {
            _programCounter = 0;
            _registerA = registerA;
            _registerB = registerB;
            _registerC = registerC;
            _program = program;
        }

        public Computer Clone()
        {
            return new Computer(_registerA, _registerB, _registerC, _program)
            {
                _programCounter = _programCounter
            };
        }

        public Computer Clone(long registerA)
        {
            return new Computer(registerA, _registerB, _registerC, _program)
            {
                _programCounter = _programCounter
            };
        }

        public string Execute()
        {
            Console.WriteLine();
            Console.WriteLine("Starting...");
            var output = new List<long>();
            while (_programCounter < _program.Length)
            {
                Console.WriteLine($"ProgramCounter: {_programCounter} A:{_registerA} B:{_registerB} C:{_registerC}");
                var instructionOutput = ExecuteInstruction(_program[_programCounter]);
                if (instructionOutput.HasValue)
                {
                    output.Add(instructionOutput.Value);
                }
            }

            var result = string.Join(",", output);
            return result;
        }

        private long? ExecuteInstruction(Operation operation)
        {
            long? output = null;

            switch (operation.Instruction)
            {
                case Instruction.Adv: Adv(ReadComboOperand(operation.Operand)); break;
                case Instruction.Bxl: Bxl(operation.Operand); break;
                case Instruction.Bst: Bst(ReadComboOperand(operation.Operand)); break;
                case Instruction.Jnz: Jnz(operation.Operand); break;
                case Instruction.Bxc: Bxc(); break;
                case Instruction.Out: Out(ReadComboOperand(operation.Operand), out output); break;
                case Instruction.Bdv: Bdv(ReadComboOperand(operation.Operand)); break;
                case Instruction.Cdv: Cdv(ReadComboOperand(operation.Operand)); break;
                default: throw new Exception($"Unexpected Instruction {operation.Instruction}");
            }

            return output;
        }

        private void Adv(long operand)
        {
            Console.WriteLine($"Adv {operand}");
            var numerator = _registerA;
            var denominator = (long)Math.Pow(2, operand);
            var result = numerator / denominator;

            _registerA = result;
            _programCounter += 1;
        }

        private void Bxl(long operand)
        {
            Console.WriteLine($"Bxl Lit {operand}");
            var result = _registerB ^ operand;

            _registerB = result;
            _programCounter += 1;
        }

        private void Bst(long operand)
        {
            Console.WriteLine($"Bst {operand}");
            var result = operand % 8;

            _registerB = result;
            _programCounter += 1;
        }

        private void Jnz(long operand)
        {
            Console.WriteLine($"Jnz Lit {operand}");
            if (_registerA != 0)
            {
                _programCounter = operand;
            }
            else
            {
                _programCounter += 1;
            }
        }

        public void Bxc()
        {
            Console.WriteLine("Bxc");
            var result = _registerB ^ _registerC;
            
            _registerB = result;
            _programCounter += 1;
        }

        private void Out(long operand, out long? output)
        {
            Console.WriteLine($"Out {operand}");
            var result = operand % 8;

            output = result;
            _programCounter += 1;
        }

        private void Bdv(long operand)
        {
            Console.WriteLine($"Bdv {operand}");
            var numerator = _registerA;
            var denominator = (long)Math.Pow(2, operand);
            var result = numerator / denominator;

            _registerB = result;
            _programCounter += 1;
        }

        private void Cdv(long operand)
        {
            Console.WriteLine($"Cdv {operand}");
            var numerator = _registerA;
            var denominator = (int)Math.Pow(2, operand);
            var result = numerator / denominator;

            _registerC = result;
            _programCounter += 1;
        }

        private long ReadComboOperand(long operand)
        {
            switch (operand)
            {
                case 4: Console.WriteLine($"Reg A {_registerA}");return _registerA;
                case 5: Console.WriteLine($"Reg B {_registerB}");return _registerB;
                case 6: Console.WriteLine($"Reg C {_registerC}");return _registerC;
                case 7: throw new Exception($"Invalid Operand {operand}");
            }

            Console.WriteLine($"Lit {operand}");
            return operand;
        }
    }

    private class Operation
    {
        public Instruction Instruction { get; set; }
        public long Operand { get; set; }
    }

    private enum Instruction
    {
        Adv = 0,
        Bxl = 1,
        Bst = 2,
        Jnz = 3,
        Bxc = 4,
        Out = 5,
        Bdv = 6,
        Cdv = 7
    }
}
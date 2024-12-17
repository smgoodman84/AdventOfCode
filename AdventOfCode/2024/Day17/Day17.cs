using AdventOfCode.Shared;
using AdventOfCode.Shared.PrimitiveExtensions;

namespace AdventOfCode._2024.Day17;

public class Day17 : Day
{
    public Day17() : base(2024, 17, "Day17/input_2024_17.txt", "5,1,4,0,5,1,0,2,6", "", true)
    {
    }

    private Computer _computer;
    public override void Initialise()
    {
        var registerA = int.Parse(InputLines[0].Replace("Register A: ", ""));
        var registerB = int.Parse(InputLines[1].Replace("Register B: ", ""));
        var registerC = int.Parse(InputLines[2].Replace("Register C: ", ""));
        var programString = InputLines[4].Replace("Program: ", "");

        var operations = programString
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
        var output = _computer.Execute();
        var result = string.Join(",", output);
        return result;
    }

    public override string Part2()
    {
        return string.Empty;
    }

    private class Computer
    {
        private int _programCounter { get; set; }
        private int _registerA { get; set; }
        private int _registerB { get; set; }
        private int _registerC { get; set; }
        private Operation[] _program { get; set; }

        public Computer(
            int registerA,
            int registerB,
            int registerC,
            Operation[] program
            )
        {
            _programCounter = 0;
            _registerA = registerA;
            _registerB = registerB;
            _registerC = registerC;
            _program = program;
        }

        public List<int> Execute()
        {
            var output = new List<int>();
            while (_programCounter < _program.Length)
            {
                Console.WriteLine($"ProgramCounter: {_programCounter}");
                var instructionOutput = ExecuteInstruction(_program[_programCounter]);
                if (instructionOutput.HasValue)
                {
                    output.Add(instructionOutput.Value);
                }
            }
            return output;
        }

        private int? ExecuteInstruction(Operation operation)
        {
            int? output = null;

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

        private void Adv(int operand)
        {
            Console.WriteLine($"Adv {operand}");
            var numerator = _registerA;
            var denominator = (int)Math.Pow(2, operand);
            var result = numerator / denominator;

            _registerA = result;
            _programCounter += 1;
        }

        private void Bxl(int operand)
        {
            Console.WriteLine($"Bxl {operand}");
            var result = _registerB ^ operand;

            _registerB = result;
            _programCounter += 1;
        }

        private void Bst(int operand)
        {
            Console.WriteLine($"Bst {operand}");
            var result = operand % 8;

            _registerB = result;
            _programCounter += 1;
        }

        private void Jnz(int operand)
        {
            Console.WriteLine($"Jnz {operand}");
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

        private void Out(int operand, out int? output)
        {
            Console.WriteLine($"Out {operand}");
            var result = operand % 8;

            output = result;
            _programCounter += 1;
        }

        private void Bdv(int operand)
        {
            Console.WriteLine($"Bdv {operand}");
            var numerator = _registerA;
            var denominator = (int)Math.Pow(2, operand);
            var result = numerator / denominator;

            _registerB = result;
            _programCounter += 1;
        }

        private void Cdv(int operand)
        {
            Console.WriteLine($"Cdv {operand}");
            var numerator = _registerA;
            var denominator = (int)Math.Pow(2, operand);
            var result = numerator / denominator;

            _registerC = result;
            _programCounter += 1;
        }

        private int ReadComboOperand(int operand)
        {
            switch (operand)
            {
                case 4: return _registerA;
                case 5: return _registerB;
                case 6: return _registerC;
                case 7: throw new Exception($"Invalid Operand {operand}");
            }

            return operand;
        }
    }

    private class Operation
    {
        public Instruction Instruction { get; set; }
        public int Operand { get; set; }
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
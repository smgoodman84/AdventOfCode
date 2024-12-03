using AdventOfCode.Shared;

namespace AdventOfCode._2024.Day03;

public class Day03 : Day
{
    public Day03() : base(2024, 3, "Day03/input_2024_03.txt", "183380722", "82733683")
    {

    }

    private string _memory;
    public override void Initialise()
    {
        _memory = string.Join("", InputLines);
    }

    public override string Part1()
    {
        var result = GetResultOne(_memory);

        return result.ToString();
    }

    public override string Part2()
    {
        var result = GetResultTwo(_memory);

        return result.ToString();
    }

    private int GetResultOne(string memory)
    {
        var instructions = GetInstructions(memory);

        return instructions
            .Where(i => i.InstructionType == InstructionType.Multiply)
            .Sum(i => i.Value);
    }

    private int GetResultTwo(string memory)
    {
        var instructions = GetInstructions(memory);

        var active = true;
        var result = 0;
        foreach(var instruction in instructions)
        {
            switch(instruction.InstructionType)
            {
                case InstructionType.Multiply:
                    if (active)
                    {
                        result += instruction.Value;
                    }
                    break;
                case InstructionType.Do:
                    active = true;
                    break;
                case InstructionType.Dont:
                    active = false;
                    break;
            }
        }

        return result;
    }

    private List<Instruction> GetInstructions(string memory)
    {
        var instructions = new List<Instruction>();

        var searchFrom = 0;
        while (true)
        {
            var mulIndex = memory.IndexOf("mul(", searchFrom);
            if (mulIndex < 0)
            {
                break;
            }

            if (TryReadMulInstruction(memory, mulIndex + "mul(".Length, out var mulInstruction))
            {
                instructions.Add(mulInstruction);
            }

            searchFrom = mulIndex + 1;
        }

        searchFrom = 0;
        while (true)
        {
            var doIndex = memory.IndexOf("do()", searchFrom);
            if (doIndex < 0)
            {
                break;
            }
            
            instructions.Add(new Instruction(InstructionType.Do, doIndex));

            searchFrom = doIndex + 1;
        }

        searchFrom = 0;
        while (true)
        {
            var dontIndex = memory.IndexOf("don't()", searchFrom);
            if (dontIndex < 0)
            {
                break;
            }
            
            instructions.Add(new Instruction(InstructionType.Dont, dontIndex));

            searchFrom = dontIndex + 1;
        }

        return instructions
            .OrderBy(x => x.Position)
            .ToList();
    }

    private bool TryReadMulInstruction(
        string memory,
        int startIndex,
        out Instruction mulInstruction)
    {
        mulInstruction = default;

        var aStart = startIndex;
        var aEnd = startIndex;
        while (IsNumeric(memory[aEnd]))
        {
            aEnd += 1;
        }

        if (aEnd == aStart)
        {
            return false;
        }

        if (memory[aEnd] != ',')
        {
            return false;
        }

        var bStart = aEnd + 1;
        var bEnd = bStart;
        while (IsNumeric(memory[bEnd]))
        {
            bEnd += 1;
        }

        if (bEnd == bStart)
        {
            return false;
        }

        if (memory[bEnd] != ')')
        {
            return false;
        }

        var aString = memory.Substring(aStart, aEnd - aStart);
        var bString = memory.Substring(bStart, bEnd - bStart);

        var a = int.Parse(aString);
        var b = int.Parse(bString);

        var mulResult = a * b;

        mulInstruction = new Instruction(InstructionType.Multiply, startIndex, mulResult);
        return true;
    }

    private bool IsNumeric(char c)
    {
        return c >= '0' && c <= '9';
    }

    private class Instruction
    {
        public InstructionType InstructionType { get; }
        public int Position { get; }
        public int Value { get; }

        public Instruction(InstructionType instructionType, int position, int value = 0)
        {
            InstructionType = instructionType;
            Position = position;
            Value = value;
        }
    }

    private enum InstructionType
    {
        Multiply,
        Do,
        Dont
    }
}
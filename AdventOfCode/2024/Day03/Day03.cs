using System.Text.RegularExpressions;
using AdventOfCode.Shared;
using Microsoft.VisualBasic;

namespace AdventOfCode._2024.Day03;

public class Day03 : Day
{
    public Day03() : base(2024, 3, "Day03/input_2024_03.txt", "", "")
    {

    }

    public override void Initialise()
    {
    }

    public override string Part1()
    {
        var result = InputLines.Sum(GetResult);

        return result.ToString();
    }

    private int GetResult(string memory)
    {
        //var mulRegex = new Regex("mul\(\)");

        var result = 0;
        var searchFrom = 0;
        while (true)
        {
            var mulIndex = memory.IndexOf("mul(", searchFrom);
            if (mulIndex < 0)
            {
                break;
            }

            if (TryReadMulResult(memory, mulIndex + "mul(".Length, out var mulResult))
            {
                result += mulResult;
            }

            searchFrom = mulIndex + 1;
        }

        //var matches = mulRegex.Matches(_memory);

        return result;
    }

    private bool TryReadMulResult(string memory, int startIndex, out int mulResult)
    {
        mulResult = 0;

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

        mulResult = a * b;
        return true;
    }

    private bool IsNumeric(char c)
    {
        return c >= '0' && c <= '9';
    }

    public override string Part2()
    {
        return 1.ToString();
    }

    private class Report
    {
        public Report(string report)
        {
        }
    }
}
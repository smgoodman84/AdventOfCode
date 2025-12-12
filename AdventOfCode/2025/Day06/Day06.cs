using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.Numbers;

namespace AdventOfCode._2025.Day06;

public class Day06 : Day
{
    public Day06() : base(2025, 6, "Day06/input_2025_06.txt", "5595593539811", "10153315705125", false)
    {

    }

    public override string Part1()
    {
        var splitLines = InputLines
            .Select(l => l.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .ToList();

        var inputs = splitLines
            .Take(splitLines.Count - 1)
            .Select(x => x.Select(long.Parse).ToList())
            .ToList();
        
        var operations = splitLines.Last();
        
        var results = new List<long>();

        for (var i = 0; i < operations.Length; i++)
        {
            var operation = operations[i];
            var result = inputs[0][i];
            
            for (var row = 1; row < inputs.Count; row++)
            {
                if (operation == "+")
                {
                    result += inputs[row][i];
                }
                
                if (operation == "*")
                {
                    result *= inputs[row][i];
                }
            }
            
            results.Add(result);
        }

        return results.Sum().ToString();
    }

    public override string Part2()
    {
        var operations = InputLines
            .Last()
            .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        operations.Reverse();

        var inputs = InputLines.Take(InputLines.Count - 1).ToList();
        var maxLength = inputs.Max(l => l.Length);

        var transformedInputs = new List<string>();
        for (var i = maxLength - 1; i >= 0; i--)
        {
            var thisInput = "";
            for (var row = 0; row < inputs.Count; row++)
            {
                if (i < inputs[row].Length)
                {
                    thisInput += inputs[row][i];
                }
            }
            transformedInputs.Add(thisInput);
        }

        var results = new List<long>();
        var inputIndex = 0;
        foreach (var operation in operations)
        {
            long result = 0;
            if (operation == "*")
            {
                result = 1;
            }

            while (inputIndex < transformedInputs.Count
                   && !string.IsNullOrWhiteSpace(transformedInputs[inputIndex]))
            {
                var input = long.Parse(transformedInputs[inputIndex].Trim());
                inputIndex += 1;
                if (operation == "+")
                {
                    result += input;
                }
                
                if (operation == "*")
                {
                    result *= input;
                }
            }
            inputIndex += 1;
            results.Add(result);
        }
        
        return results.Sum().ToString();
    }
}
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.Numbers;

namespace AdventOfCode._2025.Day06;

public class Day06 : Day
{
    public Day06() : base(2025, 6, "Day06/input_2025_06.txt", "5595593539811", "", false)
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
        return string.Empty;
    }
}
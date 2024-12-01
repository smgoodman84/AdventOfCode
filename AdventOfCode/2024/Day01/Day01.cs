using AdventOfCode.Shared;

namespace AdventOfCode._2024.Day01;

public class Day01 : Day
{
    public Day01() : base(2024, 1, "Day01/input_2024_01.txt", "2196996", "23655822")
    {

    }

    private List<int> _columnOneNumbers;
    private List<int> _columnTwoNumbers;

    public override void Initialise()
    {
        var parsedLines = InputLines
            .Select(line => 
                line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(int.Parse)
                    .ToArray())
            .ToList();

        _columnOneNumbers = parsedLines.Select(l => l[0]).ToList();
        _columnTwoNumbers = parsedLines.Select(l => l[1]).ToList();
    }

    public override string Part1()
    {
        var sortedOne = _columnOneNumbers.OrderBy(x => x).ToArray();
        var sortedTwo = _columnTwoNumbers.OrderBy(x => x).ToArray();

        var totalDistance = 0;
        for (var i = 0; i < sortedOne.Length; i++)
        {
            var distance = Math.Abs(sortedOne[i] - sortedTwo[i]);
            totalDistance += distance;
        }
        
        return totalDistance.ToString();
    }

    public override string Part2()
    {
        var similarity = 0;
        foreach(var x in _columnOneNumbers)
        {
            var columnTwoCount = _columnTwoNumbers.Count(y => y == x);
            similarity += x * columnTwoCount;
        }

        return similarity.ToString();
    }
}
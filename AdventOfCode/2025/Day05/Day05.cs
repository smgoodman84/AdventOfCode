using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.Numbers;

namespace AdventOfCode._2025.Day05;

public class Day05 : Day
{
    public Day05() : base(2025, 5, "Day05/input_2025_05.txt", "558", "344813017450467", false)
    {

    }

    private List<NumberRange> _freshRanges = new List<NumberRange>();
    private List<long> _availableIngredients = new List<long>();
    
    public override void Initialise()
    {
        var inIngredients = false;
        foreach (var line in InputLines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                inIngredients = true;
                continue;
            }

            if (inIngredients)
            {
                _availableIngredients.Add(long.Parse(line));
                continue;
            }

            var elements = line.Split("-").Select(long.Parse).ToArray();
            _freshRanges.AddRange(new NumberRange(elements[0], elements[1]));
        }
    }
    
    private bool IsFresh(long ingredientId)
    {
        return _freshRanges.Any(r => r.Contains(ingredientId));
    }

    public override string Part1()
    {
        var freshCount = _availableIngredients.Count(IsFresh);
        return freshCount.ToString();
    }

    public override string Part2()
    {
        var mergedRanges = NumberRangeMerger.Merge(_freshRanges);
        var total = mergedRanges.Sum(r => r.Length);
        return total.ToString();
    }
}
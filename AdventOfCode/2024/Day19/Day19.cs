using AdventOfCode.Shared;

namespace AdventOfCode._2024.Day19;

public class Day19 : Day
{
    public Day19() : base(2024, 19, "Day19/input_2024_19.txt", "240", "848076019766013", true)
    {
    }

    private List<string> _towels;
    private List<string> _patterns;
    public override void Initialise()
    {
        _towels = InputLines.First()
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        _patterns = InputLines.Skip(2).ToList();
    }

    public override string Part1()
    {
        var result = _patterns.Count(IsPossible);

        return result.ToString();
    }

    public override string Part2()
    {
        var result = _patterns.Sum(PossibleWays);

        return result.ToString();
    }

    private bool IsPossible(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            return true;
        }

        var possibleFirstTowels = _towels
            .Where(t => pattern.StartsWith(t))
            .ToList();

        var possibleFirstTowelLengths = possibleFirstTowels
            .Select(t => t.Length)
            .ToList();
        
        return possibleFirstTowelLengths
            .Any(tl => IsPossible(pattern.Substring(tl)));
    }

    private Dictionary<string, long> _cache = new ();
    private long PossibleWays(string pattern)
    {
        if (_cache.TryGetValue(pattern, out var cacheResult))
        {
            return cacheResult;
        }

        if (string.IsNullOrEmpty(pattern))
        {
            return 1;
        }

        var possibleFirstTowels = _towels
            .Where(t => pattern.StartsWith(t))
            .ToList();

        var possibleFirstTowelLengths = possibleFirstTowels
            .Select(t => t.Length)
            .ToList();
        
        var result = possibleFirstTowelLengths
            .Sum(tl => PossibleWays(pattern.Substring(tl)));
        
        _cache.Add(pattern, result);

        return result;
    }
}
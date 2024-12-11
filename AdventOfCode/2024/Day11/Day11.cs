using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2024.Day11;

public class Day11 : Day
{
    public Day11() : base(2024, 11, "Day11/input_2024_11.txt", "199982", "237149922829154", false)
    {
    }
    
    private List<long> _initialStones;
    private Dictionary<string, long> _cache = new Dictionary<string, long>();
    public override void Initialise()
    {
        _initialStones = InputLines[0]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(long.Parse)
            .ToList();
    }

    public override string Part1()
    {
        var count = _initialStones
            .Sum(stone => GetStoneCountCached(stone, 25, _cache));

        return count.ToString();
    }

    public override string Part2()
    {
        long count = 0;
        foreach (var stone in _initialStones)
        {
            count += GetStoneCountCached(stone, 75, _cache);
        }

        return count.ToString();
    }

    private long GetStoneCountCached(
        long stone,
        int blinks, 
        Dictionary<string, long> cache)
    {
        long result;

        var cacheKey = $"{stone}_{blinks}";
        if (!cache.TryGetValue(cacheKey, out result)) 
        {
            result = GetStoneCount(stone, blinks, cache);
            cache.Add(cacheKey, result);
        }

        return result;
    }

    private long GetStoneCount(
        long stone,
        int blinks, 
        Dictionary<string, long> cache)
    {
        if (blinks == 0)
        {
            return 1;
        }

        if (stone == 0)
        {
            return GetStoneCountCached(1, blinks - 1, cache);
        }

        var stoneString = stone.ToString();
        if (stoneString.Length % 2 == 0)
        {
            var midPoint = stoneString.Length / 2;

            var leftString = stoneString.Substring(0, midPoint);
            var rightString = stoneString.Substring(midPoint);
            var left = long.Parse(leftString);
            var right = long.Parse(rightString);

            return GetStoneCountCached(left, blinks - 1, cache)
                + GetStoneCountCached(right, blinks - 1, cache);
        }

        return GetStoneCountCached(stone * 2024, blinks - 1, cache);
    }
}
using AdventOfCode.Shared;

namespace AdventOfCode._2025.Day01;

public class Day02 : Day
{
    public Day02() : base(2025, 2, "Day02/input_2025_02.txt", "19386344315", "34421651192", false)
    {

    }

    private List<(long Start, long End)> _ranges;

    public override void Initialise()
    {
        _ranges = InputLines[0]
            .Split(',')
            .Select(r => r.Split('-').Select(long.Parse).ToArray())
            .Select(r => (r[0], r[1]))
            .ToList();
    }

    public override string Part1()
    {
        long total = 0;
        foreach (var range in _ranges)
        {
            var invalidNumbers = LongRange(range.Start, range.End)
                .Where(IsInvalid)
                .ToList();

            total += invalidNumbers.Sum();
        }

        return total.ToString();
    }

    private static IEnumerable<long> LongRange(long start, long end)
    {
        var current = start;
        while (current <= end)
        {
            yield return current;
            current += 1;
        }
    }
    
    private static bool IsInvalid(long number)
    {
        var numString = number.ToString();
        if (numString.Length % 2 == 1)
        {
            return false;
        }

        var midPoint = numString.Length / 2;
        var left = numString.Substring(0, midPoint);
        var right = numString.Substring(midPoint);

        return left == right;
    }

    public override string Part2()
    {
        long total = 0;
        foreach (var range in _ranges)
        {
            var invalidNumbers = LongRange(range.Start, range.End)
                .Where(IsInvalid2)
                .ToList();

            total += invalidNumbers.Sum();
        }

        return total.ToString();
    }
    
    private bool IsInvalid2(long number)
    {
        var numString = number.ToString();
        for (var divisor = 2; divisor <= numString.Length; divisor++)
        {
            if (IsInvalid2(numString, divisor))
            {
                TraceLine($"{numString} Invalid");
                return true;
            }
        }

        TraceLine($"{numString} Valid");
        return false;
    }
    
    private static bool IsInvalid2(string numString, int partCount)
    {
        if (numString.Length % partCount != 0)
        {
            return false;
        }

        var partLength = numString.Length / partCount;

        var parts = Enumerable.Range(0, partCount)
            .Select(i => numString.Substring(i * partLength, partLength))
            .ToList();

        return parts.Distinct().Count() == 1;
    }
}
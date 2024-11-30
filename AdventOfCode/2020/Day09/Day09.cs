using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2020.Day09;

public class Day09 : Day
{
    public Day09() : base(2020, 9, "Day09/input_2020_09.txt", "22477624", "2980044")
    {

    }

    private long[] _data;
    public override void Initialise()
    {
        _data = InputLines
            .Select(long.Parse)
            .ToArray();
    }

    public override string Part1() => Part1Internal().ToString();

    public long Part1Internal()
    {
        var window = 25;
        for (var i = window; i < _data.Length; i++)
        {
            var inPairsWithPrevious = CheckInPairsWithPrevious(i, window);
            if (!inPairsWithPrevious)
            {
                return _data[i];
            }
        }

        return -1;
    }

    private bool CheckInPairsWithPrevious(int index, int window)
    {
        var item = _data[index];
        for (var a = index - window; a < index; a++)
        {
            for (var b = index - window; b < index; b++)
            {
                if (_data[a] + _data[b] == item)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public override string Part2()
    {
        var target = Part1Internal();
        var start = 0;
        var end = 1;
        var total = _data[start] + _data[end];

        while (end < _data.Length)
        {
            while (total < target && end < _data.Length)
            {
                end += 1;
                total += _data[end];
            }

            if (total == target)
            {
                var numbers = _data.Skip(start).Take(end - start);
                return (numbers.Min() + numbers.Max()).ToString();
            }

            while (total > target && start <= end - 2)
            {
                total -= _data[start];
                start += 1;
            }
        }

        return string.Empty;
    }
}
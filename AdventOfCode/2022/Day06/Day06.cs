using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2022.Day02;

public class Day06 : Day
{
    public Day06() : base(2022, 6, "Day06/input_2022_06.txt", "1816", "2625")
    {

    }

    private string _signal;
    public override void Initialise()
    {
        _signal = InputLines.Single();
    }

    public override string Part1()
    {
        return FindIndexOfDistinctCharacters(4).ToString();
    }

    public override string Part2()
    {
        return FindIndexOfDistinctCharacters(14).ToString();
    }

    private int FindIndexOfDistinctCharacters(int distinctCount)
    {
        return new DistinctCheck(distinctCount).FindFirstDistinctIndex(_signal);
    }

    private int FindIndexOfDistinctCharactersNaive(int distinctCount)
    {
        for (var index = distinctCount; index <= _signal.Length; index++)
        {
            var lastCharacters = _signal.Substring(index - distinctCount, distinctCount);

            if (lastCharacters.ToArray().Distinct().Count() == distinctCount)
            {
                return index;
            }
        }

        return -1;
    }

    private class DistinctCheck
    {
        private int _index;
        private readonly int _length;
        private readonly int[] _mostRecentlySeen;
        private int _mostRecentDuplicate;

        public DistinctCheck(int length)
        {
            _index = 0;
            _length = length;
            _mostRecentlySeen = new int[256];
        }

        public void Push(char c)
        {
            _index += 1;

            var mostRecentIndex = _mostRecentlySeen[c];
            if (mostRecentIndex != 0)
            {
                if (_index - mostRecentIndex < _length)
                {
                    if (mostRecentIndex > _mostRecentDuplicate)
                    {
                        _mostRecentDuplicate = mostRecentIndex;
                    }
                }
            }

            _mostRecentlySeen[c] = _index;
        }

        public bool IsDistinct()
        {
            return _index - _mostRecentDuplicate >= _length;
        }

        public int FindFirstDistinctIndex(string input)
        {
            foreach (var c in input)
            {
                Push(c);
                if (IsDistinct())
                {
                    return _index;
                }
            }

            return -1;
        }
    }
}
using AdventOfCode.Shared;

namespace AdventOfCode._2024.Day04;

public class Day04 : Day
{
    public Day04() : base(2024, 4, "Day04/input_2024_04.txt", "2575", "")
    {

    }

    private char[][] _wordsearch;
    private List<string> _wordsearchLines;
    public override void Initialise()
    {
        _wordsearch = InputLines
            .Select(l => l.ToArray())
            .ToArray();

        var stringsToSearch = new List<string>();
        var height = _wordsearch.Length;
        var width = _wordsearch[0].Length;

        // horizontal
        foreach (var word in _wordsearch)
        {
            var forward = string.Join("", word);
            var backward = string.Join("", word.Reverse());
            stringsToSearch.Add(forward);
            stringsToSearch.Add(backward);
        }

        // vertical
        for (var i = 0; i < width; i++)
        {
            var column = _wordsearch.Select(l => l[i]).ToArray();
            var forward = string.Join("", column);
            var backward = string.Join("", column.Reverse());
            stringsToSearch.Add(forward);
            stringsToSearch.Add(backward);
        }

        // rising diagonal
        for (var yStart = 0; yStart < width + height - 2; yStart++)
        {
            var word = string.Empty;
            var y = yStart;
            for (var x = 0; x < width; x++, y--)
            {
                if (y >= 0 && y < height)
                {
                    word = word + _wordsearch[y][x];
                }
            }

            var backward = string.Join("", word.Reverse());
            stringsToSearch.Add(word);
            stringsToSearch.Add(backward);
        }

        // falling diagonal
        for (var yStart = height - 1; yStart > -width; yStart--)
        {
            var word = string.Empty;
            var y = yStart;
            for (var x = 0; x < width; x++, y++)
            {
                if (y >= 0 && y < height)
                {
                    word = word + _wordsearch[y][x];
                }
            }

            var backward = string.Join("", word.Reverse());
            stringsToSearch.Add(word);
            stringsToSearch.Add(backward);
        }

        _wordsearchLines = stringsToSearch;
    }

    private int InstancesOf(string haystack, string needle)
    {
        if (needle.Length > haystack.Length)
        {
            return 0;
        }

        var count = 0;
        for (var startIndex = 0; startIndex < haystack.Length - needle.Length + 1; startIndex += 1)
        {
            if (haystack.Substring(startIndex, needle.Length).Equals(needle, StringComparison.Ordinal))
            {
                count += 1;
            }
        }

        return count;
    }

    public override string Part1()
    {
        var count = 0;
        foreach (var word in _wordsearchLines)
        {
            count += InstancesOf(word, "XMAS");
            TraceLine($"{word} - {count}");
        }

        return count.ToString();
    }

    public override string Part2()
    {
        return string.Empty;
    }
}
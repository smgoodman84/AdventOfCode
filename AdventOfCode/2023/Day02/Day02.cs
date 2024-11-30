using AdventOfCode.Shared;

namespace AdventOfCode._2023.Day02;

public class Day02 : Day
{
    public Day02() : base(2023, 2, "Day02/input_2023_02.txt", "2101", "58269")
    {

    }

    private List<Game> _games;
    public override void Initialise()
    {
        _games = InputLines
            .Select(l => new Game(l))
            .ToList();
    }

    public override string Part1()
    {
        var possibleGames = _games
            .Where(g => g.IsPossibleWith("red", 12))
            .Where(g => g.IsPossibleWith("green", 13))
            .Where(g => g.IsPossibleWith("blue", 14))
            .ToList();

        var sum = possibleGames.Sum(g => g.GameNuber);

        return sum.ToString();
    }

    public override string Part2()
    {
        var result = _games.Sum(g => g.GetPower());

        return result.ToString();
    }

    private class Game
    {
        public int GameNuber { get; set; }
        public List<Draw> Draws { get; set; } = new List<Draw>();

        public Game(string line)
        {
            var gameSplit = line.Split(":");

            GameNuber = int.Parse(gameSplit[0].Replace("Game ", ""));

            var draws = gameSplit[1].Split(";", StringSplitOptions.TrimEntries);
            foreach (var drawDescription in draws)
            {
                var draw = new Draw();

                var colours = drawDescription.Split(",", StringSplitOptions.TrimEntries);
                foreach (var colour in colours)
                {
                    var countSplit = colour.Split(" ");
                    draw.ColourCounts.Add(countSplit[1], int.Parse(countSplit[0]));
                }

                Draws.Add(draw);
            }
        }

        public bool IsPossibleWith(string colour, int count)
        {
            return !Draws.Any(d => d.ColourCounts.ContainsKey(colour) && d.ColourCounts[colour] > count);
        }

        public int GetPower()
        {
            var minimumCubes = new Dictionary<string, int>();
            foreach (var draw in Draws)
            {
                foreach (var colour in draw.ColourCounts)
                {
                    if (!minimumCubes.ContainsKey(colour.Key))
                    {
                        minimumCubes.Add(colour.Key, colour.Value);
                    }
                    else
                    {
                        if (minimumCubes[colour.Key] < colour.Value)
                        {
                            minimumCubes[colour.Key] = colour.Value;
                        }
                    }
                }
            }

            var power = 1;
            foreach (var colour in minimumCubes)
            {
                power *= colour.Value;
            }

            return power;
        }
    }

    private class Draw
    {
        public Dictionary<string, int> ColourCounts { get; set; } = new Dictionary<string, int>();
    }
}
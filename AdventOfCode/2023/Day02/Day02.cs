using AdventOfCode.Shared;

namespace AdventOfCode._2023.Day02
{
    public class Day02 : Day
    {
        public Day02() : base(2023, 2, "Day02/input_2023_02.txt", "55538", "54875")
        {

        }

        private List<Game> _games;
        public override void Initialise()
        {
            _games = InputLines.Select(Parse).ToList();
        }

        public override string Part1()
        {
            var possibleGames = _games
                .Where(g => !g.Draws.Any(d => d.ColourCounts.ContainsKey("red") && d.ColourCounts["red"] > 12))
                .Where(g => !g.Draws.Any(d => d.ColourCounts.ContainsKey("green") && d.ColourCounts["green"] > 13))
                .Where(g => !g.Draws.Any(d => d.ColourCounts.ContainsKey("blue") && d.ColourCounts["blue"] > 14))
                .ToList();

            var sum = possibleGames.Sum(g => g.GameNuber);

            return sum.ToString();
        }

        public override string Part2()
        {
            var result = _games.Sum(GetPower);

            return result.ToString();
        }

        private static int GetPower(Game game)
        {
            var minimumCubes = new Dictionary<string, int>();
            foreach(var draw in game.Draws)
            {
                foreach(var colour in draw.ColourCounts)
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

        private static Game Parse(string line)
        {
            var gameSplit = line.Split(":");
            var gameNumber = int.Parse(gameSplit[0].Replace("Game ", ""));
            var game = new Game
            {
                GameNuber = gameNumber,
                Draws = new List<Draw>(),
            };

            Console.WriteLine($"Game {game.GameNuber}");
            var draws = gameSplit[1].Split(";", StringSplitOptions.TrimEntries);
            foreach (var drawDescription in draws)
            {
                Console.WriteLine($"{drawDescription}");
                var colours = drawDescription.Split(",", StringSplitOptions.TrimEntries);
                var draw = new Draw
                {
                    ColourCounts = new Dictionary<string, int>()
                };

                foreach (var colour in colours)
                {
                    Console.WriteLine($"{colour}");
                    var countSplit = colour.Split(" ");
                    Console.WriteLine($"{countSplit[1]} ({countSplit[0]})");
                    draw.ColourCounts.Add(countSplit[1], int.Parse(countSplit[0]));
                }

                game.Draws.Add(draw);
            }

            return game;
        }

        private class Game
        {
            public int GameNuber { get; set; }
            public List<Draw> Draws { get; set; }
        }

        private class Draw
        {
            public Dictionary<string, int> ColourCounts { get; set; }
        }
    }
}


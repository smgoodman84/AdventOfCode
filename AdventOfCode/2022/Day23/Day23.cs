using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Shared;
using AdventOfCode.Shared.DataStructures;
using AdventOfCode.Shared.Geometry;
using static System.Net.Mime.MediaTypeNames;

namespace AdventOfCode._2022.Day23;

public class Day23 : Day
{
    public Day23() : base(2022, 23, "Day23/input_2022_23.txt", "3987", "938")
    {

    }

    public override void Initialise()
    {
    }

    private void Print(IGrid2D<char> space)
    {
        Console.WriteLine(space.Print(c => c));
    }

    public override string Part1()
    {
        var currentState = new InfiniteGrid2D<char>('.');
        var alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        var elvesAdded = 0;

        var y = 0;
        foreach (var line in InputLines)
        {
            y += 1;
            var x = 0;
            foreach (var c in line)
            {
                x += 1;
                if (c == '#')
                {
                    currentState.Write(x, y, alphabet[elvesAdded]);
                    elvesAdded += 1;
                    elvesAdded %= alphabet.Length;
                }
            }
        }
        var _considerationRing = new Ring<Compass>(new[] { Compass.North, Compass.South, Compass.West, Compass.East });

        var roundsLeft = 10;
        var round = 0;

        Console.WriteLine("Initial State");
        Print(currentState);

        while (roundsLeft > 0)
        {
            var elfCoordinates = currentState.FindCoordinates(x => x != '.');
            var proposedMoves = new List<ProposedMove>();
            foreach (var elfCoordinate in elfCoordinates)
            {
                var elf = currentState.Read(elfCoordinate);
                if (elf =='L')
                {
                    // var stop = true;
                }
                var proposalMade = false;
                var considerationsLeft = 4;
                while (considerationsLeft > 0)
                {
                    var nextConsideration = _considerationRing.CurrentThenMoveNext();

                    if (!proposalMade)
                    {
                        var northOccupiedBy = currentState.Read(elfCoordinate.South());
                        var northEastOccupiedBy = currentState.Read(elfCoordinate.SouthEast());
                        var eastOccupiedBy = currentState.Read(elfCoordinate.East());
                        var southEastOccupiedBy = currentState.Read(elfCoordinate.NorthEast());
                        var southOccupiedBy = currentState.Read(elfCoordinate.North());
                        var southWestOccupiedBy = currentState.Read(elfCoordinate.NorthWest());
                        var westOccupiedBy = currentState.Read(elfCoordinate.West());
                        var northWestOccupiedBy = currentState.Read(elfCoordinate.SouthWest());

                        if (new[]
                            {
                                northOccupiedBy,
                                northEastOccupiedBy,
                                eastOccupiedBy,
                                southEastOccupiedBy,
                                southOccupiedBy,
                                southWestOccupiedBy,
                                westOccupiedBy,
                                northWestOccupiedBy,
                            }.All(x => x == '.'))
                        {
                            proposalMade = true;
                        }
                        else
                        {
                            switch (nextConsideration)
                            {
                                case Compass.North:
                                    if (new[] { northOccupiedBy, northEastOccupiedBy, northWestOccupiedBy }.All(x => x == '.'))
                                    {
                                        proposedMoves.Add(new ProposedMove { Start = elfCoordinate, End = elfCoordinate.South() });
                                        proposalMade = true;
                                    }
                                    break;
                                case Compass.South:
                                    if (new[] { southOccupiedBy, southEastOccupiedBy, southWestOccupiedBy }.All(x => x == '.'))
                                    {
                                        proposedMoves.Add(new ProposedMove { Start = elfCoordinate, End = elfCoordinate.North() });
                                        proposalMade = true;
                                    }
                                    break;
                                case Compass.West:
                                    if (new[] { westOccupiedBy, northWestOccupiedBy, southWestOccupiedBy }.All(x => x == '.'))
                                    {
                                        proposedMoves.Add(new ProposedMove { Start = elfCoordinate, End = elfCoordinate.West() });
                                        proposalMade = true;
                                    }
                                    break;
                                case Compass.East:
                                    if (new[] { eastOccupiedBy, northEastOccupiedBy, southEastOccupiedBy }.All(x => x == '.'))
                                    {
                                        proposedMoves.Add(new ProposedMove { Start = elfCoordinate, End = elfCoordinate.East() });
                                        proposalMade = true;
                                    }
                                    break;
                            }
                        }
                    }

                    considerationsLeft -= 1;
                }


            }

            var actualMoves = proposedMoves
                .GroupBy(x => x.End.ToString())
                .Where(g => g.Count() == 1)
                .SelectMany(g => g)
                .ToList();

            foreach (var move in actualMoves)
            {
                var elf = currentState.Read(move.Start);
                Console.WriteLine($"{elf} {move.Start} moving to {move.End}");
                currentState.Write(move.Start, '.');
                currentState.Write(move.End, elf);
            }

            roundsLeft -= 1;
            round += 1;


            Console.WriteLine($"After round {round}");
            Print(currentState);



            var skipped = _considerationRing.CurrentThenMoveNext();
            Console.WriteLine($"Skipped {skipped}");
        }

        var allElves = currentState.FindCoordinates(x => x != '.');
        var minX = allElves.Min(c => c.X);
        var maxX = allElves.Max(c => c.X);
        var minY = allElves.Min(c => c.Y);
        var maxY = allElves.Max(c => c.Y);

        var width = maxX - minX + 1;
        var height = maxY - minY + 1;

        var spaceCount = 0;
        for (var x = minX; x <= maxX; x++)
        {
            for (var cy = minY; cy <= maxY; cy++)
            {
                if (currentState.Read(x, cy) == '.')
                {
                    spaceCount += 1;
                }
            }
        }


        return spaceCount.ToString();
    }

    public override string Part2()
    {
        var currentState = new InfiniteGrid2D<char>('.');
        var alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        var elvesAdded = 0;

        var y = 0;
        foreach (var line in InputLines)
        {
            y += 1;
            var x = 0;
            foreach (var c in line)
            {
                x += 1;
                if (c == '#')
                {
                    currentState.Write(x, y, alphabet[elvesAdded]);
                    elvesAdded += 1;
                    elvesAdded %= alphabet.Length;
                }
            }
        }
        var _considerationRing = new Ring<Compass>(new[] { Compass.North, Compass.South, Compass.West, Compass.East });

        var roundsLeft = 10;
        var round = 0;

        Console.WriteLine("Initial State");
        Print(currentState);
        var anyMovement = true;
        while (anyMovement)
        {
            anyMovement = false;
            var elfCoordinates = currentState.FindCoordinates(x => x != '.');
            var proposedMoves = new List<ProposedMove>();
            foreach (var elfCoordinate in elfCoordinates)
            {
                var elf = currentState.Read(elfCoordinate);
                if (elf == 'L')
                {
                    // var stop = true;
                }
                var proposalMade = false;
                var considerationsLeft = 4;
                while (considerationsLeft > 0)
                {
                    var nextConsideration = _considerationRing.CurrentThenMoveNext();

                    if (!proposalMade)
                    {
                        var northOccupiedBy = currentState.Read(elfCoordinate.South());
                        var northEastOccupiedBy = currentState.Read(elfCoordinate.SouthEast());
                        var eastOccupiedBy = currentState.Read(elfCoordinate.East());
                        var southEastOccupiedBy = currentState.Read(elfCoordinate.NorthEast());
                        var southOccupiedBy = currentState.Read(elfCoordinate.North());
                        var southWestOccupiedBy = currentState.Read(elfCoordinate.NorthWest());
                        var westOccupiedBy = currentState.Read(elfCoordinate.West());
                        var northWestOccupiedBy = currentState.Read(elfCoordinate.SouthWest());

                        if (new[]
                            {
                                northOccupiedBy,
                                northEastOccupiedBy,
                                eastOccupiedBy,
                                southEastOccupiedBy,
                                southOccupiedBy,
                                southWestOccupiedBy,
                                westOccupiedBy,
                                northWestOccupiedBy,
                            }.All(x => x == '.'))
                        {
                            proposalMade = true;
                        }
                        else
                        {
                            switch (nextConsideration)
                            {
                                case Compass.North:
                                    if (new[] { northOccupiedBy, northEastOccupiedBy, northWestOccupiedBy }.All(x => x == '.'))
                                    {
                                        proposedMoves.Add(new ProposedMove { Start = elfCoordinate, End = elfCoordinate.South() });
                                        proposalMade = true;
                                    }
                                    break;
                                case Compass.South:
                                    if (new[] { southOccupiedBy, southEastOccupiedBy, southWestOccupiedBy }.All(x => x == '.'))
                                    {
                                        proposedMoves.Add(new ProposedMove { Start = elfCoordinate, End = elfCoordinate.North() });
                                        proposalMade = true;
                                    }
                                    break;
                                case Compass.West:
                                    if (new[] { westOccupiedBy, northWestOccupiedBy, southWestOccupiedBy }.All(x => x == '.'))
                                    {
                                        proposedMoves.Add(new ProposedMove { Start = elfCoordinate, End = elfCoordinate.West() });
                                        proposalMade = true;
                                    }
                                    break;
                                case Compass.East:
                                    if (new[] { eastOccupiedBy, northEastOccupiedBy, southEastOccupiedBy }.All(x => x == '.'))
                                    {
                                        proposedMoves.Add(new ProposedMove { Start = elfCoordinate, End = elfCoordinate.East() });
                                        proposalMade = true;
                                    }
                                    break;
                            }
                        }
                    }

                    considerationsLeft -= 1;
                }


            }

            var actualMoves = proposedMoves
                .GroupBy(x => x.End.ToString())
                .Where(g => g.Count() == 1)
                .SelectMany(g => g)
                .ToList();

            foreach (var move in actualMoves)
            {
                anyMovement = true;
                var elf = currentState.Read(move.Start);
                //Console.WriteLine($"{elf} {move.Start} moving to {move.End}");
                currentState.Write(move.Start, '.');
                currentState.Write(move.End, elf);
            }

            roundsLeft -= 1;
            round += 1;


            Console.WriteLine($"After round {round}");
            //Print(currentState);



            var skipped = _considerationRing.CurrentThenMoveNext();
            //Console.WriteLine($"Skipped {skipped}");
        }

        var allElves = currentState.FindCoordinates(x => x != '.');
        var minX = allElves.Min(c => c.X);
        var maxX = allElves.Max(c => c.X);
        var minY = allElves.Min(c => c.Y);
        var maxY = allElves.Max(c => c.Y);

        var width = maxX - minX + 1;
        var height = maxY - minY + 1;

        var spaceCount = 0;
        for (var x = minX; x <= maxX; x++)
        {
            for (var cy = minY; cy <= maxY; cy++)
            {
                if (currentState.Read(x, cy) == '.')
                {
                    spaceCount += 1;
                }
            }
        }

        Print(currentState);

        return round.ToString();
    }

    private class ProposedMove
    {
        public Coordinate2D Start { get; set; }
        public Coordinate2D End { get; set; }
    }

    private enum Space
    {
        Empty,
        Elf
    }
}
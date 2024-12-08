using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode.Shared;
using AdventOfCode.Shared.DataStructures;
using AdventOfCode.Shared.FileProcessing;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2022.Day17;

public class Day17 : Day
{
    public Day17() : base(2022, 17, "Day17/input_2022_17.txt", "3071", "1523615160362")
    {

    }


    private const string Rocks = @"
####

.#.
###
.#.

..#
..#
###

#
#
#
#

##
##
";

    private List<Grid2D<Space>> _rocks;
    private List<Move> _moves;
    public override void Initialise()
    {
        _moves = Parse(InputLines.Single());
        _rocks = ParsePieces();
    }

    private List<Grid2D<Space>> ParsePieces()
    {
        var results = new List<Grid2D<Space>>();

        var rocks = LineGrouper.GroupLinesBySeperator(Rocks.Split(Environment.NewLine));

        foreach (var rock in rocks.Where(p => p.Count > 0))
        {
            var height = rock.Count;
            var left = rock.Min(x => x.IndexOf('#'));
            var right = rock.Max(x => x.LastIndexOf('#'));
            var width = right - left + 1;
            var grid = new Grid2D<Space>(width, height);

            var y = height - 1;
            foreach (var line in rock)
            {
                var x = 0;
                foreach (var c in line)
                {
                    switch (c)
                    {
                        case ' ':
                            grid.Write(x, y, Space.Empty);
                            break;
                        case '.':
                            grid.Write(x, y, Space.Empty);
                            break;
                        case '#':
                            grid.Write(x, y, Space.Rock);
                            break;
                    }
                    x += 1;
                }
                y -= 1;
            }

            results.Add(grid);
        }

        return results;
    }

    private enum Space
    {
        Empty,
        Rock
    }

    private enum Move
    {
        Left,
        Right
    }

    private List<Move> Parse(string input)
    {
        var list = new List<Move>();
        foreach (var c in input)
        {
            if (c == '<')
            {
                list.Add(Move.Left);
            }
            else if (c == '>')
            {
                list.Add(Move.Right);
            }
        }
        return list;
    }

    public override string Part1()
    {
        return new RockDrop(new Ring<Grid2D<Space>>(_rocks), new Ring<Move>(_moves), 2022)
            .GetHeightAfterRocks()
            .ToString();
    }

    public override string Part2()
    {
        return new RockDrop(new Ring<Grid2D<Space>>(_rocks), new Ring<Move>(_moves), 1000000000000)
            .GetHeightAfterRocks()
            .ToString();
    }


    private class RockDrop
    {
        private readonly Ring<Grid2D<Space>> _rocks;
        private readonly Ring<Move> _moves;
        private readonly long _rockCount;
        private readonly Grid2D<Space> _chamber;
        private long _highestRock;
        private long _highestRockOffset = 0;
        private long _rocksSkipped = 0;

        public RockDrop(Ring<Grid2D<Space>> rocks, Ring<Move> moves, long rockCount)
        {
            _rocks = rocks;
            _moves = moves;
            _rockCount = rockCount;

            long loopSize = _rocks.Count * _moves.Count;
            long maxRockHeight = _rocks.Max(p => p.Height);
            long simpleMaxHeight = (int)(maxRockHeight * loopSize * 2) + 3;
            long complexMaxHeight = (_rocks.Max(p => p.Height) * (rockCount + 1)) + 3;
            var maxHeight = (int)long.Min(simpleMaxHeight, complexMaxHeight);
            _chamber = new Grid2D<Space>(7, maxHeight);
            _highestRock = 0;
        }

        private string GetCurrentContext()
        {
            var rockIndex = _rocks.Index;
            var moveIndex = _moves.Index;
            var top = DrawChamber(5).ReplaceLineEndings("_");
            return $"{rockIndex}_{moveIndex}_{top}";
        }

        private readonly Dictionary<string, (long rockNumber, long highestRock)> _contextCache = new Dictionary<string, (long rockNumber, long highestRock)>();

        public long GetHeightAfterRocks()
        {
            long rockNumber = 1;
            while (rockNumber + _rocksSkipped <= _rockCount)
            {
                if (_highestRockOffset == 0)
                {
                    var context = GetCurrentContext();
                    if (_contextCache.ContainsKey(context))
                    {
                        var happenedAt = _contextCache[context];
                        var previousRockNumber = happenedAt.rockNumber;
                        var previousHighestRock = happenedAt.highestRock;

                        var rockLoopLength = rockNumber - previousRockNumber;
                        var loopHeight = _highestRock - previousHighestRock;

                        var rocksRemaining = _rockCount - rockNumber;
                        var loopsToSkip = rocksRemaining / rockLoopLength;

                        _highestRockOffset = loopsToSkip * loopHeight;
                        _rocksSkipped = loopsToSkip * rockLoopLength;
                    }
                    else
                    {
                        _contextCache.Add(context, (rockNumber, _highestRock));
                    }
                }


                var rock = _rocks.CurrentThenMoveNext();
                var rockLocation = new Coordinate2D(2, 3 + _highestRock);

                var rockLanded = false;
                while (!rockLanded)
                {
                    var move = _moves.CurrentThenMoveNext();
                    if (move == Move.Left)
                    {
                        var left = rockLocation.Left();
                        if (CanRockMoveTo(rock, left))
                        {
                            rockLocation = left;
                        }
                    }
                    else if (move == Move.Right)
                    {
                        var right = rockLocation.Right();
                        if (CanRockMoveTo(rock, right))
                        {
                            rockLocation = right;
                        }
                    }

                    var down = rockLocation.Down();
                    if (CanRockMoveTo(rock, down))
                    {
                        rockLocation = down;
                    }
                    else
                    {
                        var highestNewRock = SettleRock(rock, rockLocation) + 1;
                        if (highestNewRock > _highestRock)
                        {
                            _highestRock = highestNewRock;
                        }

                        rockLanded = true;
                    }
                }

                if (rockNumber < 0)
                {
                    Console.WriteLine($"Rock {rockNumber} Landed:");
                    Console.WriteLine(DrawChamber());
                }

                if (rockNumber + _rocksSkipped == _rockCount)
                {
                    return _highestRock + _highestRockOffset;
                }

                rockNumber += 1;
            }

            return -1;
        }

        private int SettleRock(
            Grid2D<Space> rock,
            Coordinate2D rockLocation)
        {
            var maxHeight = 0L;
            for (var y = 0; y < rock.Height; y++)
            {
                for (var x = 0; x < rock.Width; x++)
                {
                    if (rock.Read(x, y) == Space.Rock)
                    {
                        var chamberLocation = rockLocation.Add(new Vector2D(x, y));
                        _chamber.Write(chamberLocation, Space.Rock);
                        if (chamberLocation.Y > maxHeight)
                        {
                            maxHeight = chamberLocation.Y;
                        }
                    }
                }
            }

            return (int)maxHeight;
        }

        private bool CanRockMoveTo(
            Grid2D<Space> rock,
            Coordinate2D rockLocation)
        {
            for (var y = 0; y < rock.Height; y++)
            {
                for (var x = 0; x < rock.Width; x++)
                {
                    if (rock.Read(x, y) == Space.Rock)
                    {
                        var chamberLocation = rockLocation.Add(new Vector2D(x, y));
                        if (!_chamber.IsInGrid(chamberLocation))
                        {
                            return false;
                        }

                        var chamberIsRock = _chamber.Read(chamberLocation) == Space.Rock;
                        if (chamberIsRock)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private string DrawChamber(int rows = -1)
        {
            if (rows == -1)
            {
                rows = (int)_highestRock;
            }

            var drawUntil = _highestRock - rows;
            if (drawUntil < 0)
            {
                drawUntil = 0;
            }

            var chamber = new StringBuilder();
            for (var y = (int)_highestRock; y >= drawUntil; y--)
            {
                chamber.Append('|');
                for (var x = 0; x < _chamber.Width; x++)
                {
                    var isRock = _chamber.Read(x, y) == Space.Rock;
                    if (isRock)
                    {
                        chamber.Append('#');
                    }
                    else
                    {
                        chamber.Append('.');
                    }
                }
                chamber.AppendLine("|");
            }
            return chamber.ToString();
        }
    }
}
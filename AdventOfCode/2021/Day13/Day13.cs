﻿using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2021.Day13;

public class Day13 : Day
{
    private const string _validatedPart2 = @"
#### #  #   ## #  #  ##  #### #  # ### 
   # #  #    # #  # #  # #    #  # #  #
  #  #  #    # #  # #  # ###  #### #  #
 #   #  #    # #  # #### #    #  # ### 
#    #  # #  # #  # #  # #    #  # #   
####  ##   ##   ##  #  # #    #  # #   
";

    private List<Coordinate> _coordinates;
    private List<Fold> _folds;

    public Day13() : base(2021, 13, "Day13/input_2021_13.txt", "737", _validatedPart2)
    {

    }

    private void LoadData()
    {
        _coordinates = InputLines
            .Where(l => l.Contains(','))
            .Select(l => new Coordinate(l))
            .ToList();

        _folds = InputLines
            .Where(l => l.StartsWith("fold"))
            .Select(l => new Fold(l))
            .ToList();
    }

    public override string Part1()
    {
        LoadData();

        var firstFold = _folds.First();

        foreach(var coordinate in _coordinates)
        {
            coordinate.ApplyFold(firstFold);
        }

        var uniqueCoordinates = _coordinates.Select(c => c.ToString()).Distinct();

        return uniqueCoordinates.Count().ToString();
    }

    public override string Part2()
    {
        LoadData();

        foreach (var fold in _folds)
        {
            foreach (var coordinate in _coordinates)
            {
                coordinate.ApplyFold(fold);
            }
        }

        var maxX = _coordinates.Max(c => c.X);
        var maxY = _coordinates.Max(c => c.Y);
        var result = Environment.NewLine;
        for (var y =0; y <= maxY; y++)
        {
            for (var x = 0; x <= maxX; x++)
            {
                var isDot = _coordinates.Any(c => c.X == x && c.Y == y);
                var c = isDot ? '#' : ' ';
                result = $"{result}{c}";
            }

            result = $"{result}{Environment.NewLine}";
        }


        return result;
    }

    private class Coordinate
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Coordinate(string coordinate)
        {
            var split = coordinate.Split(',');
            X = int.Parse(split[0]);
            Y = int.Parse(split[1]);
        }

        public void ApplyFold(Fold fold)
        {
            switch(fold.FoldDirection)
            {
                case FoldDirection.Horizontal:
                    FoldHorizontal(fold.Location);
                    break;
                case FoldDirection.Vertical:
                    FoldVertical(fold.Location);
                    break;
            }
        }

        private void FoldHorizontal(int y)
        {
            if (Y > y)
            {
                Y = y - (Y - y);
            }
        }

        private void FoldVertical(int x)
        {
            if (X > x)
            {
                X = x - (X - x);
            }
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }
    }

    private class Fold
    {
        public FoldDirection FoldDirection { get; private set; }
        public int Location { get; private set; }

        public Fold(string fold)
        {
            var split = fold.Replace("fold along ", "")
                .Split('=');

            FoldDirection = split[0] == "x" ? FoldDirection.Vertical : FoldDirection.Horizontal;
            Location = int.Parse(split[1]);
        }
    }

    private enum FoldDirection
    {
        Horizontal,
        Vertical
    }
}
﻿using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2021.Day03;

public class Day03 : Day
{
    private List<BinaryString> _numbers;
    public Day03() : base(2021, 3, "Day03/input_2021_03.txt", "2035764", "2817661")
    {
    }

    public override void Initialise()
    {
        _numbers = InputLines
            .Select(l => new BinaryString(l))
            .ToList();
    }

    public override string Part1()
    {
        var totalNumbers = _numbers.Count;
        var threshold = totalNumbers / 2;
        var gammaRate = "";
        var epsilonRate = "";

        foreach (var index in Enumerable.Range(0, _numbers.First().Length))
        {
            var totalOnes = _numbers.Sum(n => n.GetBit(index));
            if (totalOnes > threshold)
            {
                gammaRate = $"{gammaRate}1";
                epsilonRate = $"{epsilonRate}0";
            }
            else
            {
                gammaRate = $"{gammaRate}0";
                epsilonRate = $"{epsilonRate}1";
            }
        }

        var gamma = new BinaryString(gammaRate);
        var epsilon = new BinaryString(epsilonRate);

        var powerConsumption = gamma.GetValue() * epsilon.GetValue();

        return powerConsumption.ToString();
    }

    public override string Part2()
    {
        var currentOxygenRatingList = _numbers;

        foreach (var index in Enumerable.Range(0, _numbers.First().Length))
        {
            var totalNumbers = currentOxygenRatingList.Count;
            var totalOnes = currentOxygenRatingList.Sum(n => n.GetBit(index));
            var totalZeros = totalNumbers - totalOnes;

            var mostCommonBitValue = totalOnes >= totalZeros ? 1 : 0;
            currentOxygenRatingList = currentOxygenRatingList
                .Where(n => n.GetBit(index) == mostCommonBitValue)
                .ToList();

            if (currentOxygenRatingList.Count == 1)
            {
                break;
            }
        }


        var currentScrubberList = _numbers;

        foreach (var index in Enumerable.Range(0, _numbers.First().Length))
        {
            var totalNumbers = currentScrubberList.Count;
            var totalOnes = currentScrubberList.Sum(n => n.GetBit(index));
            var totalZeros = totalNumbers - totalOnes;

            var leastCommonBitValue = totalOnes >= totalZeros ? 0 : 1;
            currentScrubberList = currentScrubberList
                .Where(n => n.GetBit(index) == leastCommonBitValue)
                .ToList();

            if (currentScrubberList.Count == 1)
            {
                break;
            }
        }


        var oxygenRating = currentOxygenRatingList.Single();
        var scrubberRating = currentScrubberList.Single();

        var lifeSupportRating = oxygenRating.GetValue() * scrubberRating.GetValue();

        return lifeSupportRating.ToString();
    }

    private class BinaryString
    {
        private readonly string _binaryString;

        public BinaryString(string binaryString)
        {
            _binaryString = binaryString;
        }

        public int Length => _binaryString.Length;
        public int GetBit(int index) => BitValue(_binaryString[index]);

        public int GetValue()
        {
            int value = 0;
            int exponent = 1;
            for(var i = _binaryString.Length - 1; i >= 0; i--)
            {
                int bitValue = BitValue(_binaryString[i]);
                value += bitValue * exponent;
                exponent *= 2;
            }

            return value;
        }

        private int BitValue(char c) => c == '0' ? 0 : 1;
    }
}
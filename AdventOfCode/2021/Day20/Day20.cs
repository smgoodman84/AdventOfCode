﻿using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2021.Day20;

public class Day20 : Day
{
    private int[] _algorithm;
    private int[][] _initialImage;

    public Day20() : base(2021, 20, "Day20/input_2021_20.txt", "5663", "19638")
    {
    }

    public override void Initialise()
    {
        _algorithm = InputLines.First().Select(ToBit).ToArray();

        _initialImage = InputLines
            .Skip(2)
            .Select(l => l.Select(ToBit).ToArray())
            .ToArray();
    }

    public override string Part1() => GetLitPixelsForEnhancements(2).ToString();
    public override string Part2() => GetLitPixelsForEnhancements(50).ToString();

    private int GetLitPixelsForEnhancements(int enhanceCount)
    {
        var image = new Image(_initialImage, 0);

        while (enhanceCount > 0)
        {
            image = image.Enhance(_algorithm);
            enhanceCount--;
        }

        var result = image.CountLitPixels();
        return result;
    }

    private int ToBit(char c)
    {
        return c == '.' ? 0 : 1;
    }

    private class Image
    {
        private readonly int[][] _pixels;
        private readonly int _infiniteSpaceValue;

        public Image(int[][] pixels, int infiniteSpaceValue)
        {
            _pixels = pixels;
            _infiniteSpaceValue = infiniteSpaceValue;
        }

        public int CountLitPixels()
        {
            return _pixels.Sum(line => line.Sum());
        }

        public Image Enhance(int[] algorithm)
        {
            var newHeight = _pixels.Length + 2;
            var newWidth = _pixels.First().Length + 2;
            var output = new int[newHeight][];
            for (var outputY = 0; outputY < newHeight; outputY++)
            {
                output[outputY] = new int[newWidth];
                for (var outputX = 0; outputX < newWidth; outputX++)
                {
                    var inputY = outputY - 1;
                    var inputX = outputX - 1;
                    output[outputY][outputX] = GetEnhancedPixel(algorithm, inputX, inputY);
                }
            }

            var newInfiniteSpaceValue = algorithm[0];
            if (_infiniteSpaceValue == 1)
            {
                newInfiniteSpaceValue = algorithm[511];
            }
            return new Image(output, newInfiniteSpaceValue);
        }

        private int GetEnhancedPixel(int[] algorithm, int x, int y)
        {
            var index = GetPixelIndex(x, y);
            var pixelValue = algorithm[index];
            return pixelValue;
        }

        private int GetPixelIndex(int x, int y)
        {
            var bits = new List<int>();
            foreach (var currentY in Enumerable.Range(y - 1, 3))
            {
                foreach (var currentX in Enumerable.Range(x - 1, 3))
                {
                    var bit = _infiniteSpaceValue;
                    if (0 <= currentY && currentY < _pixels.Length
                                      && 0 <= currentX && currentX < _pixels[currentY].Length)
                    {
                        bit = _pixels[currentY][currentX];
                    }
                    bits.Add(bit);
                }
            }

            var number = 0;
            foreach (var bit in bits)
            {
                number *= 2;
                number += bit;
            }
            return number;
        }

        public override string ToString()
        {
            var result = string.Empty;
            for (var y = 0; y < _pixels.Length; y++)
            {
                for (var x = 0; x < _pixels[y].Length; x++)
                {
                    if (_pixels[y][x] == 1)
                    {
                        result += '#';
                    }
                    else
                    {
                        result += '.';
                    }
                }
                result += Environment.NewLine;
            }

            return result;
        }
    }
}
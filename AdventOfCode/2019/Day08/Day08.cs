using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode.Shared;

namespace AdventOfCode._2019.Day08;

public class Day08 : Day
{
    private const string part2Result = "\n#  # ###  #  # #### ###  \n#  # #  # #  # #    #  # \n#  # ###  #  # ###  #  # \n#  # #  # #  # #    ###  \n#  # #  # #  # #    #    \n ##  ###   ##  #    #    \n";

    public Day08() : base(2019, 8, "Day08/input_2019_08.txt", "1677", part2Result)
    {

    }

    private const int Transparent = 2;

    private int[] _pixels;
    private int _width;
    private int _height;
    private List<SpaceImageLayer> _layers;

    public override void Initialise()
    {
        var pixels = string.Join("", InputLines)
            .ToCharArray()
            .Select(c => int.Parse(c.ToString()))
            .ToArray();

        _pixels = pixels;
        _width = 25;
        _height = 6;

        var layerLength = _width * _height;
        var layers = pixels.Length / layerLength;

        var layer = 0;
        _layers = new List<SpaceImageLayer>(layers);
        while (layer < layers)
        {
            var layerPixels = _pixels
                .Skip(layer * layerLength)
                .Take(layerLength)
                .ToArray();

            _layers.Add(new SpaceImageLayer(layerPixels, _width, _height, layer));

            layer += 1;
        }
    }

    public override string Part1()
    {
        return FindLayerWithFewestZerosAndGetNumberOfOnesTimeNumberOfTwos().ToString();
    }

    public override string Part2()
    {
        var image = RenderImage();
        var result = new StringBuilder();

        result.Append('\n');
        var y = 0;
        while (y < image.GetLength(1))
        {
            var x = 0;
            while (x < image.GetLength(0))
            {
                var pixel = image[x, y];
                result.Append(pixel == 0 ? ' ' : '#');
                x += 1;
            }
            result.Append('\n');
            y += 1;
        }

        return result.ToString();
    }

    public int[,] RenderImage()
    {
        var result = new int[_width, _height];
        foreach (var y in Enumerable.Range(0, _height))
        {
            foreach (var x in Enumerable.Range(0, _width))
            {
                foreach (var layer in _layers)
                {
                    var pixel = layer.GetPixel(x, y);
                    if (pixel != Transparent)
                    {
                        result[x, y] = pixel;
                        // Console.Write(pixel == 0 ? ' ' : '#');
                        break;
                    }
                }
            }

            // Console.WriteLine();
        }

        return result;
    }


    public int FindLayerWithFewestZerosAndGetNumberOfOnesTimeNumberOfTwos()
    {
        var layer = _layers
            .Select(l => (Layer: l, PixelsMatching: l.CountPixelsMatching(0)))
            .OrderBy(x => x.PixelsMatching)
            .Select(x => x.Layer)
            .First();

        var ones = layer.CountPixelsMatching(1);
        var twos = layer.CountPixelsMatching(2);

        return ones * twos;
    }

    private class SpaceImageLayer
    {
        private readonly int[] _pixels;
        private readonly int _width;
        private readonly int _height;

        public readonly int LayerNumber;

        public SpaceImageLayer(int[] pixels, int width, int height, int layerNumber)
        {
            _pixels = pixels;
            _width = width;
            _height = height;
            LayerNumber = layerNumber;
        }

        public int CountPixelsMatching(int pixelValue)
        {
            return _pixels.Count(p => p == pixelValue);
        }

        public int GetPixel(int x, int y)
        {
            return _pixels[y * _width + x];
        }
    }
}
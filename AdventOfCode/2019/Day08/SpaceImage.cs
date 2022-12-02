using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2019.Day08
{
    public class SpaceImage
    {
        private const int Transparent = 2;

        private readonly int[] _pixels;
        private readonly int _width;
        private readonly int _height;
        private readonly List<SpaceImageLayer> _layers;

        public static SpaceImage LoadFromFile(string filename, int width, int height)
        {
            var pixels = File.ReadAllText(filename)
                .ToCharArray()
                .Select(c => int.Parse(c.ToString()))
                .ToArray();

            return new SpaceImage(pixels, width, height);
        }

        public SpaceImage(int[] pixels, int width, int height)
        {
            _pixels = pixels;
            _width = width;
            _height = height;

            var layerLength = width * height;
            var layers = pixels.Length / layerLength;

            var layer = 0;
            _layers = new List<SpaceImageLayer>(layers);
            while (layer <= layers)
            {
                var layerPixels = _pixels
                    .Skip(layer * layerLength)
                    .Take(layerLength)
                    .ToArray();

                _layers.Add(new SpaceImageLayer(layerPixels, width, height, layer));

                layer += 1;
            }
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
                            Console.Write(pixel == 0 ? ' ' : '#');
                            break;
                        }
                    }
                }

                Console.WriteLine();
            }

            return result;
        }


        public int FindLayerWithFewestZerosAndGetNumberOfOnesTimeNumberOfTwos()
        {
            var layer = _layers
                .Select(l => (l, l.CountPixelsMatching(0)))
                .OrderBy(x => x.Item2)
                .Select(x => x.Item1)
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
}

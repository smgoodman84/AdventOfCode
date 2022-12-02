using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;

namespace AdventOfCode2019.Day18
{
    public class PathFinder
    {
        public static PathFinder LoadFromFile(string filename)
        {
            var vault = File.ReadAllLines(filename)
                .Select(l => l.ToCharArray())
                .ToArray();

            return new PathFinder(vault);
        }

        private readonly char[][] _vault;

        public PathFinder(char[][] vault)
        {
            _vault = vault;
        }

        public int GetShortestPath()
        {
            return 0;
        }

        private class Context
        {
            List<char> keys = new List<char>();
            Point location;
            Point previousLocation;
        }
    }
}

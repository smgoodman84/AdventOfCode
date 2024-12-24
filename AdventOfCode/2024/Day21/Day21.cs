using System.ComponentModel;
using AdventOfCode.Shared;
using AdventOfCode.Shared.DataStructures;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2024.Day21;

public class Day21 : Day
{
    public Day21() : base(2024, 21, "Day21/input_2024_21.txt", "", "", true)
    {
    }


    KeyPad _numPad = new KeyPad("789|456|123| 0A");
    KeyPad _directionalPad = new KeyPad(" ^A|<v>");

    public override void Initialise()
    {
    }

    public override string Part1()
    {
        var complexity = InputLines.Sum(GetComplexity);
        return complexity.ToString();
    }

    private int GetComplexity(string code)
    {
        var codeIntPart = int.Parse(code.Replace("A", ""));

        var shortestPath = 0;
        var previousKey = 'A';
        foreach (var key in code)
        {
            shortestPath += GetStepShortestPath(previousKey, key);
            previousKey = key;
        }

        var complexity = codeIntPart * shortestPath;

        return complexity;
    }

    private int GetStepShortestPath(char start, char key)
    {
        var shortestPath = int.MaxValue;

        var paths = _numPad.GetPathsString(start, key);
        foreach (var path in paths)
        {
            // TraceLine(path);

            var robotOnePaths = _directionalPad.GetPaths('A', path);
            foreach (var robotOnePath in robotOnePaths)
            {
                // TraceLine($" - {robotOnePath}");

                var robotTwoPaths = _directionalPad.GetPaths('A', robotOnePath);
                foreach (var robotTwoPath in robotTwoPaths)
                {
                    // TraceLine($" - - {robotTwoPath}");

                    var thisShortestPath = _directionalPad.GetShortesPathLength('A', robotTwoPath);
                    // foreach (var myPath in myPaths)
                    {
                        // TraceLine($" - - - {myPath}");
                        if (thisShortestPath < shortestPath)
                        {
                            shortestPath = thisShortestPath;
                        }
                    }
                }
            }
        }

        return shortestPath;
    }

    public override string Part2()
    {
        return string.Empty;
    }

    private class KeyPad
    {
        private Dictionary<char, Coordinate2D> _keyToLocations;
        private Dictionary<Coordinate2D, char> _locationToKey;
        public KeyPad(string keys)
        {
            var rows = keys.Split('|');
            
            _keyToLocations = rows
                .SelectMany((r, y) => 
                    r.Select((c, x) => (c, x, y))
                )
                .Where(e => e.c != ' ')
                .ToDictionary(
                    e => e.c,
                    e => new Coordinate2D(e.x, e.y, CoordinateSystem.Screen));
            
            _locationToKey = _keyToLocations
                .ToDictionary(
                    kvp => kvp.Value,
                    kvp => kvp.Key);
        }

        private Dictionary2D<char, string, List<string>> _fullPathCache = new();
        public List<string> GetPaths(char start, string keysToPress)
        {
            if (_fullPathCache.TryGetValue(start, keysToPress, out var cachedResult))
            {
                return cachedResult;
            }

            var result = GetPathsUncached(start, keysToPress);
            _fullPathCache.Add(start, keysToPress, result);

            return result;
        }

        public List<string> GetPathsUncached(char start, string keysToPress)
        {
            if (keysToPress.Length == 1)
            {
                return GetPaths(start, keysToPress[0])
                    .Select(MapKeyPressPath)
                    .ToList();
            }

            var middle = keysToPress.Length / 2;
            var left = keysToPress.Substring(0, middle);
            var right = keysToPress.Substring(middle);

            var leftPaths = GetPaths(start, left);
            var rightPaths = GetPaths(left.Last(), right);

            var routes = new List<string>();
            foreach (var leftPath in leftPaths)
            {
                foreach (var rightPath in rightPaths)
                {
                    routes.Add($"{leftPath}{rightPath}");
                }
            }

            return routes;
        }


        public int GetShortesPathLength(char start, string keysToPress)
        {
            if (keysToPress.Length == 1)
            {
                return GetPaths(start, keysToPress[0])
                    .Select(MapKeyPressPath)
                    .Min(p => p.Length);
            }

            var middle = keysToPress.Length / 2;
            var left = keysToPress.Substring(0, middle);
            var right = keysToPress.Substring(middle);

            var leftPaths = GetPaths(start, left);
            var rightPaths = GetPaths(left.Last(), right);

            return leftPaths.Min(x => x.Length) + rightPaths.Min(x => x.Length);
        }

        public List<string> GetPathsUncachedOriginal(char start, string keysToPress)
        {
            var routes = new List<string>()
            {
                string.Empty
            };

            var current = start;
            foreach (var key in keysToPress)
            {
                var nextStepPaths = GetPaths(current, key)
                    .Select(MapKeyPressPath)
                    .ToList();

                routes = routes.SelectMany(r =>
                    nextStepPaths.Select(nsp => $"{r}{nsp}")
                )
                .ToList();

                current = key;
            }

            return routes;
        }

        private string MapKeyPressPath(List<Direction> path)
        {
            var mappedPath = path.Select(MapDirection).Concat(['A']);
            var result = string.Join("", mappedPath);
            return result;
        }

        private char MapDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up: return '^';
                case Direction.Down: return 'v';
                case Direction.Left: return '<';
                case Direction.Right: return '>';
            }

            throw new Exception($"Unknown direction {direction}");
        }

        private Dictionary2D<char, char, List<string>> _pathStringCache = new();
        public List<string> GetPathsString(char from, char to)
        {
            if (_pathStringCache.TryGetValue(from, to, out var cachedResult))
            {
                return cachedResult;
            }

            var result = GetPathsUncached(from, to)
                    .Select(MapKeyPressPath)
                    .ToList();

            _pathStringCache.Add(from, to, result);

            return result;
        }

        private Dictionary2D<char, char, List<List<Direction>>> _pathCache = new();
        public List<List<Direction>> GetPaths(char from, char to)
        {
            if (_pathCache.TryGetValue(from, to, out var cachedResult))
            {
                return cachedResult;
            }

            var result = GetPathsUncached(from, to);
            _pathCache.Add(from, to, result);
            return result;
        }

        private List<List<Direction>> GetPathsUncached(char from, char to)
        {
            if (from == to)
            {
                return new List<List<Direction>>
                {
                    new List<Direction>()
                };
            }

            var fromLocation = _keyToLocations[from];
            var toLocation = _keyToLocations[to];

            var routes = new List<List<Direction>>();

            if (fromLocation.X < toLocation.X)
            {
                var left = toLocation.Left();
                if (_locationToKey.TryGetValue(left, out var keyToLeft))
                {
                    var pathsToLeft = GetPaths(from, keyToLeft);
                    foreach(var pathToLeft in pathsToLeft)
                    {
                        var newPath = pathToLeft.Concat([Direction.Right]).ToList();
                        routes.Add(newPath);
                    }
                }
            }

            if (fromLocation.X > toLocation.X)
            {
                var right = toLocation.Right();
                if (_locationToKey.TryGetValue(right, out var keyToRight))
                {
                    var pathsToRight = GetPaths(from, keyToRight);
                    foreach(var pathToRight in pathsToRight)
                    {
                        var newPath = pathToRight.Concat([Direction.Left]).ToList();
                        routes.Add(newPath);
                    }
                }
            }

            if (fromLocation.Y < toLocation.Y)
            {
                var up = toLocation.Up();
                if (_locationToKey.TryGetValue(up, out var keyToUp))
                {
                    var pathsToUp = GetPaths(from, keyToUp);
                    foreach(var pathToUp in pathsToUp)
                    {
                        var newPath = pathToUp.Concat([Direction.Down]).ToList();
                        routes.Add(newPath);
                    }
                }
            }

            if (fromLocation.Y > toLocation.Y)
            {
                var down = toLocation.Down();
                if (_locationToKey.TryGetValue(down, out var keyToDown))
                {
                    var pathsToDown = GetPaths(from, keyToDown);
                    foreach(var pathToDown in pathsToDown)
                    {
                        var newPath = pathToDown.Concat([Direction.Up]).ToList();
                        routes.Add(newPath);
                    }
                }
            }

            return routes;
        }
    }
}
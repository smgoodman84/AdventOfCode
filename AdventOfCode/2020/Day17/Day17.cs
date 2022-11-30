using System.Collections.Generic;
using System.IO;
using AdventOfCode.Shared;

namespace AdventOfCode._2020.Day17
{
    public class Day17 : Day
    {
        public Day17() : base(2020, 17, "Day17/input_2020_17.txt", "346", "1632")
        {

        }

        private const char Active = '#';

        private Space _space;
        private Space4D _space4D;

        public override void Initialise()
        {
            _space = new Space(0);
            _space4D = new Space4D(0);

            var y = 0;
            foreach (var line in InputLines)
            {
                var x = 0;
                foreach (var c in line.ToCharArray())
                {
                    _space.SetLocation(x, y, 0, c == Active);
                    _space4D.SetLocation(x, y, 0, 0, c == Active);
                    x += 1;
                }
                y += 1;
            }
        }

        public override string Part1() => GetGeneration(6).CountActive().ToString();

        public override string Part2() => GetGeneration4D(6).CountActive().ToString();

        private Space GetGeneration(int generation)
        {
            var space = _space;
            while (space.Generation < generation)
            {
                space = space.GetNextGeneration();
            }
            return space;
        }

        private class Space
        {
            public int Generation { get; private set; }
            private int _minX = 0;
            private int _minY = 0;
            private int _minZ = 0;
            private int _maxX = 0;
            private int _maxY = 0;
            private int _maxZ = 0;

            private Dictionary<int, Dictionary<int, Dictionary<int, bool>>> _space;
            public Space(int generation)
            {
                Generation = generation;
                _space = new Dictionary<int, Dictionary<int, Dictionary<int, bool>>>();
            }

            public void SetLocation(int x, int y, int z, bool active)
            {
                if (!_space.ContainsKey(x))
                {
                    _space.Add(x, new Dictionary<int, Dictionary<int, bool>>());
                }

                if (!_space[x].ContainsKey(y))
                {
                    _space[x].Add(y, new Dictionary<int, bool>());
                }

                _space[x][y][z] = active;

                if (active)
                {
                    if (x < _minX)
                    {
                        _minX = x;
                    }

                    if (x > _maxX)
                    {
                        _maxX = x;
                    }

                    if (y < _minY)
                    {
                        _minY = y;
                    }

                    if (y > _maxY)
                    {
                        _maxY = y;
                    }

                    if (z < _minZ)
                    {
                        _minZ = z;
                    }

                    if (z > _maxZ)
                    {
                        _maxZ = z;
                    }
                }
            }

            public bool GetLocation(int x, int y, int z)
            {
                if (!_space.ContainsKey(x))
                {
                    return false;
                }

                if (!_space[x].ContainsKey(y))
                {
                    return false;
                }

                if (!_space[x][y].ContainsKey(z))
                {
                    return false;
                }

                return _space[x][y][z];
            }

            public Space GetNextGeneration()
            {
                var nextGeneration = new Space(Generation + 1);
                for (var x = _minX - 1; x <= _maxX + 1; x++)
                {
                    for (var y = _minY - 1; y <= _maxY + 1; y++)
                    {
                        for (var z = _minZ - 1; z <= _maxZ + 1; z++)
                        {
                            var active = GetLocation(x, y, z);
                            var count = CountNeighbours(x, y, z);
                            var newState = GetNewState(active, count);
                            nextGeneration.SetLocation(x, y, z, newState);
                        }
                    }
                }
                return nextGeneration;
            }

            private int CountNeighbours(int x, int y, int z)
            {
                var count = 0;
                for (var currentX = x - 1; currentX <= x + 1; currentX++)
                {
                    for (var currentY = y - 1; currentY <= y + 1; currentY++)
                    {
                        for (var currentZ = z - 1; currentZ <= z + 1; currentZ++)
                        {
                            if (!(currentX == x && currentY == y && currentZ == z))
                            {
                                if (GetLocation(currentX, currentY, currentZ))
                                {
                                    count += 1;
                                }
                            }
                        }
                    }
                }
                return count;
            }

            private bool GetNewState(bool active, int count)
            {
                if (active)
                {
                    return count == 2 || count == 3;
                }

                return count == 3;
            }

            public int CountActive()
            {
                var count = 0;
                foreach (var x in _space.Keys)
                {
                    foreach (var y in _space[x].Keys)
                    {
                        foreach (var z in _space[x][y].Keys)
                        {
                            if (_space[x][y][z])
                            {
                                count += 1;
                            }
                        }
                    }
                }
                return count;
            }
        }

        private Space4D GetGeneration4D(int generation)
        {
            var space = _space4D;
            while (space.Generation < generation)
            {
                space = space.GetNextGeneration();
            }
            return space;
        }

        private class Space4D
        {
            public int Generation { get; private set; }
            private int _minX = 0;
            private int _minY = 0;
            private int _minZ = 0;
            private int _minW = 0;
            private int _maxX = 0;
            private int _maxY = 0;
            private int _maxZ = 0;
            private int _maxW = 0;

            private Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, bool>>>> _space;
            public Space4D(int generation)
            {
                Generation = generation;
                _space = new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, bool>>>>();
            }

            public void SetLocation(int x, int y, int z, int w, bool active)
            {
                if (!_space.ContainsKey(x))
                {
                    _space.Add(x, new Dictionary<int, Dictionary<int, Dictionary<int, bool>>>());
                }

                if (!_space[x].ContainsKey(y))
                {
                    _space[x].Add(y, new Dictionary<int, Dictionary<int, bool>>());
                }

                if (!_space[x][y].ContainsKey(z))
                {
                    _space[x][y].Add(z, new Dictionary<int, bool>());
                }

                _space[x][y][z][w] = active;

                if (active)
                {
                    if (x < _minX)
                    {
                        _minX = x;
                    }

                    if (x > _maxX)
                    {
                        _maxX = x;
                    }

                    if (y < _minY)
                    {
                        _minY = y;
                    }

                    if (y > _maxY)
                    {
                        _maxY = y;
                    }

                    if (z < _minZ)
                    {
                        _minZ = z;
                    }

                    if (z > _maxZ)
                    {
                        _maxZ = z;
                    }

                    if (w < _minW)
                    {
                        _minW = w;
                    }

                    if (w > _maxW)
                    {
                        _maxW = w;
                    }
                }
            }

            public bool GetLocation(int x, int y, int z, int w)
            {
                if (!_space.ContainsKey(x))
                {
                    return false;
                }

                if (!_space[x].ContainsKey(y))
                {
                    return false;
                }

                if (!_space[x][y].ContainsKey(z))
                {
                    return false;
                }

                if (!_space[x][y][z].ContainsKey(w))
                {
                    return false;
                }

                return _space[x][y][z][w];
            }

            public Space4D GetNextGeneration()
            {
                var nextGeneration = new Space4D(Generation + 1);
                for (var x = _minX - 1; x <= _maxX + 1; x++)
                {
                    for (var y = _minY - 1; y <= _maxY + 1; y++)
                    {
                        for (var z = _minZ - 1; z <= _maxZ + 1; z++)
                        {
                            for (var w = _minW - 1; w <= _maxW + 1; w++)
                            {
                                var active = GetLocation(x, y, z, w);
                                var count = CountNeighbours(x, y, z, w);
                                var newState = GetNewState(active, count);
                                nextGeneration.SetLocation(x, y, z, w, newState);
                            }
                        }
                    }
                }
                return nextGeneration;
            }

            private int CountNeighbours(int x, int y, int z, int w)
            {
                var count = 0;
                for (var currentX = x - 1; currentX <= x + 1; currentX++)
                {
                    for (var currentY = y - 1; currentY <= y + 1; currentY++)
                    {
                        for (var currentZ = z - 1; currentZ <= z + 1; currentZ++)
                        {
                            for (var currentW = w - 1; currentW <= w + 1; currentW++)
                            {
                                if (!(currentX == x && currentY == y && currentZ == z && currentW == w))
                                {
                                    if (GetLocation(currentX, currentY, currentZ, currentW))
                                    {
                                        count += 1;
                                    }
                                }
                            }
                        }
                    }
                }
                return count;
            }

            private bool GetNewState(bool active, int count)
            {
                if (active)
                {
                    return count == 2 || count == 3;
                }

                return count == 3;
            }

            public int CountActive()
            {
                var count = 0;
                foreach (var x in _space.Keys)
                {
                    foreach (var y in _space[x].Keys)
                    {
                        foreach (var z in _space[x][y].Keys)
                        {
                            foreach (var w in _space[x][y][z].Keys)
                            {
                                if (_space[x][y][z][w])
                                {
                                    count += 1;
                                }
                            }
                        }
                    }
                }
                return count;
            }
        }
    }
}

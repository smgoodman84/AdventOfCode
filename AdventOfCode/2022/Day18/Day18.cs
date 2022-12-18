using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2022.Day18
{
    public class Day18 : Day
    {
        public Day18() : base(2022, 18, "Day18/input_2022_18.txt", "4580", "2610")
        {

        }

        List<Coordinate3D> _lava;
        Grid3D<State> _map;
        public override void Initialise()
        {
            _lava = InputLines
                .Select(l => new Coordinate3D(l))
                .ToList();

            var minX = _lava.Min(l => l.X);
            var maxX = _lava.Max(l => l.X);
            var minY = _lava.Min(l => l.Y);
            var maxY = _lava.Max(l => l.Y);
            var minZ = _lava.Min(l => l.Z);
            var maxZ = _lava.Max(l => l.Z);

            _map = new Grid3D<State>((int)minX, (int)minY, (int)minZ, (int)maxX, (int)maxY, (int)maxZ);

            foreach(var lava in _lava)
            {
                _map.Write(lava, State.Lava);
            }
        }

        public override string Part1()
        {
            var surfaceArea = 0;
            foreach (var lava in _lava)
            {
                if (!IsLava((int)lava.X - 1, (int)lava.Y, (int)lava.Z))
                {
                    surfaceArea += 1;
                }
                if (!IsLava((int)lava.X + 1, (int)lava.Y, (int)lava.Z))
                {
                    surfaceArea += 1;
                }
                if (!IsLava((int)lava.X, (int)lava.Y - 1, (int)lava.Z))
                {
                    surfaceArea += 1;
                }
                if (!IsLava((int)lava.X, (int)lava.Y + 1, (int)lava.Z))
                {
                    surfaceArea += 1;
                }
                if (!IsLava((int)lava.X, (int)lava.Y, (int)lava.Z - 1))
                {
                    surfaceArea += 1;
                }
                if (!IsLava((int)lava.X, (int)lava.Y, (int)lava.Z + 1))
                {
                    surfaceArea += 1;
                }
            }

            return surfaceArea.ToString();
        }

        private bool IsLava(int x, int y, int z)
        {
            if (!_map.IsInGrid(x, y, z))
            {
                return false;
            }

            return _map.Read(x, y, z) == State.Lava;
        }

        private bool IsSteam(int x, int y, int z)
        {
            if (!_map.IsInGrid(x, y, z))
            {
                return true;
            }

            return _map.Read(x, y, z) == State.Steam;
        }


        private enum State
        {
            Air,
            Lava,
            Steam
        }

        public override string Part2()
        {
            foreach (var x in _map.AllX())
            {
                foreach (var y in _map.AllY())
                {
                    if (_map.Read(x, y, _map.MinZ) == State.Air)
                    {
                        Fill(x, y, _map.MinZ);
                    }

                    if (_map.Read(x, y, _map.MaxZ) == State.Air)
                    {
                        Fill(x, y, _map.MaxZ);
                    }
                }
            }

            foreach (var x in _map.AllX())
            {
                foreach (var z in _map.AllZ())
                {
                    if (_map.Read(x, _map.MinY, z) == State.Air)
                    {
                        Fill(x, _map.MinY, z);
                    }

                    if (_map.Read(x, _map.MaxY, z) == State.Air)
                    {
                        Fill(x, _map.MaxY, z);
                    }
                }
            }

            foreach (var y in _map.AllY())
            {
                foreach (var z in _map.AllZ())
                {
                    if (_map.Read(_map.MinX, y, z) == State.Air)
                    {
                        Fill(_map.MinX, y, z);
                    }

                    if (_map.Read(_map.MaxX, y, z) == State.Air)
                    {
                        Fill(_map.MaxX, y, z);
                    }
                }
            }

            /*
            while (true)
            {
                Console.Clear();
                Task.Delay(1000).ConfigureAwait(false).GetAwaiter().GetResult();
                foreach (var z in _map.AllZ())
                {
                    Task.Delay(1000).ConfigureAwait(false).GetAwaiter().GetResult();
                    Console.Clear();
                    foreach (var y in _map.AllY())
                    {
                        foreach (var x in _map.AllX())
                        {
                            var value = _map.Read(x, y, z);
                            switch (value)
                            {
                                case State.Air:
                                    Console.Write('@');
                                    break;
                                case State.Steam:
                                    Console.Write('~');
                                    break;
                                case State.Lava:
                                    Console.Write('#');
                                    break;
                            }
                        }
                        Console.WriteLine();
                    }
                }
            }
            */
            var surfaceArea = 0;
            foreach (var lava in _lava)
            {
                if (IsSteam((int)lava.X - 1, (int)lava.Y, (int)lava.Z))
                {
                    surfaceArea += 1;
                }
                if (IsSteam((int)lava.X + 1, (int)lava.Y, (int)lava.Z))
                {
                    surfaceArea += 1;
                }
                if (IsSteam((int)lava.X, (int)lava.Y - 1, (int)lava.Z))
                {
                    surfaceArea += 1;
                }
                if (IsSteam((int)lava.X, (int)lava.Y + 1, (int)lava.Z))
                {
                    surfaceArea += 1;
                }
                if (IsSteam((int)lava.X, (int)lava.Y, (int)lava.Z - 1))
                {
                    surfaceArea += 1;
                }
                if (IsSteam((int)lava.X, (int)lava.Y, (int)lava.Z + 1))
                {
                    surfaceArea += 1;
                }
            }

            return surfaceArea.ToString();
        }

        private void Fill(int x, int y, int z)
        {
            if (!_map.IsInGrid(x, y, z))
            {
                return;
            }

            var currentState = _map.Read(x, y, z);
            if (currentState == State.Air)
            {
                _map.Write(x, y, z, State.Steam);
                Fill(x - 1, y, z);
                Fill(x + 1, y, z);
                Fill(x, y - 1, z);
                Fill(x, y + 1, z);
                Fill(x, y, z - 1);
                Fill(x, y, z + 1);
            }
        }
    }
}

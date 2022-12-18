using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2022.Day18
{
    public class Day18 : Day
    {
        public Day18() : base(2022, 18, "Day18/input_2022_18.txt", "4580", "")
        {

        }

        List<Coordinate3D> _lava;
        Grid3D<bool> _map;
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

            _map = new Grid3D<bool>((int)minX, (int)minY, (int)minZ, (int)maxX, (int)maxY, (int)maxZ);

            foreach(var lava in _lava)
            {
                _map.Write(lava, true);
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

            return _map.Read(x, y, z);
        }

        public override string Part2()
        {
            return "";
        }
    }
}

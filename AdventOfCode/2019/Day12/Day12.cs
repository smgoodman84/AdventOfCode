using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2019.Day12
{
    public class Day12 : Day
    {
        public Day12() : base(2019, 12, "Day12/input_2019_12.txt", "7013", "324618307124784")
        {

        }

        public override string Part1()
        {
            return Simulate(1000).GetTotalEnergy().ToString();
        }

        public override string Part2()
        {
            return FindCycle().ToString();
        }

        private IEnumerable<Moon> _moons;
        public override void Initialise()
        {
            _moons = InputLines
                .Select((line, lineNumber) => new Moon(line, lineNumber))
                .ToList();
        }

        private string GetPositions(Func<Coordinate3D, long> pointAccessor)
        {
            var allPositions = _moons
                .Select(m => pointAccessor(m.Position))
                .Concat(_moons
                .Select(m => pointAccessor(m.Velocity)));

            var result = string.Join(",", allPositions);

            return result;
        }

        public long FindCycle()
        {
            var xDictionary = new Dictionary<string, int>();
            var yDictionary = new Dictionary<string, int>();
            var zDictionary = new Dictionary<string, int>();

            int? xLoopStart = null;
            int? yLoopStart = null;
            int? zLoopStart = null;

            int? xLoopRepeat = null;
            int? yLoopRepeat = null;
            int? zLoopRepeat = null;

            var time = 0;
            while (xLoopStart == null || yLoopStart == null || zLoopStart == null)
            {
                var xKey = GetPositions(p => p.X);
                var yKey = GetPositions(p => p.Y);
                var zKey = GetPositions(p => p.Z);

                if (xLoopStart == null)
                {
                    if (xDictionary.ContainsKey(xKey))
                    {
                        xLoopStart = xDictionary[xKey];
                        xLoopRepeat = time;
                    }
                    else
                    {
                        xDictionary.Add(xKey, time);
                    }
                }

                if (yLoopStart == null)
                {
                    if (yDictionary.ContainsKey(yKey))
                    {
                        yLoopStart = yDictionary[yKey];
                        yLoopRepeat = time;
                    }
                    else
                    {
                        yDictionary.Add(yKey, time);
                    }
                }

                if (zLoopStart == null)
                {
                    if (zDictionary.ContainsKey(zKey))
                    {
                        zLoopStart = zDictionary[zKey];
                        zLoopRepeat = time;
                    }
                    else
                    {
                        zDictionary.Add(zKey, time);
                    }
                }

                time += 1;
                AdvanceSimulation();
            }

            long loopStart = new int[]
            {
                xLoopStart.Value,
                yLoopStart.Value,
                zLoopStart.Value
            }
            .Max();

            long xLoopLength = xLoopRepeat.Value - xLoopStart.Value;
            long yLoopLength = yLoopRepeat.Value - yLoopStart.Value;
            long zLoopLength = zLoopRepeat.Value - zLoopStart.Value;

            /*
            Console.WriteLine($"xLoopStart: {xLoopStart}");
            Console.WriteLine($"yLoopStart: {yLoopStart}");
            Console.WriteLine($"zLoopStart: {zLoopStart}");
            Console.WriteLine($"loopStart: {loopStart}");
            Console.WriteLine($"xLoopLength: {xLoopLength}");
            Console.WriteLine($"yLoopLength: {yLoopLength}");
            Console.WriteLine($"zLoopLength: {zLoopLength}");

            Console.WriteLine($"Multiply: {xLoopLength * yLoopLength * zLoopLength}");
            */
            long loopLength = LowestCommonMultiple(xLoopLength, yLoopLength, zLoopLength);
            //Console.WriteLine($"loopLength: {loopLength}");

            // 0                   1                   2                   3                   4                   5                   6                   7       
            // 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9

            // 1 2 3 4 5 3 4 5 3 4 5 3 4 5 3 4 5 3 4 5 3 4 5 3 4 5 3 4 5 3 4 5 3 4 5 3 4 5 3 4 5 3 4 5 3 4 5 3 4 5 3 4 5 3 4 5 3 4 5 3 4 5 3 4 5 3 4 5 3 4 5 3 4 5 3 4 5 3 4 5
            // 5 6 7 8 9 6 7 8 9 6 7 8 9 6 7 8 9 6 7 8 9 6 7 8 9 6 7 8 9 6 7 8 9 6 7 8 9 6 7 8 9 6 7 8 9 6 7 8 9 6 7 8 9 6 7 8 9 6 7 8 9 6 7 8 9 6 7 8 9 6 7 8 9 6 7 8 9 6 7 8
            // A B C D E A B C D E A B C D E A B C D E A B C D E A B C D E A B C D E A B C D E A B C D E A B C D E A B C D E A B C D E A B C D E A B C D E A B C D E A B C D E

            // 2, 3
            // 1, 4
            // 0, 5
            // 2 + 60 = 62

            return loopStart + loopLength;
        }

        public long LowestCommonMultiple(params long[] inputs)
        {
            if (inputs.Length == 2)
            {
                var a = inputs[0];
                var b = inputs[1];
                var result = a * b / GreatestCommonFactor(a, b); ;
                //Console.WriteLine($"LCM: {a}, {b} = {result}");
                return result;
            }
            else
            {
                var first = inputs.First();
                var rest = inputs.Skip(1).ToArray();
                return LowestCommonMultiple(first, LowestCommonMultiple(rest));
            }
        }

        public long GreatestCommonFactor(long a, long b)
        {
            //Console.Write($"GCF: {a}, {b}");

            while (a != b)
            {
                if (a > b)
                {
                    a = a - b;
                }
                else
                {
                    b = b - a;
                }
            }

            //Console.WriteLine($" = {a}");
            return a;
            /*
            if (a == b)
            {
                return a;
            }

            if (a > b)
            {
                return GreatestCommonFactor(a - b, b);
            }
            else
            {
                return GreatestCommonFactor(a, b - a);
            }*/
        }

        public Day12 Simulate(int iterations)
        {
            foreach (var iteration in Enumerable.Range(1, iterations))
            {
                AdvanceSimulation();
            }
            return this;
        }

        private void AdvanceSimulation()
        {
            ApplyGravity();
            ApplyVelocity();
        }

        public long GetTotalEnergy() => _moons.Sum(m => m.TotalEnergy);

        private void ApplyVelocity()
        {
            foreach (var moon in _moons)
            {
                moon.ApplyVelocity();
            }
        }

        private void ApplyGravity()
        {
            foreach (var moon1 in _moons)
            {
                foreach (var moon2 in _moons.Where(m => m.Id > moon1.Id))
                {
                    if (moon1.Position.X > moon2.Position.X)
                    {
                        moon1.Velocity = moon1.Velocity.AddX(-1);
                        moon2.Velocity = moon2.Velocity.AddX(1);
                    }
                    if (moon1.Position.X < moon2.Position.X)
                    {
                        moon1.Velocity = moon1.Velocity.AddX(1);
                        moon2.Velocity = moon2.Velocity.AddX(-1);
                    }

                    if (moon1.Position.Y > moon2.Position.Y)
                    {
                        moon1.Velocity = moon1.Velocity.AddY(-1);
                        moon2.Velocity = moon2.Velocity.AddY(1);
                    }
                    if (moon1.Position.Y < moon2.Position.Y)
                    {
                        moon1.Velocity = moon1.Velocity.AddY(1);
                        moon2.Velocity = moon2.Velocity.AddY(-1);
                    }

                    if (moon1.Position.Z > moon2.Position.Z)
                    {
                        moon1.Velocity = moon1.Velocity.AddZ(-1);
                        moon2.Velocity = moon2.Velocity.AddZ(1);
                    }
                    if (moon1.Position.Z < moon2.Position.Z)
                    {
                        moon1.Velocity = moon1.Velocity.AddZ(1);
                        moon2.Velocity = moon2.Velocity.AddZ(-1);
                    }
                }
            }
        }

        private class Moon
        {
            public Coordinate3D Position { get; set; } = Coordinate3D.Origin;

            public Coordinate3D Velocity { get; set; } = Coordinate3D.Origin;

            public int Id { get; set; }

            public Moon(string input, int id)
            {
                Id = id;

                var trimmed = input.Replace("<", "").Replace(">", "");

                long x = 0;
                long y = 0;
                long z = 0;
                foreach (var coordinate in trimmed.Split(","))
                {
                    var assignment = coordinate.Trim().Split("=").ToArray();
                    var value = long.Parse(assignment[1]);
                    switch (assignment[0].ToLower())
                    {
                        case "x": x = value; break;
                        case "y": y = value; break;
                        case "z": z = value; break;
                    }
                }

                Position = new Coordinate3D(x, y, z);
            }

            public void ApplyVelocity()
            {
                Position = Position.Add(Velocity);
            }

            public long PotentialEnergy() => Energy(Position);
            public long KineticEnergy() => Energy(Velocity);
            public long TotalEnergy => PotentialEnergy() * KineticEnergy();


            public long Energy(Coordinate3D coordinate)
            {
                return Math.Abs(coordinate.X) + Math.Abs(coordinate.Y) + Math.Abs(coordinate.Z);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Shared;

namespace AdventOfCode._2021.Day22
{
    public class Day22 : Day
    {
        private List<RebootStep> _rebootSteps;
        private Regex _rebootStepParser = new Regex("(?<rebootcommand>[onf]*) x=(?<xfrom>[-0-9]*)\\.\\.(?<xto>[-0-9]*),y=(?<yfrom>[-0-9]*)\\.\\.(?<yto>[-0-9]*),z=(?<zfrom>[-0-9]*)\\.\\.(?<zto>[-0-9]*)");

        public Day22() : base(2021, 22, "Day22/input.txt", "576028", "1387966280636636")
        {
        }

        public override void Initialise()
        {
            _rebootSteps = InputLines.Select(ParseLine).ToList();
        }

        private RebootStep ParseLine(string line)
        {
            var matches = _rebootStepParser.Match(line);
            var rebootcommand = matches.Groups["rebootcommand"].Value;
            var xfrom = int.Parse(matches.Groups["xfrom"].Value);
            var xto = int.Parse(matches.Groups["xto"].Value);
            var yfrom = int.Parse(matches.Groups["yfrom"].Value);
            var yto = int.Parse(matches.Groups["yto"].Value);
            var zfrom = int.Parse(matches.Groups["zfrom"].Value);
            var zto = int.Parse(matches.Groups["zto"].Value);

            return new RebootStep
            {
                RebootCommand = rebootcommand == "on" ? RebootCommand.On : RebootCommand.Off,
                From = new Coordinate(xfrom, yfrom, zfrom),
                To = new Coordinate(xto, yto, zto)
            };
        }

        public override string Part1()
        {
            var reactor = new Dictionary<string,bool>();

            foreach (var command in _rebootSteps)
            {
                var fromX = command.From.X < -50 ? -50 : command.From.X;
                var toX = command.To.X > 50 ? 50 : command.To.X;
                for (var x = fromX; x <= toX; x++)
                {
                    var fromY = command.From.Y < -50 ? -50 : command.From.Y;
                    var toY = command.To.Y > 50 ? 50 : command.To.Y;
                    for (var y = fromY; y <= toY; y++)
                    {
                        var fromZ = command.From.Z < -50 ? -50 : command.From.Z;
                        var toZ = command.To.Z > 50 ? 50 : command.To.Z;
                        for (var z = fromZ; z <= toZ; z++)
                        {
                            var key = $"{x},{y},{z}";
                            var cubeState = command.RebootCommand == RebootCommand.On;
                            reactor[key] = cubeState;
                        }

                    }
                }
            }

            var onCount = reactor.Values.Where(v => v).Count();
            return onCount.ToString();
        }

        public override string Part2()
        {
            return "Runs too slowly";
            var cubes = new List<Cube>();
        
            foreach (var command in _rebootSteps)
            {
                var cube = new Cube
                {
                    From = command.From,
                    To = command.To
                };

                //Trace($"Processing {cube}");

                var newCubes = new List<Cube>();
                foreach (var existingCube in cubes)
                {
                    var replacements = existingCube.SliceOut(cube).ToList();
                    /*
                    Trace($"Replacement cubes for {existingCube}");
                    foreach (var c in replacements)
                    {
                        Trace($"{c}");
                    }
                    */
                    newCubes.AddRange(replacements);
                }

                if (command.RebootCommand == RebootCommand.On)
                {
                    //Trace($"Adding on {cube}");
                    newCubes.Add(cube);
                }

                cubes = newCubes;
                /*
                Trace($"Resulting cubes");
                foreach (var c in cubes)
                {
                    Trace($"{c}");
                }*/
                //Trace($"New count: {cubes.Sum(c => c.OnCount())}");
                //Trace();
            }

            var totalOn = cubes.Sum(c => c.OnCount());

            return totalOn.ToString();
        }

        private class Cube
        {
            public long Left => From.X;
            public long Right => To.X;
            public long Top => To.Y;
            public long Bottom => From.X;
            public long Front => From.Z;
            public long Back => To.Z;

            public Coordinate From { get; set; }
            public Coordinate To { get; set; }

            public IEnumerable<Cube> SliceOut(Cube cube)
            {
                if (cube.FullyContains(this))
                {
                    //Trace($"Fully contained {this}");
                    return Enumerable.Empty<Cube>();
                }

                if (Left < cube.Left && cube.Left <= Right)
                {
                    var left = new Cube
                    {
                        From = new Coordinate(From.X, From.Y, From.Z),
                        To = new Coordinate(cube.From.X - 1, To.Y, To.Z)
                    };

                    var right = new Cube
                    {
                        From = new Coordinate(cube.From.X, From.Y, From.Z),
                        To = new Coordinate(To.X, To.Y, To.Z)
                    };

                    //Trace($"Keep left {left} Slice right {right}");
                    return KeepAndSlice(left, right, cube);
                }

                if (Left <= cube.Right && cube.Right < Right)
                {
                    var left = new Cube
                    {
                        From = new Coordinate(From.X, From.Y, From.Z),
                        To = new Coordinate(cube.To.X, To.Y, To.Z)
                    };

                    var right = new Cube
                    {
                        From = new Coordinate(cube.To.X + 1, From.Y, From.Z),
                        To = new Coordinate(To.X, To.Y, To.Z)
                    };
                    //Trace($"Keep right {right} Slice left {left}");
                    return KeepAndSlice(right, left, cube);
                }

                if (From.Y < cube.From.Y && cube.From.Y <= To.Y)
                {
                    var bottom = new Cube
                    {
                        From = new Coordinate(From.X, From.Y, From.Z),
                        To = new Coordinate(To.X, cube.From.Y - 1, To.Z)
                    };

                    var top = new Cube
                    {
                        From = new Coordinate(From.X, cube.From.Y, From.Z),
                        To = new Coordinate(To.X, To.Y, To.Z)
                    };
                    //Trace($"Keep bottom {bottom} Slice top {top}");
                    return KeepAndSlice(bottom, top, cube);
                }

                if (From.Y <= cube.To.Y && cube.To.Y < To.Y)
                {
                    var bottom = new Cube
                    {
                        From = new Coordinate(From.X, From.Y, From.Z),
                        To = new Coordinate(To.X, cube.To.Y, To.Z)
                    };

                    var top = new Cube
                    {
                        From = new Coordinate(From.X, cube.To.Y + 1, From.Z),
                        To = new Coordinate(To.X, To.Y, To.Z)
                    };
                    //Trace($"Keep top {top} Slice bottom {bottom}");
                    return KeepAndSlice(top, bottom, cube);
                }

                if (From.Z < cube.From.Z && cube.From.Z <= To.Z)
                {
                    var front = new Cube
                    {
                        From = new Coordinate(From.X, From.Y, From.Z),
                        To = new Coordinate(To.X, To.Y, cube.From.Z - 1)
                    };

                    var back = new Cube
                    {
                        From = new Coordinate(From.X, From.Y, cube.From.Z),
                        To = new Coordinate(To.X, To.Y, To.Z)
                    };
                    //Trace($"Keep front {front} Slice back {back}");
                    return KeepAndSlice(front, back, cube);
                }

                if (From.Z <= cube.To.Z && cube.To.Z < To.Z)
                {
                    var front = new Cube
                    {
                        From = new Coordinate(From.X, From.Y, From.Z),
                        To = new Coordinate(To.X, To.Y, cube.To.Z)
                    };

                    var back = new Cube
                    {
                        From = new Coordinate(From.X, From.Y, cube.To.Z + 1),
                        To = new Coordinate(To.X, To.Y, To.Z)
                    };
                    //Trace($"Keep back {back} Slice front {front}");
                    return KeepAndSlice(back, front, cube);
                }

                return new List<Cube> { this };
            }

            public IEnumerable<Cube> KeepAndSlice(Cube keep, Cube slice, Cube cubeToSliceOut)
            {
                yield return keep;

                foreach (var c in slice.SliceOut(cubeToSliceOut))
                {
                    yield return c;
                }
            }

            public bool FullyContains(Cube cube)
            {
                return cube.Vertices.All(v => Contains(v));
            }

            public IEnumerable<Coordinate> Vertices => new List<Coordinate>
                {
                    new Coordinate(From.X, From.Y, From.Z),
                    new Coordinate(From.X, From.Y, To.Z),
                    new Coordinate(From.X, To.Y, From.Z),
                    new Coordinate(From.X, To.Y, To.Z),
                    new Coordinate(To.X, From.Y, From.Z),
                    new Coordinate(To.X, From.Y, To.Z),
                    new Coordinate(To.X, To.Y, From.Z),
                    new Coordinate(To.X, To.Y, To.Z)
                };

            public bool Contains(Coordinate coordinate)
            {
                return From.X <= coordinate.X && coordinate.X <= To.X
                    && From.Y <= coordinate.Y && coordinate.Y <= To.Y
                    && From.Z <= coordinate.Z && coordinate.Z <= To.Z;
            }

            public long OnCount()
            {
                var width = To.X - From.X + 1;
                var height = To.Y - From.Y + 1;
                var depth = To.Z - From.Z + 1;

                return width * height * depth;
            }

            public override string ToString()
            {
                return $"[{From} -> {To}]";
            }
        }

        private enum RebootCommand
        {
            On,
            Off
        }

        private class RebootStep
        {
            public RebootCommand RebootCommand { get; set; }
            public Coordinate From { get; set; }
            public Coordinate To { get; set; }
        }

        private class Coordinate
        {
            public long X { get; set; }
            public long Y { get; set; }
            public long Z { get; set; }

            public Coordinate(long x, long y, long z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public override string ToString()
            {
                return $"{X},{Y},{Z}";
            }
        }
    }
}

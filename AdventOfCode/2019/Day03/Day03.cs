using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;
using AdventOfCode.Shared.DataStructures;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2019.Day03
{
    public class Day03 : Day
    {
        public Day03() : base(2019, 3, "Day03/input_2019_03.txt", "232", "6084")
        {

        }

        private Wire[] _wires;
        public override void Initialise()
        {
            _wires = InputLines
                .Select(line => new Wire(line))
                .ToArray();
        }

        public override string Part1()
        {
            return FindClosestIntersection(0, 1).ToString();
        }

        public override string Part2()
        {
            return FindClosestSignalIntersection(0, 1).ToString();
        }

        public long FindClosestIntersection(int firstWireIndex, int secondWireIndex)
        {
            var wire0 = _wires[firstWireIndex];
            var wire1 = _wires[secondWireIndex];

            var intersections = wire0.GetIntersections(wire1).ToList();

            var result = intersections.Min(c => c.Key.ManhattanDistanceTo(Coordinate2D.Origin));

            return result;
        }

        public int FindClosestSignalIntersection(int firstWireIndex, int secondWireIndex)
        {
            var wire0 = _wires[firstWireIndex];
            var wire1 = _wires[secondWireIndex];

            var intersections = wire0.GetIntersections(wire1).ToList();

            var result = intersections.Min(c => c.Value);

            return result;
        }

        private class Wire
        {
            public readonly List<Coordinate2D> Locations;
            public Wire(string wireDescription)
            {
                Locations = ParseWire(wireDescription);
            }

            private static List<Coordinate2D> ParseWire(string wireDescription)
            {
                var coordinates = new List<Coordinate2D>();

                var location = Coordinate2D.Origin;

                var directions = wireDescription.Split(",");
                foreach (var direction in directions)
                {
                    var dir = direction[0];
                    var distanceToMove = int.Parse(direction.Substring(1));

                    while (distanceToMove > 0)
                    {
                        location = location.Neighbour(ParseDirection(dir));
                        coordinates.Add(location);
                        distanceToMove -= 1;
                    }
                }

                return coordinates;
            }

            public IEnumerable<KeyValuePair<Coordinate2D, int>> GetIntersections(Wire wire)
            {
                var locationDictionary = new Dictionary2D<long, long, int>();
                var signalDelay = 1;
                foreach (var location in Locations)
                {
                    locationDictionary[location.X][location.Y] = signalDelay;
                    signalDelay += 1;
                }

                signalDelay = 1;
                foreach (var location in wire.Locations)
                {
                    if (locationDictionary.ContainsKey(location.X, location.Y))
                    {
                        var thisSignalDelay = locationDictionary[location.X][location.Y];
                        yield return KeyValuePair.Create(location, signalDelay + thisSignalDelay);
                    }

                    signalDelay += 1;
                }
            }
        }

        private static Direction ParseDirection(char direction)
        {
            switch (direction)
            {
                case 'U': return Direction.Up;
                case 'D': return Direction.Down;
                case 'L': return Direction.Left;
                case 'R': return Direction.Right;
            }

            throw new Exception($"Unexpected direction: {direction}");
        }
    }
}

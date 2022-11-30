using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2020.Day12
{
    public class Day12 : Day
    {
        public Day12() : base(2020, 12, "Day12/input_2020_12.txt", "1221", "59435")
        {

        }

        private List<NavigationInstruction> _instructions;

        public override void Initialise()
        {
            _instructions = InputLines
                .Select(ParseLine)
                .ToList();
        }

        private static NavigationInstruction ParseLine(string line)
        {
            var instruction = line[0];
            var distance = int.Parse(line.Substring(1));

            return new NavigationInstruction()
            {
                Instruction = instruction,
                Distance = distance
            };
        }

        public override string Part1()
        {
            var north = 0;
            var east = 0;
            var currentDirection = 'E';

            foreach (var instruction in _instructions)
            {
                var direction = instruction.Instruction;
                if (direction == 'F')
                {
                    direction = currentDirection;
                }

                switch(direction)
                {
                    case 'N':
                        north += instruction.Distance;
                        break;
                    case 'E':
                        east += instruction.Distance;
                        break;
                    case 'S':
                        north -= instruction.Distance;
                        break;
                    case 'W':
                        east -= instruction.Distance;
                        break;
                    case 'L':
                    case 'R':
                        currentDirection = GetBearing(currentDirection, direction, instruction.Distance);
                        break;
                }
            }

            return (Math.Abs(north) + Math.Abs(east)).ToString();
        }

        private char GetBearing(char initialBearing, char direction, int degrees)
        {
            var compassPoints = new[] { 'N', 'E', 'S', 'W' };

            var currentIndex = Array.IndexOf(compassPoints, initialBearing);
            var indexStep = degrees / 90;
            if (direction == 'L')
            {
                indexStep *= -1;
            }

            var newIndex = (currentIndex + indexStep + compassPoints.Length) % compassPoints.Length;

            return compassPoints[newIndex];
        }

        public override string Part2()
        {
            var shipNorth = 0;
            var shipEast = 0;
            var waypointNorth = 1;
            var waypointEast = 10;

            foreach (var instruction in _instructions)
            {
                var direction = instruction.Instruction;

                switch (direction)
                {
                    case 'F':
                        shipNorth += waypointNorth * instruction.Distance;
                        shipEast += waypointEast * instruction.Distance;
                        break;
                    case 'N':
                        waypointNorth += instruction.Distance;
                        break;
                    case 'E':
                        waypointEast += instruction.Distance;
                        break;
                    case 'S':
                        waypointNorth -= instruction.Distance;
                        break;
                    case 'W':
                        waypointEast -= instruction.Distance;
                        break;
                    case 'L':
                        switch (instruction.Distance)
                        {
                            case 90:
                                var originalNorth = waypointNorth;
                                waypointNorth = waypointEast;
                                waypointEast = originalNorth * -1;
                                break;
                            case 180:
                                waypointNorth *= -1;
                                waypointEast *= -1;
                                break;
                            case 270:
                                var originalEast = waypointEast;
                                waypointEast = waypointNorth;
                                waypointNorth = originalEast * -1;
                                break;
                        }
                        break;
                    case 'R':
                        switch (instruction.Distance)
                        {
                            case 90:
                                var originalEast = waypointEast;
                                waypointEast = waypointNorth;
                                waypointNorth = originalEast * -1;
                                break;
                            case 180:
                                waypointNorth *= -1;
                                waypointEast *= -1;
                                break;
                            case 270:
                                var originalNorth = waypointNorth;
                                waypointNorth = waypointEast;
                                waypointEast = originalNorth * -1;
                                break;
                        }
                        break;
                }
            }

            return (Math.Abs(shipNorth) + Math.Abs(shipEast)).ToString();
        }

        private class NavigationInstruction
        {
            public char Instruction { get; set; }
            public int Distance { get; set; }
        }
    }
}

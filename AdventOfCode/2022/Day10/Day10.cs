using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.PrimitiveExtensions;

namespace AdventOfCode._2022.Day09
{
    public class Day10 : Day
    {
        public Day10() : base(2022, 10, "Day10/input_2022_10.txt", "", "")
        {

        }

        private List<IInstruction> _instructions;
        public override void Initialise()
        {
            _instructions = InputLines
                .Select(Create)
                .ToList();
        }

        private IInstruction Create(string instruction)
        {
            if (instruction.StartsWith("addx"))
            {
                var split = instruction.Split(" ");
                return new AddX(int.Parse(split[1]));
            }

            return new Noop();
        }

        public override string Part1()
        {
            var cyclesToCheck = new List<int>
            {
                20,
                60,
                100,
                140,
                180,
                220
            };

            var result = 0;
            var crt = new Crt();
            var cycle = 1;

            foreach (var instruction in _instructions)
            {
                while (!instruction.IsComplete())
                {
                    if (cyclesToCheck.Contains(cycle))
                    {
                        Console.WriteLine($"Result += {cycle} * {crt.X} ({cycle * crt.X})");
                        result += cycle * crt.X;
                    }

                    instruction.Cycle(crt);
                    cycle++;
                    Console.WriteLine($"Cycle {cycle}");
                }
            }

            return result.ToString();
        }

        public override string Part2()
        {
            return "";
        }

        private interface IInstruction
        {
            void Cycle(Crt crt);
            bool IsComplete();
        }

        private class AddX : IInstruction
        {
            private readonly int _operand;
            private int cyclesRemaining = 2;

            public AddX(int operand)
            {
                _operand = operand;
            }

            public void Cycle(Crt crt)
            {
                cyclesRemaining -= 1;
                if (cyclesRemaining == 0)
                {
                    crt.X += _operand;
                    Console.WriteLine($"Add {_operand}");
                }
                // crt.Cycle += 1;
            }

            public bool IsComplete()
            {
                return cyclesRemaining <= 0;
            }
        }

        private class Noop : IInstruction
        {
            private int cyclesRemaining = 1;

            public void Cycle(Crt crt)
            {
                cyclesRemaining -= 1;

                Console.WriteLine("Noop");
                // crt.Cycle += 1;
            }

            public bool IsComplete()
            {
                return cyclesRemaining <= 0;
            }
        }

        private class Crt
        {
            // public int Cycle { get; set; } = 1;
            public int X { get; set; } = 1;
        }
    }
}


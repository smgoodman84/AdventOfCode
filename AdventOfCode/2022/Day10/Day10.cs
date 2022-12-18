using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode.Shared;

namespace AdventOfCode._2022.Day09
{
    public class Day10 : Day
    {
        private const string Part2Result = "\n#### ###   ##  ###  #  # ####  ##  #  # \n#    #  # #  # #  # #  # #    #  # #  # \n###  #  # #    #  # #### ###  #    #### \n#    ###  # ## ###  #  # #    # ## #  # \n#    #    #  # #    #  # #    #  # #  # \n#    #     ### #    #  # #     ### #  # \n";

        public Day10() : base(2022, 10, "Day10/input_2022_10.txt", "10760", Part2Result)
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
            var cpu = new Cpu();

            cpu.Execute(
                _instructions,
                (cpu, cycle) =>
                {
                    if (cyclesToCheck.Contains(cycle))
                    {
                        result += cycle * cpu.X;
                    }
                });

            return result.ToString();
        }

        public override string Part2()
        {
            var output = new StringBuilder();
            output.Append('\n');


            var cpu = new Cpu();
            var crt = new Crt(cpu);

            cpu.Execute(
                _instructions,
                (cpu, cycle) =>
                {
                    if (crt.Draw(cycle - 1))
                    {
                        output.Append('#');
                    }
                    else
                    {
                        output.Append(' ');
                    }

                    if (cycle % 40 == 0)
                    {
                        output.Append('\n');
                    }
                });

            return output.ToString();
        }

        private class Crt
        {
            private readonly Cpu _cpu;

            public Crt(Cpu cpu)
            {
                _cpu = cpu;
            }

            public bool Draw(int cycle)
            {
                var position = cycle % 40;

                if (_cpu.X - 1 <= position && position <= _cpu.X + 1)
                {
                    return true;
                }

                return false;
            }
        }

        private class Cpu
        {
            public int X { get; set; } = 1;

            private int _clock = 0;

            public void Execute(IEnumerable<IInstruction> instructions, Action<Cpu, int> tickAction)
            {
                foreach (var instruction in instructions)
                {
                    var tickCount = instruction.CyclesToExecute;
                    while (tickCount > 0)
                    {
                        tickCount -= 1;
                        _clock += 1;

                        tickAction(this, _clock);
                    }

                    instruction.Execute(this);
                }
            }
        }

        private interface IInstruction
        {
            int CyclesToExecute { get; }
            void Execute(Cpu cpu);
        }

        private class AddX : IInstruction
        {
            public int CyclesToExecute => 2;
            private readonly int _operand;

            public AddX(int operand)
            {
                _operand = operand;
            }

            public void Execute(Cpu cpu)
            {
                cpu.X += _operand;
            }
        }

        private class Noop : IInstruction
        {
            public int CyclesToExecute => 1;

            public void Execute(Cpu cpu)
            {
            }
        }
    }
}


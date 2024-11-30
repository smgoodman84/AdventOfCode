using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2015.Day06;

public class Day06 : Day
{
    public Day06() : base(2015, 6, "Day06/input_2015_06.txt", "543903", "14687245")
    {
    }

    private List<Instruction> _instructions;
    public override void Initialise()
    {
        _instructions = InputLines
            .Select(line => new Instruction(line))
            .ToList();
    }

    public override string Part1()
    {
        var grid = new Grid(1000, 1000);

        RunInstructionsOnGrid(grid);

        return grid.GetTotalActive().ToString();
    }

    public override string Part2()
    {
        var grid = new VariableGrid(1000, 1000);

        RunInstructionsOnGrid(grid);

        return grid.GetTotalActive().ToString();
    }

    private void RunInstructionsOnGrid(IGrid grid)
    {
        var instructionNumber = 1;
        foreach (var instruction in _instructions)
        {
            TraceLine($"Instruction {instructionNumber}");

            Action<int, int> gridAction = (x, y) => throw new Exception("GridAction Not Set");
            switch (instruction.Code)
            {
                case Instruction.InstructionCode.Toggle:
                    gridAction = grid.Toggle;
                    break;
                case Instruction.InstructionCode.TurnOn:
                    gridAction = grid.TurnOn;
                    break;
                case Instruction.InstructionCode.TurnOff:
                    gridAction = grid.TurnOff;
                    break;
            }

            for (var x = instruction.Start.X; x <= instruction.End.X; x++)
            {
                for (var y = instruction.Start.Y; y <= instruction.End.Y; y++)
                {
                    gridAction((int)x, (int)y);
                }
            }

            instructionNumber += 1;
        }
    }

    private class Instruction
    {
        private static readonly Regex instructionRegex =
            new Regex("(?<command>.*) (?<startx>[0-9]*),(?<starty>[0-9]*) through (?<endx>[0-9]*),(?<endy>[0-9]*)", RegexOptions.Compiled);

        public enum InstructionCode
        {
            Toggle,
            TurnOn,
            TurnOff
        }

        public InstructionCode Code { get; set; }
        public Coordinate2D Start { get; set; }
        public Coordinate2D End { get; set; }

        public Instruction(string instruction)
        {
            var match = instructionRegex.Match(instruction);
            Code = ParseCode(match.Groups["command"].Value);

            var startx = int.Parse(match.Groups["startx"].Value);
            var starty = int.Parse(match.Groups["starty"].Value);
            var endx = int.Parse(match.Groups["endx"].Value);
            var endy = int.Parse(match.Groups["endy"].Value);

            Start = new Coordinate2D(startx, starty);
            End = new Coordinate2D(endx, endy);
        }

        private InstructionCode ParseCode(string code)
        {
            return code switch
            {
                "toggle" => InstructionCode.Toggle,
                "turn on" => InstructionCode.TurnOn,
                "turn off" => InstructionCode.TurnOff,
                _ => throw new Exception($"Unknown InstructionCode {code}"),
            };
        }
    }

    private interface IGrid
    {
        void TurnOn(int x, int y);
        void TurnOff(int x, int y);
        void Toggle(int x, int y);
        int GetTotalActive();
    }

    private class Grid : IGrid
    {
        private readonly bool[,] _grid;
        private readonly int _width;
        private readonly int _height;
        private int _totalActive;

        public Grid(int width, int height)
        {
            _width = width;
            _height = height;
            _grid = new bool[_width, _height];
            _totalActive = 0;
        }

        public void TurnOn(int x, int y)
        {
            if (!_grid[x, y])
            {
                _grid[x, y] = true;
                _totalActive += 1;
            }
        }

        public void TurnOff(int x, int y)
        {
            if (_grid[x, y])
            {
                _grid[x, y] = false;
                _totalActive -= 1;
            }
        }

        public void Toggle(int x, int y)
        {
            if (_grid[x, y])
            {
                _grid[x, y] = false;
                _totalActive -= 1;
            }
            else
            {
                _grid[x, y] = true;
                _totalActive += 1;
            }
        }

        public int GetTotalActive() => _totalActive;
    }

    private class VariableGrid : IGrid
    {
        private readonly int[,] _grid;
        private readonly int _width;
        private readonly int _height;
        private int _totalActive;

        public VariableGrid(int width, int height)
        {
            _width = width;
            _height = height;
            _grid = new int[_width, _height];
            _totalActive = 0;
        }

        public void TurnOn(int x, int y)
        {
            _grid[x, y] += 1;
            _totalActive += 1;
        }

        public void TurnOff(int x, int y)
        {
            if (_grid[x, y] > 0)
            {
                _grid[x, y] -= 1;
                _totalActive -= 1;
            }
        }

        public void Toggle(int x, int y)
        {
            _grid[x, y] += 2;
            _totalActive += 2;
        }

        public int GetTotalActive() => _totalActive;
    }
}
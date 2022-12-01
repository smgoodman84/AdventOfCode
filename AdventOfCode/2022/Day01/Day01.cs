using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;

namespace AdventOfCode._2022.Day01
{
    public class Day01 : Day
    {
        public Day01() : base(2022, 1, "Day01/input_2022_01.txt", "66719", "198551")
        {

        }

        private List<List<int>> _elfCalories;
        public override void Initialise()
        {
            _elfCalories = LineGrouper.GroupLines(InputLines)
                .Select(group => group.Select(int.Parse).ToList())
                .ToList();
        }

        public override string Part1()
        {
            var max = _elfCalories
                .Select(elf => elf.Sum())
                .Max();

            return max.ToString();
        }

        public override string Part2()
        {
            var topThree = _elfCalories
                .Select(elf => elf.Sum())
                .OrderByDescending(calories => calories)
                .Take(3)
                .Sum();

            return topThree.ToString();
        }
    }
}


using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2019.Day01
{
    public class Day01 : Day
    {
        public Day01() : base(2019, 1, "Day01/input_2019_01.txt", "3401852", "5099916")
        {

        }

        private List<int> _moduleMasses;
        public override void Initialise()
        {
            _moduleMasses = InputLines
                .Select(int.Parse)
                .ToList();
        }

        public int GetFuelRequirements()
        {
            return _moduleMasses
                .Select(GetFuelRequirement)
                .Sum();
        }

        public override string Part1()
        {
            return GetFuelRequirements().ToString();
        }

        public override string Part2()
        {
            return GetFuelRequirementsWithFuelForFuel().ToString();
        }

        private int GetFuelRequirement(int moduleMass)
        {
            return (moduleMass / 3) - 2;
        }

        public int GetFuelRequirementsWithFuelForFuel()
        {
            return _moduleMasses
                .Select(GetFuelRequirementWithFuelForFuel)
                .Sum();
        }

        private int GetFuelRequirementWithFuelForFuel(int moduleMass)
        {
            var totalFuelMass = GetFuelRequirement(moduleMass);

            var unfueledMass = totalFuelMass;
            while (true)
            {
                unfueledMass = GetFuelRequirement(unfueledMass);
                if (unfueledMass <= 0)
                {
                    break;
                }

                totalFuelMass += unfueledMass;
            }

            return totalFuelMass;
        }
    }
}

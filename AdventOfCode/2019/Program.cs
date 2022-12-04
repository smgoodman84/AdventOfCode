using System;
using AdventOfCode2019.Day14;
using AdventOfCode2019.Day15;
using AdventOfCode2019.Day16;
using AdventOfCode2019.Day17;
using AdventOfCode._2019.Intcode;

namespace AdventOfCode2019
{
    class Program
    {
        static void Main(string[] args)
        {
            var day14Part1Result = OreForFuelCalculator.LoadFromFile("Day14/Reactions.txt").OreRequiredForChemical("FUEL", 1);
            Console.WriteLine($"Day 14 Part 1: {day14Part1Result}");

            //var day14Part2Result = OreForFuelCalculator.LoadFromFile("Day14/Reactions.txt").MaximumChemicalWithOre("FUEL", 1000000000000);
            //Console.WriteLine($"Day 14 Part 2: {day14Part2Result}");
            
            var day15Part1Result = new OxygenSystemLocater().GetShortestRouteToOxygen();
            Console.WriteLine($"Day 15 Part 1: {day15Part1Result}");

            var day15Part2Result = new OxygenSystemLocater().GetTimeToFillWithOxygen();
            Console.WriteLine($"Day 15 Part 2: {day15Part2Result}");
            
            var day16Part1Result = FlawedFrequencyTransmission.LoadFromFile("Day16/Signal.txt").ProcessSignal(100);
            Console.WriteLine($"Day 16 Part 1: {day16Part1Result}");
            
            var day16Part2Result = FlawedFrequencyTransmission.LoadFromFile("Day16/Signal.txt").ProcessRepeatedSignal(100, 10000);
            Console.WriteLine($"Day 16 Part 2: {day16Part2Result}");
            
            var day17Part1Result = new Scaffold().GetSumOfAlignmentParameters();
            Console.WriteLine($"Day 17 Part 1: {day17Part1Result}");
            
            var day17Part2Result = new Scaffold().CollectSpaceDust().Result;
            Console.WriteLine($"Day 17 Part 2: {day17Part2Result}");

            Console.ReadKey();
        }
    }
}

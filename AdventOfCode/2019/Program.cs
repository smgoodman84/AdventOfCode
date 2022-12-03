using System;
using AdventOfCode2019.Day10;
using AdventOfCode2019.Day12;
using AdventOfCode2019.Day13;
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
            Console.WriteLine($"Day 9 Part 1:");
            IntcodeMachine.LoadFromFile("Day09/Boost.txt").SetInput(new PreparedInput(1)).Execute().Wait();

            Console.WriteLine($"Day 9 Part 2:");
            IntcodeMachine.LoadFromFile("Day09/Boost.txt").SetInput(new PreparedInput(2)).Execute().Wait();

            var day10Part1Result = AsteroidMap.LoadFromFile("Day10/AsteroidMap.txt").GetMaximumVisibility();
            Console.WriteLine($"Day 10 Part 1: {day10Part1Result}");

            var day10Part2Result = AsteroidMap.LoadFromFile("Day10/AsteroidMap.txt").GetNthDestroyedAsteroidLocation(200);
            Console.WriteLine($"Day 10 Part 2: {day10Part2Result}");

            var day11Part1Result = new Day11.Robot().Execute().GetAwaiter().GetResult().GetEverWhiteCount();
            Console.WriteLine($"Day 11 Part 1: {day11Part1Result}");
            
            Console.WriteLine($"Day 11 Part 2:");
            new Day11.Robot().StartOnWhite().Execute().GetAwaiter().GetResult().RenderHull();
            
            var day12Part1Result  = GravitationalEnergy.LoadFromFile("Day12/MoonLocations.txt").Simulate(1000).GetTotalEnergy();
            Console.WriteLine($"Day 12 Part 1: {day12Part1Result}");

            var day12Part2Result = GravitationalEnergy.LoadFromFile("Day12/MoonLocations.txt").FindCycle();
            Console.WriteLine($"Day 12 Part 2: {day12Part2Result}");

            var day13Part1Result = Game.LoadFromFile("Day13/Game.txt").Execute().CountCharacters(2);
            Console.WriteLine($"Day 13 Part 1: {day13Part1Result}");
            
            var day13Part2Result = Game.LoadFromFile("Day13/Game.txt").ExecuteWithInput().GetAwaiter().GetResult().GetScore();
            Console.WriteLine($"Day 13 Part 2: {day13Part2Result}");
            
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

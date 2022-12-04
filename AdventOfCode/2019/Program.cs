using System;
using AdventOfCode2019.Day16;
using AdventOfCode2019.Day17;
using AdventOfCode._2019.Intcode;

namespace AdventOfCode2019
{
    class Program
    {
        static void Main(string[] args)
        {
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

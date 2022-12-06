using System;
using AdventOfCode2019.Day17;
using AdventOfCode._2019.Intcode;

namespace AdventOfCode2019
{
    class Program
    {
        static void Main(string[] args)
        {
            var day17Part1Result = new Scaffold().GetSumOfAlignmentParameters();
            Console.WriteLine($"Day 17 Part 1: {day17Part1Result}");
            
            var day17Part2Result = new Scaffold().CollectSpaceDust().Result;
            Console.WriteLine($"Day 17 Part 2: {day17Part2Result}");

            Console.ReadKey();
        }
    }
}

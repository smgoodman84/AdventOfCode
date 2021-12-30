using System;
using System.Collections.Generic;
using System.Diagnostics;
using AdventOfCode.Shared;

namespace AdventOfCode.AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            var days = new List<IDay>
            {
                new _2015.Day01.Day01(),
            };

            var invalidCount = 0;
            foreach (var day in days)
            {
                invalidCount += ResultForDay(day.DayNumber, 1, () => day.Part1(), day.ValidatedPart1);
                invalidCount += ResultForDay(day.DayNumber, 2, () => day.Part2(), day.ValidatedPart2);
            }

            Console.WriteLine($"{invalidCount} INVALID Results");
        }

        private static int ResultForDay(int day, int part, Func<string> resultFunc, string validatedResult)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = resultFunc();
            stopwatch.Stop();

            var invalid = !string.IsNullOrWhiteSpace(validatedResult)
                && result != validatedResult;

            var invalidString = invalid ? " INVALID" : "";

            Console.WriteLine($"Day {day} Part {part} ({stopwatch.ElapsedMilliseconds}ms): {result}{invalidString}");

            return invalid ? 1 : 0;
        }
    }
}

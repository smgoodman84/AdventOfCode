using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                new _2015.Day02.Day02(),
                new _2015.Day03.Day03(),
                new _2015.Day04.Day04(),
                new _2015.Day05.Day05(),
            };

            var yearToRun = 2015;
            var dayToRun = 5;
            var invalidCount = 0;
            foreach (var day in days.Where(x => x.Year == yearToRun && x.DayNumber == dayToRun))
            {
                day.Initialise();
                invalidCount += ResultForDay(day.DayNumber, 1, () => day.Part1(), day.ValidatedPart1);
                invalidCount += ResultForDay(day.DayNumber, 2, () => day.Part2(), day.ValidatedPart2);
            }

            Console.WriteLine($"{invalidCount} INVALID Results");
        }

        private static int ResultForDay(int day, int part, Func<string> resultFunc, string validatedResult)
        {
            var stopwatch = Stopwatch.StartNew();
            string result;
            try
            {
                result = resultFunc();
            }
            catch (Exception ex)
            {
                result = $"EXCEPTION: {ex.Message}";
            }
            stopwatch.Stop();

            var invalid = !string.IsNullOrWhiteSpace(validatedResult)
                && result != validatedResult;

            var invalidString = invalid ? " INVALID" : "";

            Console.WriteLine($"Day {day} Part {part} ({stopwatch.ElapsedMilliseconds}ms): {result}{invalidString}");

            return invalid ? 1 : 0;
        }
    }
}

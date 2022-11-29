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


                new _2021.Day01.Day01(),
                new _2021.Day02.Day02(),
                new _2021.Day03.Day03(),
                new _2021.Day04.Day04(),
                new _2021.Day05.Day05(),
                new _2021.Day06.Day06(),
                new _2021.Day07.Day07(),
                new _2021.Day08.Day08(),
                new _2021.Day09.Day09(),
                new _2021.Day10.Day10(),
                new _2021.Day11.Day11(),
                new _2021.Day12.Day12(),
                new _2021.Day13.Day13(),
                new _2021.Day14.Day14(),
                new _2021.Day15.Day15(),
                new _2021.Day16.Day16(),
                new _2021.Day17.Day17(),
                new _2021.Day18.Day18(),
                new _2021.Day19.Day19(),
                new _2021.Day20.Day20(),
                new _2021.Day21.Day21(),
                // new _2021.Day22.Day22(),
                // new _2021.Day24.Day24(),
            };

            var runPredicate = RunYear(2021);

            var invalidCount = 0;
            foreach (var day in days.Where(x => runPredicate(x)))
            {
                day.Initialise();
                invalidCount += ResultForDay(day.Year, day.DayNumber, 1, () => day.Part1(), day.ValidatedPart1);
                invalidCount += ResultForDay(day.Year, day.DayNumber, 2, () => day.Part2(), day.ValidatedPart2);
            }

            Console.WriteLine($"{invalidCount} INVALID Results");
        }

        private static Func<IDay, bool> RunAll()
        {
            return day => true;
        }

        private static Func<IDay, bool> RunYear(int year)
        {
            return day => day.Year == year;
        }

        private static Func<IDay, bool> RunDay(int year, int dayNumber)
        {
            return day => day.Year == year && day.DayNumber == dayNumber;
        }

        private static int ResultForDay(int year, int day, int part, Func<string> resultFunc, string validatedResult)
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

            Console.WriteLine($"{year} Day {day} Part {part} ({stopwatch.ElapsedMilliseconds}ms): {result}{invalidString}");

            return invalid ? 1 : 0;
        }
    }
}

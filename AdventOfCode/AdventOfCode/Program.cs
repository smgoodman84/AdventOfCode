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
            var assemblies = new[]
            {
                typeof(_2015.Day01.Day01).Assembly,
                typeof(_2020.Day01.Day01).Assembly,
                typeof(_2021.Day01.Day01).Assembly
            };

            var days = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsAssignableTo(typeof(IDay)))
                .Select(t => (IDay)Activator.CreateInstance(t))
                .OrderBy(day => day.Year)
                .ThenBy(day => day.DayNumber)
                .ToList();

            // var runPredicate = RunLatestDayInYear(2020, days);
            var runPredicate = RunYear(2020);

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


        private static Func<IDay, bool> RunLatestDayInYear(int year, IEnumerable<IDay> allDays)
        {
            var maxDayInYear = allDays
                .Where(d => d.Year == year)
                .Select(d => d.DayNumber)
                .Max();

            return RunDay(year, maxDayInYear);
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

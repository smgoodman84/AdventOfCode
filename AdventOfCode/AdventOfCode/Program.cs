using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdventOfCode.Shared;

namespace AdventOfCode.AdventOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().Wait();
            System.Environment.Exit(0);
        }

        private static async Task MainAsync()
        {
            var assemblies = new[]
            {
                typeof(_2015.Day01.Day01).Assembly,
                typeof(_2019.Day01.Day01).Assembly,
                typeof(_2020.Day01.Day01).Assembly,
                typeof(_2021.Day01.Day01).Assembly,
                typeof(_2022.Day01.Day01).Assembly
            };

            var days = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsAssignableTo(typeof(IDay)))
                .Select(t => (IDay)Activator.CreateInstance(t))
                .OrderBy(day => day.Year)
                .ThenBy(day => day.DayNumber)
                .ToList();

            // var runPredicate = RunDay(2021, 22);
            var runPredicate = RunLatestDayInYear(2022, days);
            // var runPredicate = RunYear(2021);
            // var runPredicate = RunAll();

            var resultDetails = new List<Result>();
            foreach (var day in days.Where(x => runPredicate(x)))
            {
                day.Initialise();
                resultDetails.Add(await ResultForDay(day.Year, day.DayNumber, 1, () => day.Part1(), day.ValidatedPart1));
                resultDetails.Add(await ResultForDay(day.Year, day.DayNumber, 2, () => day.Part2(), day.ValidatedPart2));
            }

            var yearGroups = resultDetails
                .GroupBy(r => r.Year)
                .OrderBy(group => group.Key);

            foreach(var yearGroup in yearGroups)
            {
                var resultTypeGroups = yearGroup
                    .GroupBy(r => r.ResultType);

                Console.WriteLine();
                Console.WriteLine(yearGroup.Key);

                foreach (var resultType in resultTypeGroups)
                {
                    var count = resultType.Count();
                    var resultSuffix = count > 1 ? "s" : string.Empty;
                    Console.WriteLine($"{count} {resultType.Key} Result{resultSuffix}");
                }
            }
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

        private enum ResultType
        {
            None,
            Exception,
            Timeout,
            Unvalidated,
            Valid,
            Invalid,
            Incomplete
        }

        private class Result
        {
            public int Year { get; set; }
            public int Day { get; set; }
            public int Part { get; set; }

            public ResultType ResultType { get; set; }
            public string Value { get; set; }
            public long RuntimeMilliseconds { get; set; }
            public Exception Exception { get; set; }
            public Func<string> ResultFunction { get; set; }

            public void Execute() {
                try
                {
                    Value = ResultFunction();
                }
                catch (Exception ex)
                {
                    Exception = ex;
                    ResultType = ResultType.Exception;
                }
            }
        }

        private static async Task<Result> ResultForDay(int year, int day, int part, Func<string> resultFunc, string validatedResult)
        {
            var resultDetails = await GetResultForDay(year, day, part, resultFunc, validatedResult);

            var message = resultDetails.Value;
            if (resultDetails.ResultType == ResultType.Exception)
            {
                message = resultDetails.Exception.Message;
            }

            Console.WriteLine($"{year} Day {day} Part {part} ({resultDetails.RuntimeMilliseconds}ms): {message} - {resultDetails.ResultType}");

            return resultDetails;
        }


        private static async Task<Result> GetResultForDay(int year, int day, int part, Func<string> resultFunc, string validatedResult)
        {
            var resultDetails = new Result()
            {
                Year = year,
                Day = day,
                Part = part,
                ResultFunction = resultFunc
            };

            var stopwatch = Stopwatch.StartNew();
            // string result = null;
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();

                Thread thread = new Thread(new ThreadStart(resultDetails.Execute), 16*1024*1024);
                thread.Start();

                var timeout = 60_000;
                var delay = 1;
                while (thread.IsAlive && timeout > 0)
                {
                    await Task.Delay(delay);
                    timeout -= delay;

                    delay *= 2;
                    if (delay > 50)
                    {
                        delay = 50;
                    }

                    if (timeout <= 0)
                    {
                        resultDetails.ResultType = ResultType.Timeout;
                        // thread.Abort();
                    }
                }
                /*
                var resultTask = new Task<string>(() => resultFunc(), cancellationTokenSource.Token);
                var timeoutTask = Task.Delay(60_000, cancellationTokenSource.Token);

                resultTask.Start();
                await Task.WhenAny(resultTask, timeoutTask);

                if (resultTask.IsCompletedSuccessfully)
                {
                    result = await resultTask;
                    resultDetails.Value = result;
                }
                else
                {
                    resultDetails.ResultType = ResultType.Timeout;
                }

                cancellationTokenSource.Cancel();*/
            }
            catch (Exception ex)
            {
                resultDetails.Exception = ex;
                resultDetails.ResultType = ResultType.Exception;
            }
            finally
            {
                stopwatch.Stop();
                resultDetails.RuntimeMilliseconds = stopwatch.ElapsedMilliseconds;
            }

            if (resultDetails.ResultType != ResultType.None)
            {
                return resultDetails;
            }

            if (validatedResult == string.Empty)
            {
                if (resultDetails.Value == string.Empty)
                {
                    resultDetails.ResultType = ResultType.Incomplete;
                }
                else
                {
                    resultDetails.ResultType = ResultType.Unvalidated;
                }
            }
            else
            {
                if (resultDetails.Value == validatedResult)
                {
                    resultDetails.ResultType = ResultType.Valid;
                }
                else
                {
                    resultDetails.ResultType = ResultType.Invalid;
                }
            }

            return resultDetails;
        }
    }
}

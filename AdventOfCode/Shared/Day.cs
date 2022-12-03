using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Shared
{
    public abstract class Day : IDay
    {
        private readonly bool _outputTrace;

        public virtual void Initialise()
        {

        }
        public abstract string Part1();
        public abstract string Part2();

        public Day(
            int year,
            int dayNumber,
            string filename,
            string validatedPart1,
            string validatedPart2,
            bool outputTrace = false)
        {
            Year = year;
            DayNumber = dayNumber;

            InputLines = File
                .ReadAllLines(filename)
                .ToList();

            ValidatedPart1 = validatedPart1;
            ValidatedPart2 = validatedPart2;
            _outputTrace = outputTrace;

            if (File.Exists(TraceFilename))
            {
                File.Delete(TraceFilename);
            }
        }

        public int Year { get; private set; }
        public int DayNumber { get; private set; }
        public string ValidatedPart1 { get; private set; }
        public string ValidatedPart2 { get; private set; }

        protected List<string> InputLines { get; private set; }

        private string TraceFilename => $"trace_{Year}_{DayNumber:00}.txt";
        protected void Trace(string message)
        {
            if (_outputTrace)
            {
                Console.Write(message);
            }

            File.AppendAllText(TraceFilename, message);
        }

        protected void TraceLine(string message = "")
        {
            if (_outputTrace)
            {
                Console.WriteLine(message);
            }

            File.AppendAllLines(TraceFilename, new[] { message });
        }
    }
}
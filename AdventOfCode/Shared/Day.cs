using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Shared
{
    public abstract class Day : IDay
    {
        public abstract void Initialise();
        public abstract string Part1();
        public abstract string Part2();

        public Day(
            int year,
            int dayNumber,
            string filename,
            string validatedPart1,
            string validatedPart2)
        {
            Year = year;
            DayNumber = dayNumber;

            InputLines = File
                .ReadAllLines(filename)
                .ToList();

            ValidatedPart1 = validatedPart1;
            ValidatedPart2 = validatedPart2;
        }

        public int Year { get; private set; }
        public int DayNumber { get; private set; }
        public string ValidatedPart1 { get; private set; }
        public string ValidatedPart2 { get; private set; }

        protected List<string> InputLines { get; private set; }
    }
}
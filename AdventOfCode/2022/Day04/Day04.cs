using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2022.Day04;

public class Day04 : Day
{
    public Day04() : base(2022, 4, "Day04/input_2022_04.txt", "556", "876")
    {

    }

    private List<CleaningPair> _cleaningPairs;
    public override void Initialise()
    {
        _cleaningPairs = InputLines
            .Select(line => new CleaningPair(line))
            .ToList();
    }

    public override string Part1()
    {
        return _cleaningPairs
            .Where(p => p.OneFullyContainsAnother())
            .Count()
            .ToString();
    }

    public override string Part2()
    {
        return _cleaningPairs
            .Where(p => p.AnyOverlap())
            .Count()
            .ToString();
    }

    private class CleaningPair
    {
        private readonly CleaningAssignment _first;
        private readonly CleaningAssignment _second;

        public CleaningPair(string cleaningPair)
        {
            var split = cleaningPair.Split(",");

            _first = new CleaningAssignment(split[0]);
            _second = new CleaningAssignment(split[1]);
        }

        public bool OneFullyContainsAnother()
        {
            return _first.FullyContains(_second)
                   || _second.FullyContains(_first);
        }

        public bool AnyOverlap()
        {
            return _first.AnyOverlap(_second);
        }
    }

    private class CleaningAssignment
    {
        public int Start { get; private set; }
        public int End { get; private set; }

        public CleaningAssignment(string assignment)
        {
            var split = assignment.Split("-");

            Start = int.Parse(split[0]);
            End = int.Parse(split[1]);
        }

        public bool FullyContains(CleaningAssignment assignment)
        {
            return Start <= assignment.Start && End >= assignment.End;
        }

        public bool AnyOverlap(CleaningAssignment assignment)
        {
            return FullyContains(assignment)
                   || assignment.FullyContains(this)
                   || (Start <= assignment.Start && assignment.Start <= End)
                   || (Start <= assignment.End && assignment.End <= End);
        }
    }
}
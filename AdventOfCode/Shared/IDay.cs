namespace AdventOfCode.Shared;

public interface IDay
{
    int Year { get; }
    int DayNumber { get; }
    string ValidatedPart1 { get; }
    string ValidatedPart2 { get; }

    void Initialise();
    string Part1();
    string Part2();
}
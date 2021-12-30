namespace AdventOfCode.Shared
{
    public interface IDay
    {
        int Year { get; }
        int DayNumber { get; }
        string Part1();
        string Part2();
        string ValidatedPart1 { get; }
        string ValidatedPart2 { get; }
    }
}
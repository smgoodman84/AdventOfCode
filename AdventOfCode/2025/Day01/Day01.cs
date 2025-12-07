using AdventOfCode.Shared;

namespace AdventOfCode._2025.Day01;

public class Day01 : Day
{
    public Day01() : base(2025, 1, "Day01/input_2025_01.txt", "1132", "6623", false)
    {

    }

    public override string Part1()
    {
        var timesAtZero = 0;
        var position = 50;
        foreach (var instruction in InputLines)
        {
            var distance = int.Parse(instruction.Substring(1));
            if (instruction[0] == 'L')
            {
                distance *= -1;
            }
            position += distance;
            position = position % 100;

            if (position == 0)
            {
                timesAtZero += 1;
            }
        }
        
        return timesAtZero.ToString();
    }

    public override string Part2()
    {
        var timesAtZero = 0;
        var position = 50;
        foreach (var instruction in InputLines)
        {
            TraceLine($"Processing {instruction}");
            TraceLine($"1. Times at Zero: {timesAtZero}, Position: {position}");
            var distance = int.Parse(instruction.Substring(1));
            timesAtZero += distance / 100;
            distance = distance % 100;

            TraceLine($"2. Times at Zero: {timesAtZero}, Position: {position}");
            var startPosition = position;
            if (instruction[0] == 'L')
            {
                position += 100 - distance;
                position = position % 100;

                if (position == 0 
                    || (position > startPosition && startPosition != 0))
                {
                    timesAtZero += 1;
                }
            }
            else // (instruction[0] == 'R')
            {
                position += distance;
                position = position % 100;

                if (position < startPosition)
                {
                    timesAtZero += 1;
                }
            }
            TraceLine($"3. Times at Zero: {timesAtZero}, Position: {position}");
        }
        
        return timesAtZero.ToString();
    }
}
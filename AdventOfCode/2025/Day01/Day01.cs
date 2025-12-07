using AdventOfCode.Shared;

namespace AdventOfCode._2025.Day01;

public class Day01 : Day
{
    public Day01() : base(2025, 1, "Day01/input_2025_01.txt", "1132", "")
    {

    }

    public override void Initialise()
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
        return string.Empty;
    }
}
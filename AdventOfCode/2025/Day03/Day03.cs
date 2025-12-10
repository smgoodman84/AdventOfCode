using AdventOfCode.Shared;

namespace AdventOfCode._2025.Day03;

public class Day03 : Day
{
    public Day03() : base(2025, 3, "Day03/input_2025_03.txt", "16858", "", false)
    {

    }

    public override string Part1()
    {
        var total = 0;
        foreach (var bank in InputLines)
        {
            total += MaxJoltage(bank);
        }

        return total.ToString();
    }

    private int MaxJoltage(string bank)
    {
        var digits = bank.Select(c => int.Parse(c.ToString())).ToList();
        var firstDigit = -1;
        var firstDigitIndex = -1;
        for (var i = 0; i < digits.Count - 1; i++)
        {
            if (digits[i] > firstDigit)
            {
                firstDigit = digits[i];
                firstDigitIndex = i;
            }
        }
        
        var secondDigit = -1;
        for (var i = firstDigitIndex + 1; i < digits.Count; i++)
        {
            if (digits[i] > secondDigit)
            {
                secondDigit = digits[i];
            }
        }

        return firstDigit * 10 + secondDigit;
    }

    public override string Part2()
    {
        return String.Empty;
    }
}
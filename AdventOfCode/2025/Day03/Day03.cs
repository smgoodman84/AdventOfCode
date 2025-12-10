using AdventOfCode.Shared;

namespace AdventOfCode._2025.Day03;

public class Day03 : Day
{
    public Day03() : base(2025, 3, "Day03/input_2025_03.txt", "16858", "167549941654721", false)
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
        long total = 0;
        foreach (var bank in InputLines)
        {
            var joltage = MaxJoltage2(bank);
            TraceLine($"{bank} - {joltage}");
            total += joltage;
        }

        return total.ToString();
    }

    private long MaxJoltage2(string bank)
    {
        var digits = bank.Select(c => int.Parse(c.ToString())).ToList();
        return MaxJoltage2(digits, 12);
    }

    private long MaxJoltage2(List<int> digits, int batteryCount)
    {
        if (batteryCount == 0)
        {
            return 0;
        }
        
        var thisDigit = -1;
        var thisDigitIndex = -1;
        for (var i = 0; i <= digits.Count - batteryCount; i++)
        {
            if (digits[i] > thisDigit)
            {
                thisDigit = digits[i];
                thisDigitIndex = i;
            }
        }

        long thisJoltage = thisDigit;
        for (var i = 1; i < batteryCount; i++)
        {
            thisJoltage *= 10;
        }
        
        var additionalJoltage = MaxJoltage2(digits.Skip(thisDigitIndex + 1).ToList(), batteryCount - 1);

        return thisJoltage + additionalJoltage;
    }
}
using AdventOfCode.Shared;

namespace AdventOfCode._2024.Day07;

public class Day07 : Day
{
    public Day07() : base(2024, 7, "Day07/input_2024_07.txt", "1545311493300", "169122112716571")
    {

    }

    private List<Calibration> _calibrations = new List<Calibration>();

    public override void Initialise()
    {
        _calibrations = InputLines
            .Select(l => new Calibration(l))
            .ToList();
    }

    public override string Part1()
    {
        var validCalibrations = _calibrations
            .Where(c => c.IsValid())
            .ToList();

        var result = validCalibrations.Sum(c => c.Result);

        return result.ToString();
    }

    public override string Part2()
    {
        var validCalibrations = _calibrations
            .Where(c => c.IsValid(true))
            .ToList();

        var result = validCalibrations.Sum(c => c.Result);

        return result.ToString();
    }

    private class Calibration
    {
        public long Result { get; set; }
        public List<long> Operands { get; set; }
        public Calibration(string calibration)
        {
            var split = calibration
                .Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            
            Result = long.Parse(split[0]);
            Operands = split[1].Split(' ').Select(long.Parse).ToList();
        }

        public bool IsValid(bool includeConcatenation = false)
        {
            var reversed = Operands.ToList();
            reversed.Reverse();

            var possibleValues = GetPossibleValues(reversed, includeConcatenation);

            return possibleValues.Contains(Result);
        }

        private List<long> GetPossibleValues(List<long> operands, bool includeConcatenation)
        {
            if (operands.Count == 1)
            {
                return operands;
            }

            var head = operands.First();
            var tail = operands.Skip(1).ToList();
            var result = new List<long>();

            foreach (var possibleValue in GetPossibleValues(tail, includeConcatenation))
            {
                result.Add(head + possibleValue);
                result.Add(head * possibleValue);

                if (includeConcatenation)
                {
                    // backwards beause we reversed the list
                    result.Add(long.Parse($"{possibleValue}{head}"));
                }
            }

            return result;
        }
    }
}
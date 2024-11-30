using AdventOfCode.Shared;

namespace AdventOfCode._2023.Day09;

public class Day09 : Day
{
    public Day09() : base(2023, 9, "Day09/input_2023_09.txt", "1696140818", "1152")
    {

    }

    private List<Sequence> _sequences;
    public override void Initialise()
    {
        _sequences = InputLines
            .Select(l => new Sequence(l))
            .ToList();
    }

    public override string Part1()
    {
        var extrapolated = _sequences
            .Select(s => s.Extrapolate())
            .ToList();

        var sum = extrapolated
            .Sum(x => x.Values.Last());

        return sum.ToString();
    }

    public override string Part2()
    {
        var extrapolated = _sequences
            .Select(s => s.ExtrapolateBackwards())
            .ToList();

        var sum = extrapolated
            .Sum(x => x.Values.First());

        return sum.ToString();
    }

    private class Sequence
    {
        public List<int> Values;
        public Sequence(string description)
        {
            Values = description
                .Split(" ")
                .Select(int.Parse)
                .ToList();
        }

        public Sequence(IEnumerable<int> values)
        {
            // Console.WriteLine(string.Join(" ", values));
            Values = values.ToList();
        }

        public Sequence GetDeltas()
        {
            var values = new List<int>(Values.Count - 1);
            for (var index = 0; index < Values.Count - 1; index += 1)
            {
                values.Add(Values[index + 1] - Values[index]);
            }
            return new Sequence(values);
        }

        public Sequence Extrapolate()
        {
            var deltas = GetDeltas();
            if (deltas.Values.All(x => x == 0))
            {
                return new Sequence(Values.Append(Values[0]));
            }

            var extrapolatedDeltas = deltas.Extrapolate();
            var newElement = extrapolatedDeltas.Values.Last() + Values.Last();

            return new Sequence(Values.Append(newElement));
        }

        public Sequence ExtrapolateBackwards()
        {
            var deltas = GetDeltas();
            if (deltas.Values.All(x => x == 0))
            {
                return new Sequence(Values.Append(Values[0]));
            }

            var extrapolatedDeltas = deltas.ExtrapolateBackwards();
            var newElement = Values.First() - extrapolatedDeltas.Values.First();

            return new Sequence(new[] { newElement }.Concat(Values));
        }
    }
}
using AdventOfCode.Shared;

namespace AdventOfCode._2023.Day15;

public class Day15 : Day
{
    public Day15() : base(2023, 15, "Day15/input_2023_15.txt", "507666", "233537", false)
    {

    }

    private List<string> _instructions;
    public override void Initialise()
    {
        _instructions = InputLines[0].Split(",").ToList();
    }

    public override string Part1()
    {
        var hashes = _instructions.Select(Hash).ToList();
        var sum = hashes.Sum();
        return sum.ToString();
    }

    public override string Part2()
    {
        var instructions = _instructions
            .Select(x => new Instruction(x))
            .ToList();

        var boxes = Enumerable.Range(0, 256)
            .Select(x => new Box(x))
            .ToArray();

        foreach (var instruction in instructions)
        {
            var box = boxes[instruction.Box];
            if (instruction.Operation == Operation.Remove)
            {
                box.RemoveLens(instruction.Label);
            }
            else
            {
                box.PlaceLens(instruction.Label, instruction.Lens.Value);
            }
        }

        var result = boxes.Sum(b => b.GetFocusingPower());

        return result.ToString();
    }

    private static int Hash(string input)
    {
        var currentValue = 0;
        foreach (var c in input)
        {
            var ascii = Ascii(c);
            currentValue += ascii;
            currentValue *= 17;
            currentValue %= 256;
        }
        return currentValue;
    }

    private static int Ascii(char c)
    {
        return (int)c;
    }

    private class Box
    {
        public int BoxNumber { get; }
        private List<LabelledLens> _lenses;

        public Box(int boxNumber)
        {
            BoxNumber = boxNumber;
            _lenses = new List<LabelledLens>();
        }

        public void RemoveLens(string label)
        {
            _lenses.RemoveAll(x => x.Label == label);
        }

        public void PlaceLens(string label, int lens)
        {
            var currentLens = _lenses.FirstOrDefault(x => x.Label == label);
            if (currentLens != default)
            {
                currentLens.Lens = lens;
            }
            else
            {
                _lenses.Add(new LabelledLens(label, lens));
            }
        }

        public int GetFocusingPower()
        {
            var sum = 0;
            var lensPosition = 1;
            foreach(var lens in _lenses)
            {
                var lensPower = (BoxNumber + 1) * lensPosition * lens.Lens;
                sum += lensPower;

                lensPosition += 1;
            }

            return sum;
        }

        public override string ToString()
        {
            var lenses = string.Join(" ", _lenses.Select(l => l.ToString()));
            return $"Box {BoxNumber}: {lenses}";
        }
    }

    private class LabelledLens
    {
        public string Label { get; }
        public int Lens { get; set; }
        public LabelledLens(string label, int lens)
        {
            Label = label;
            Lens = lens;
        }

        public override string ToString()
        {
            return $"[{Label} {Lens}]";
        }
    }

    private class Instruction
    {
        public string Label { get; }
        public int Box { get; }
        public Operation Operation { get; }
        public int? Lens { get; }
        public Instruction(string instruction)
        {
            if (instruction.EndsWith('-'))
            {
                Operation = Operation.Remove;
                Label = instruction.Replace("-", "");
                Box = Hash(Label);
            }
            else
            {
                var split = instruction.Split("=");
                Operation = Operation.Place;
                Label = split[0];
                Box = Hash(Label);
                Lens = int.Parse(split[1]);
            }
        }

        public override string ToString()
        {
            return $"{Operation} {Box} {Lens} {Label}";
        }
    }

    private enum Operation
    {
        Remove,
        Place
    }
}
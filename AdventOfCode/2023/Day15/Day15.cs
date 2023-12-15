using AdventOfCode.Shared;

namespace AdventOfCode._2023.Day15
{
    public class Day15 : Day
    {
        public Day15() : base(2023, 15, "Day15/input_2023_15.txt", "507666", "", false)
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
            return string.Empty;
        }

        private int Hash(string input)
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

        private int Ascii(char c)
        {
            return (int)c;
        }
    }
}

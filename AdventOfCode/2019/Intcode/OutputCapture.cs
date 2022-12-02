using System.Collections.Generic;

namespace AdventOfCode._2019.Intcode
{
    public class OutputCapture : IOutput
    {
        public List<long> OutputValues { get; } = new List<long>();

        public void Output(long output)
        {
            OutputValues.Add(output);
        }
    }
}

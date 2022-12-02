using System;

namespace AdventOfCode._2019.Intcode
{
    public class ConsoleOutput : IOutput
    {
        public void Output(long output)
        {
            Console.WriteLine($"Output: {output}");
        }
    }
}

using System;
using System.Threading.Tasks;

namespace AdventOfCode._2019.Intcode;

public class ConsoleInput : IInput
{
    public Task<long> ReadInput()
    {
        string inputString;
        long input;
        do
        {
            Console.Write("Input: ");
            inputString = Console.ReadLine();
        } while (!long.TryParse(inputString, out input));

        return Task.FromResult(input);
    }
}
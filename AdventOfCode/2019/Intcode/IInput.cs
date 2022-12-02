using System.Threading.Tasks;

namespace AdventOfCode._2019.Intcode
{
    public interface IInput
    {
        Task<long> ReadInput();
    }
}

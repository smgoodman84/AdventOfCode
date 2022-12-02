using System.Threading.Tasks;

namespace AdventOfCode._2019.Intcode
{
    public class CombinedInput : IInput
    {
        private IInput[] _inputs;
        int index = 0;

        public CombinedInput(params IInput[] inputs)
        {
            _inputs = inputs;
        }

        public Task<long> ReadInput()
        {
            var result = _inputs[index].ReadInput();

            index += 1;

            return result;
        }
    }
}

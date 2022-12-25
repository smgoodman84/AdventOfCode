using System.Collections.Generic;

namespace AdventOfCode.Shared.Algorithms
{
    public class RunLength<T>
    {
        public T Element { get; }
        public int Length { get; }

        public RunLength(T element, int length)
        {
            Element = element;
            Length = length;
        }

        public override string ToString()
        {
            return $"{Length}: {Element}";
        }

        public static IEnumerable<RunLength<TElement>> GetRunLengths<TElement>(IEnumerable<TElement> input)
        {
            var currentLength = 0;
            TElement current = default;
            foreach (var c in input)
            {
                if (currentLength == 0)
                {
                    current = c;
                    currentLength = 1;
                }
                else
                {
                    if (c.Equals(current))
                    {
                        currentLength += 1;
                    }
                    else
                    {
                        yield return new RunLength<TElement>(current, currentLength);

                        current = c;
                        currentLength = 1;
                    }
                }
            }

            yield return new RunLength<TElement>(current, currentLength);
        }
    }
}

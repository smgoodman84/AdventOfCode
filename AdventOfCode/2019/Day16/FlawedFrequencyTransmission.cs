using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace AdventOfCode2019.Day16
{
    class FlawedFrequencyTransmission
    {
        public static FlawedFrequencyTransmission LoadFromFile(string filename)
        {
            var signal = File.ReadAllText(filename)
                .ToCharArray()
                .Select(c => int.Parse(c.ToString()));

            return new FlawedFrequencyTransmission(signal);
        }

        private readonly List<int[]> _signal = new List<int[]>();
        private int _signalLength;
        private int[] _originalSignal;

        public FlawedFrequencyTransmission(IEnumerable<int> signal)
        {
            _originalSignal = signal.ToArray();
            _signalLength = _originalSignal.Length;
            _signal.Add(_originalSignal);
        }

        public string ProcessSignal(int phases)
        {
            for (var phase = 0; phase < phases; phase++)
            {
                var phaseOutput = new int[_signalLength];
                for (var row = 0; row < _signalLength; row++)
                {
                    var input = _signal[phase];
                    var pattern = PatternForOutputElement(row);

                    var index = 0;
                    var total = 0;
                    foreach(var patternElement in pattern)
                    {
                        //Console.Write($"{input[index]}*{patternElement} + ");
                        total += input[index] * patternElement;
                        index += 1;

                        if (index >= input.Length)
                        {
                            break;
                        }
                    }
                    phaseOutput[row] = Math.Abs(total) % 10;
                    //Console.WriteLine($" = {total} = {phaseOutput[row]}");
                    //Console.WriteLine(string.Join(",", pattern));
                }
                _signal.Add(phaseOutput);
            }

            return string.Join("", _signal[phases].Take(8));
        }

        public string ProcessRepeatedSignal(int phases, int repetition)
        {
            var skipLength = int.Parse(string.Join("", _originalSignal.Take(7)));

            var result = GetInputForPhase(phases, repetition).Skip(skipLength).Take(8);

            return string.Join("", result);
        }

        public IEnumerable<int> GetInputForPhase(int phase, int repetition)
        {
            Console.WriteLine($"Starting input for Phase {phase}");
            if (phase == 0)
            {
                for (var i = 0; i < _originalSignal.Length * repetition; i++)
                {
                    yield return _originalSignal[i % _originalSignal.Length];
                }
            }
            else
            {
                //var inputList = GetInputForPhase(phase - 1, repetition).ToList();
                for (var row = 0; row < _originalSignal.Length * repetition; row++)
                {
                    var input = GetInputForPhase(phase - 1, repetition).GetEnumerator();
                    var pattern = PatternForOutputElement(row).GetEnumerator();

                    var index = 0;
                    var total = 0;

                    while (index < _originalSignal.Length * repetition)
                    {
                        total += input.Current * pattern.Current;

                        input.MoveNext();
                        pattern.MoveNext();

                        index += 1;
                    }
                    yield return Math.Abs(total) % 10;
                }
            }
            Console.WriteLine($"Completed input for Phase {phase}");
        }

        private IEnumerable<int> PatternForOutputElement(int elementNumber)
        {
            var basePattern = new[] { 0, 1, 0, -1 };
            for (var index = 1; true; index++)
            {
                yield return basePattern[(index / (elementNumber + 1)) % 4];
            }
        }
    }
}

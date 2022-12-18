using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;

namespace AdventOfCode._2022.Day11
{
    public class Day11 : Day
    {
        public Day11() : base(2022, 11, "Day11/input_2022_11.txt", "54054", "14314925001")
        {

        }

        private Monkeys CreateMonkeys(Func<int, IEnumerable<int>, IItem> itemFactory)
        {
            var result = new Monkeys();

            var monkeys = LineGrouper.GroupLinesBySeperator(InputLines)
                .ToList();

            var divisors = new List<int>();
            foreach (var monkey in monkeys)
            {
                var divisibleTest = int.Parse(monkey[3]
                    .Trim()
                    .Replace("Test: divisible by ", ""));

                divisors.Add(divisibleTest);
            }

            foreach (var monkey in monkeys)
            {
                var id = int.Parse(monkey[0]
                    .Trim()
                    .Replace("Monkey ", "")
                    .Replace(":", ""));

                var items = monkey[1]
                    .Trim()
                    .Replace("Starting items: ", "")
                    .Split(",")
                    .Select(x => itemFactory(int.Parse(x.Trim()), divisors))
                    .ToList();

                var operation = new Operation(monkey[2]
                    .Trim()
                    .Replace("Operation: new = ", ""));
                    

                var divisibleTest = int.Parse(monkey[3]
                    .Trim()
                    .Replace("Test: divisible by ", ""));

                var trueDestination = int.Parse(monkey[4]
                    .Trim()
                    .Replace("If true: throw to monkey ", ""));

                var falseDestination = int.Parse(monkey[5]
                    .Trim()
                    .Replace("If false: throw to monkey ", ""));

                result.AddMonkey(new Monkey(
                    id,
                    items,
                    operation,
                    divisibleTest,
                    trueDestination,
                    falseDestination
                    ));
            }

            return result;
        }

        public override string Part1()
        {
            var topTwo = CreateMonkeys((x, divisors) => new LittleItem(x))
                .RunRounds(20, true)
                .GetTopTwoMonkeys()
                .ToArray();

            var result = topTwo[0].MonkeyBusiness * topTwo[1].MonkeyBusiness;

            return result.ToString();
        }

        public override string Part2()
        {
            var topTwo = CreateMonkeys((x, divisors) => new BigItem(x, divisors))
                .RunRounds(10000, false)
                .GetTopTwoMonkeys()
                .ToArray();

            var result = topTwo[0].MonkeyBusiness * topTwo[1].MonkeyBusiness;

            return result.ToString();
        }

        private class Monkeys
        {
            private readonly List<Monkey> _monkeys = new List<Monkey>();
            private readonly Dictionary<int, Monkey> _monkeyDictionary = new Dictionary<int, Monkey>();

            public IEnumerable<Monkey> GetTopTwoMonkeys() => _monkeys
                .OrderByDescending(m => m.MonkeyBusiness)
                .Take(2);

            public void AddMonkey(Monkey monkey)
            {
                _monkeys.Add(monkey);
                _monkeyDictionary.Add(monkey.Id, monkey);
            }

            public Monkeys RunRounds(int count, bool divideByThree)
            {
                var printAt = new int[] { 1, 20, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000 };
                var iteration = 0;
                while (iteration < count)
                {
                    RunRound(divideByThree);
                    iteration += 1;

                    if (printAt.Contains(iteration))
                    {
                        var message = string.Join(", ", _monkeys.Select(m => m.MonkeyBusiness));
                        // Console.WriteLine($"{iteration}: {message}");
                    }
                }

                return this;
            }

            private void RunRound(bool divideByThree)
            {
                foreach (var monkey in _monkeys)
                {
                    foreach (var item in monkey.Items)
                    {
                        monkey.Operation.ApplyOperation(item);
                        if (divideByThree)
                        {
                            item.Divide(3);
                        }

                        if (item.IsDivisbleBy(monkey.DividibleTest))
                        {
                            _monkeyDictionary[monkey.TrueDestination].Items.Add(item);
                        }
                        else
                        {
                            _monkeyDictionary[monkey.FalseDestination].Items.Add(item);
                        }

                        monkey.MonkeyBusiness += 1;
                    }

                    monkey.Items.Clear();
                }
            }
        }

        private class Monkey
        {
            public int Id { get; }
            public List<IItem> Items { get; }
            public Operation Operation { get; }
            public int DividibleTest { get; }
            public int TrueDestination { get; }
            public int FalseDestination { get; }

            public long MonkeyBusiness { get; set; } = 0;

            public Monkey(
                int id,
                List<IItem> items,
                Operation operation,
                int dividibleTest,
                int trueDestination,
                int falseDestination)
            {
                Id = id;
                Items = items;
                Operation = operation;
                DividibleTest = dividibleTest;
                TrueDestination = trueDestination;
                FalseDestination = falseDestination;
            }
        }

        private class Operation
        {
            private string _left;
            private string _operator;
            private string _right;

            public Operation(string operation)
            {
                var split = operation.Split(" ");
                _left = split[0].Trim();
                _operator = split[1].Trim();
                _right = split[2].Trim();
            }

            public void ApplyOperation(IItem value)
            {
                if (_operator == "+")
                {
                    if (_left == "old")
                    {
                        if (_right == "old")
                        {
                            value.AddSelf();
                        }
                        else
                        {
                            value.Add(int.Parse(_right));
                        }
                    }
                    else
                    {
                        if (_right == "old")
                        {
                            value.Add(int.Parse(_left));
                        }
                        else
                        {
                            // shouldn't happen
                        }
                    }
                }

                if (_operator == "*")
                {
                    if (_left == "old")
                    {
                        if (_right == "old")
                        {
                            value.MultiplySelf();
                        }
                        else
                        {
                            value.Multiply(int.Parse(_right));
                        }
                    }
                    else
                    {
                        if (_right == "old")
                        {
                            value.Multiply(int.Parse(_left));
                        }
                        else
                        {
                            // shouldn't happen
                        }
                    }
                }
            }
        }

        private interface IItem
        {
            void Add(int value);
            void Multiply(int value);
            void Divide(int value);
            void AddSelf();
            void MultiplySelf();
            bool IsDivisbleBy(int divisor);
        }

        private class LittleItem : IItem
        {
            private int _value;

            public LittleItem(int value)
            {
                _value = value;
            }

            public void Add(int value)
            {
                _value += value;
            }

            public void AddSelf()
            {
                _value += _value;
            }

            public void Divide(int value)
            {
                _value /= value;
            }

            public bool IsDivisbleBy(int divisor)
            {
                return _value % divisor == 0;
            }

            public void Multiply(int value)
            {
                _value *= value;
            }

            public void MultiplySelf()
            {
                _value *= _value;
            }
        }

        private class BigItem : IItem
        {
            private Dictionary<int, DivisibleBy> _divisibleBy = new Dictionary<int, DivisibleBy>();

            public BigItem(int value, IEnumerable<int> divisors)
            {
                foreach(var divisor in divisors)
                {
                    var thisDivisibleBy = new DivisibleBy(divisor);
                    thisDivisibleBy.Add(value);

                    _divisibleBy.Add(divisor, thisDivisibleBy);
                }
            }

            public void Add(int value)
            {
                foreach(var divisibleBy in _divisibleBy.Values)
                {
                    divisibleBy.Add(value);
                }
            }

            public void Multiply(int value)
            {
                foreach (var divisibleBy in _divisibleBy.Values)
                {
                    divisibleBy.Multiply(value);
                }
            }

            public void Divide(int value)
            {
                foreach (var divisibleBy in _divisibleBy.Values)
                {
                    divisibleBy.Divide(value);
                }
            }

            public void AddSelf()
            {
                foreach (var divisibleBy in _divisibleBy.Values)
                {
                    divisibleBy.AddSelf();
                }
            }

            public void MultiplySelf()
            {
                foreach (var divisibleBy in _divisibleBy.Values)
                {
                    divisibleBy.MultiplySelf();
                }
            }

            public bool IsDivisbleBy(int divisor)
            {
                return _divisibleBy[divisor].IsDivisible();
            }
        }

        private class DivisibleBy
        {
            private readonly int _divisor;
            private int _value;

            public DivisibleBy(int divisor)
            {
                _divisor = divisor;
            }

            public void Add(int value)
            {
                _value = (_value + value) % _divisor;
            }

            public void Multiply(int value)
            {
                _value = (_value * value) % _divisor;
            }


            public void Divide(int value)
            {
                _value = (_value / value) % _divisor;
            }

            public void AddSelf()
            {
                _value = (_value + _value) % _divisor;
            }

            public void MultiplySelf()
            {
                _value = (_value * _value) % _divisor;
            }

            public bool IsDivisible() => _value == 0;
        }
    }
}


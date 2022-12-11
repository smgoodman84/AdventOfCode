using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.PrimitiveExtensions;

namespace AdventOfCode._2022.Day11
{
    public class Day11 : Day
    {
        public Day11() : base(2022, 11, "Day11/input_2022_11.txt", "54054", "")
        {

        }

        private Monkeys _monkeys = new Monkeys();
        public override void Initialise()
        {
            var monkeys = LineGrouper.GroupLinesBySeperator(InputLines)
                .ToList();

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
                    .Select(x => new Item(int.Parse(x.Trim())))
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

                _monkeys.AddMonkey(new Monkey(
                    id,
                    items,
                    operation,
                    divisibleTest,
                    trueDestination,
                    falseDestination
                    ));
            }
        }

        public override string Part1()
        {
            var topTwo = _monkeys
                .RunRounds(20)
                .GetTopTwoMonkeys()
                .ToArray();

            var result = topTwo[0].MonkeyBusiness * topTwo[1].MonkeyBusiness;

            return result.ToString();
        }

        public override string Part2()
        {
            return "";
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

            public Monkeys RunRounds(int count)
            {
                while (count > 0)
                {
                    RunRound();
                    count -= 1;
                }

                return this;
            }

            private void RunRound()
            {
                foreach (var monkey in _monkeys)
                {
                    foreach (var item in monkey.Items)
                    {
                        item.WorryLevel = monkey.Operation.ApplyOperation(item.WorryLevel);
                        item.WorryLevel /= 3;

                        if (item.WorryLevel % monkey.DividibleTest == 0)
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
            public List<Item> Items { get; }
            public Operation Operation { get; }
            public int DividibleTest { get; }
            public int TrueDestination { get; }
            public int FalseDestination { get; }

            public int MonkeyBusiness { get; set; } = 0;

            public Monkey(
                int id,
                List<Item> items,
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

            public int ApplyOperation(int oldValue)
            {
                var left = Read(_left, oldValue);
                var right = Read(_right, oldValue);

                switch (_operator)
                {
                    case "+": return left + right;
                    case "*": return left * right;
                }

                throw new Exception($"Unkown Operator {_operator}");
            }

            private int Read(string value, int oldValue)
            {
                if (value == "old")
                {
                    return oldValue;
                }

                return int.Parse(value);
            }
        }

        private class Item
        {
            public int WorryLevel { get; set; }

            public Item(int worryLevel)
            {
                WorryLevel = worryLevel;
            }
        }
    }
}


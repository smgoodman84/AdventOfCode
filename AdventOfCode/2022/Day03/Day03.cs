using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;

namespace AdventOfCode._2022.Day02
{
    public class Day03 : Day
    {
        public Day03() : base(2022, 3, "Day03/input_2022_03.txt", "", "")
        {

        }

        private List<Rucksack> _rucksacks;
        public override void Initialise()
        {
            _rucksacks = InputLines
                .Select(line => new Rucksack(line))
                .ToList();
        }

        public override string Part1()
        {
            return _rucksacks
                .Select(r => r.GetPriorityOfCommonItem())
                .Sum()
                .ToString();
        }

        public override string Part2()
        {
            //var allItems = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

            var groups = new List<List<string>>();

            var currentGroup = new List<string>();
            foreach (var line in InputLines)
            {
                currentGroup.Add(line);
                if (currentGroup.Count == 3)
                {
                    groups.Add(currentGroup);
                    currentGroup = new List<string>();
                }
            }

            if (currentGroup.Count == 3)
            {
                groups.Add(currentGroup);
                currentGroup = new List<string>();
            }

            var sum = 0;
            foreach (var group in groups)
            {
                var allItems = group.SelectMany(g => g.ToCharArray()).Distinct().ToList();

                var commonItem = allItems
                    .Where(item => group.All(x => x.Contains(item)))
                    .Single();

                var commonPriority = Rucksack.GetPriority(commonItem);
                sum += commonPriority;
            }

            return sum.ToString();
        }

        private class Rucksack
        {
            private string _contents;
            private string _pocketOne;
            private string _pocketTwo;
            public Rucksack(string contents)
            {
                _contents = contents;
                var length = contents.Length;

                var halfLength = length / 2;

                _pocketOne = contents.Substring(0, halfLength);
                _pocketTwo = contents.Substring(halfLength);
            }

            public int GetPriorityOfCommonItem()
            {
                var itemsOne = _pocketOne.ToCharArray();
                var itemsTwo = _pocketTwo.ToCharArray();

                var commonItem = itemsOne.First(itemsTwo.Contains);

                return GetPriority(commonItem);
            }

            public static int GetPriority(char c)
            {
                return "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(c) + 1;
            }
        }
    }
}


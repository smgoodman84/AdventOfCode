using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;
using AdventOfCode.Shared.PrimitiveExtensions;

namespace AdventOfCode._2022.Day02;

public class Day03 : Day
{
    public Day03() : base(2022, 3, "Day03/input_2022_03.txt", "8240", "2587")
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
        var sum = _rucksacks
            .GroupsOfSize(3)
            .Select(g => new RucksackGroup(g))
            .Select(g => g.GetCommonItemPriority())
            .Sum();

        return sum.ToString();
    }

    private class RucksackGroup
    {
        private readonly List<Rucksack> _rucksacks;

        public RucksackGroup(IEnumerable<Rucksack> rucksacks)
        {
            _rucksacks = rucksacks.ToList();
        }

        public int GetCommonItemPriority()
        {
            var allCommonPriorities = _rucksacks
                .SelectMany(g => g.GetItemPriorities())
                .Distinct()
                .ToList();

            var commonItemPriority = allCommonPriorities
                .Where(item => _rucksacks.All(x => x.ContainsItemWithPriority(item)))
                .Single();

            return commonItemPriority;
        }
    }

    private class Rucksack
    {
        private readonly List<int> _contentPriorities;
        private readonly List<int> _pocketOnePriorities;
        private readonly List<int> _pocketTwoPriorities;

        public Rucksack(string contents)
        {
            _contentPriorities = contents
                .Select(GetPriority)
                .ToList();

            var halfLength = contents.Length / 2;

            _pocketOnePriorities = contents
                .Substring(0, halfLength)
                .Select(GetPriority)
                .ToList();

            _pocketTwoPriorities = contents
                .Substring(halfLength)
                .Select(GetPriority)
                .ToList();
        }

        public int GetPriorityOfCommonItem()
        {
            var commonItem = _pocketOnePriorities.First(_pocketTwoPriorities.Contains);

            return commonItem;
        }

        public IEnumerable<int> GetItemPriorities()
        {
            return _contentPriorities;
        }

        public bool ContainsItemWithPriority(int priority)
        {
            return _contentPriorities.Contains(priority);
        }

        private static int GetPriority(char c)
        {
            return "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(c) + 1;
        }
    }
}
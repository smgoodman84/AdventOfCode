using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode2020.Day07;

public class Day07 : Day
{
    public Day07() : base(2020, 7, "Day07/input_2020_07.txt", "316", "11310")
    {

    }

    private Dictionary<string, LuggageRule> _luggageRules;
    private Dictionary<string, int> _containCount = new Dictionary<string, int>();
    private Dictionary<string, List<string>> _canGoIn;

    public override void Initialise()
    {
        _luggageRules = InputLines
            .Select(ParseLine)
            .ToDictionary(lr => lr.Colour, lr => lr);

        _canGoIn = _luggageRules
            .SelectMany(lr => lr.Value.MustContain.Keys
                .Select(contain => new KeyValuePair<string, string>(contain, lr.Key)))
            .GroupBy(kvp => kvp.Key)
            .ToDictionary(g => g.Key, g => g.Select(kvp => kvp.Value).ToList());
    }

    private static LuggageRule ParseLine(string luggageRule)
    {
        return new LuggageRule(luggageRule);
    }

    public override string Part1()
    {
        var shinyGoldCanGoIn = new HashSet<string>();
        var newColours = _canGoIn["shiny gold"];

        while (newColours.Any())
        {
            newColours.ForEach(c => shinyGoldCanGoIn.Add(c));

            newColours = GetCanGoIn(newColours)
                .Where(c => !shinyGoldCanGoIn.Contains(c))
                .ToList();
        }

        return shinyGoldCanGoIn.Count().ToString();
    }

    public IEnumerable<string> GetCanGoIn(IEnumerable<string> colours)
    {
        var result = colours
            .SelectMany(c => _canGoIn.ContainsKey(c) ? _canGoIn[c] : new List<string>())
            .Distinct();

        return result;
    }

    public override string Part2()
    {
        var result = GetContainCount("shiny gold");

        return result.ToString();
    }

    private int GetContainCount(string colour)
    {
        var luggageRule = _luggageRules[colour];

        var total = 0;

        foreach (var bag in luggageRule.MustContain)
        {
            total += bag.Value * (GetCachedContainCount(bag.Key) + 1);
        }

        return total;
    }

    private int GetCachedContainCount(string colour)
    {
        if (!_containCount.ContainsKey(colour))
        {
            _containCount[colour] = GetContainCount(colour);
        }

        return _containCount[colour];
    }

    private class LuggageRule
    {
        public Dictionary<string, int> MustContain { get; private set; }
        public string Colour { get; set; }

        public LuggageRule(string luggageRule)
        {
            var rule = luggageRule
                .Replace("bags", "bag")
                .Replace(".", string.Empty);

            var colourRule = rule.Split(" bag contain ");
            Colour = colourRule[0];

            if (colourRule[1] == "no other bag")
            {
                MustContain = new Dictionary<string, int>();
            }
            else
            {
                var rules = colourRule[1]
                    .Split(",")
                    .Select(r => r.Trim())
                    .Select(ParseRule)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                MustContain = rules;
            }
        }

        private KeyValuePair<string, int> ParseRule(string rule)
        {
            rule = rule.Replace("bag", string.Empty).Trim();

            var firstSpace = rule.IndexOf(' ');
            var countString = rule.Substring(0, firstSpace);
            var colour = rule.Substring(firstSpace + 1);
            return new KeyValuePair<string, int>(colour, int.Parse(countString));
        }
    }
}
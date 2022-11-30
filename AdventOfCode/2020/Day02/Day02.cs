using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2020.Day02
{
    public class Day02 : Day
    {
        private IEnumerable<PasswordWithPolicy> _passwordPolicies;


        public Day02() : base(2020, 2, "Day02/input_2020_02.txt", "564", "325")
        {
        }

        public override void Initialise()
        {
            _passwordPolicies = InputLines
                .Select(ParseLine)
                .ToList();
        }

        private static PasswordWithPolicy ParseLine(string line)
        {
            var policyPasswordSplit = line.Split(':');
            var policy = policyPasswordSplit[0].Trim();
            var password = policyPasswordSplit[1].Trim();

            var requiredCharacterSplit = policy.Split(' ');
            var minMax = requiredCharacterSplit[0].Trim();
            var requiredCharacter = requiredCharacterSplit[1].Trim()[0];

            var minMaxSplit = minMax.Split('-');
            var min = int.Parse(minMaxSplit[0].Trim());
            var max = int.Parse(minMaxSplit[1].Trim());

            return new PasswordWithPolicy
            {
                Password = password,
                RequiredCharacter = requiredCharacter,
                MinOccurences = min,
                MaxOccurences = max
            };
        }

        public override string Part1()
        {
            return _passwordPolicies.Count(IsValid).ToString();
        }

        private bool IsValid(PasswordWithPolicy passwordWithPolicy)
        {
            var count = passwordWithPolicy.Password
                .Count(c => c == passwordWithPolicy.RequiredCharacter);

            return passwordWithPolicy.MinOccurences <= count
                && count <= passwordWithPolicy.MaxOccurences;
        }

        public override string Part2()
        {
            return _passwordPolicies.Count(IsValidPart2).ToString();
        }

        private bool IsValidPart2(PasswordWithPolicy passwordWithPolicy)
        {
            var passwordArray = passwordWithPolicy.Password.ToCharArray();
            var char1 = passwordArray[passwordWithPolicy.Position1 - 1];
            var char2 = passwordArray[passwordWithPolicy.Position2 - 1];

            var char1MatchCount = char1 == passwordWithPolicy.RequiredCharacter ? 1 : 0;
            var char2MatchCount = char2 == passwordWithPolicy.RequiredCharacter ? 1 : 0;

            return char1MatchCount + char2MatchCount == 1;
        }

        private class PasswordWithPolicy
        {
            public string Password { get; set; }
            public char RequiredCharacter { get; set; }
            public int MinOccurences { get; set; }
            public int MaxOccurences { get; set; }

            public int Position1 => MinOccurences;
            public int Position2 => MaxOccurences;
        }
    }
}

using System.Linq;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Cryptography;

namespace AdventOfCode._2015.Day04
{
    public class Day04 : Day
    {
        public Day04() : base(2015, 4, @"Day04/input_2015_04.txt", "282749", "9962624")
        {
        }

        public override string Part1() => FindNumberSuffixForHashPrefix(InputLines.Single(), "00000").ToString();
        public override string Part2() => FindNumberSuffixForHashPrefix(InputLines.Single(), "000000").ToString();

        private int FindNumberSuffixForHashPrefix(string prefix, string desiredHashPrefix)
        {
            var number = 0;

            while (true)
            {
                var md5 = MD5.GetMD5String($"{prefix}{number}");
                if (md5.StartsWith(desiredHashPrefix))
                {
                    return number;
                }

                number += 1;
            }
        }
    }
}
using System.Linq;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Cryptography;

namespace AdventOfCode._2015.Day04
{
    public class Day04 : Day
    {
        public Day04() : base(2015, 4, @"Day04/input.txt", "282749", "")
        {
        }

        public override string Part1()
        {
            var prefix = InputLines.Single();
            var number = 0;

            while (true)
            {
                var md5 = MD5.GetMD5String($"{prefix}{number}");
                if (md5.StartsWith("00000"))
                {
                    return number.ToString();
                }

                number += 1;
            }
        }

        public override string Part2() => "";

    }
}
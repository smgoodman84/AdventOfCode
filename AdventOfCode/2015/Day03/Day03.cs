using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2015.Day03
{
    public class Day03 : Day
    {
        public Day03() : base(2015, 3, @"Day03/input.txt", "2565", "2639")
        {
        }

        public override string Part1() => GetVisitsForSantas(1).ToString();
        public override string Part2() => GetVisitsForSantas(2).ToString();

        public int GetVisitsForSantas(int numberOfSantas)
        {
            var visitCounts = new Dictionary<string, int>();

            var santas = Enumerable.Range(1, 2)
                .Select(i => Coordinate2D.Origin)
                .ToArray();

            var santaIndex = 0;

            visitCounts.Add(Coordinate2D.Origin.ToString(), numberOfSantas);

            foreach (var c in InputLines.Single())
            {
                var x = santas[santaIndex].X;
                var y = santas[santaIndex].Y;

                switch (c)
                {
                    case '<':
                        x -= 1;
                        break;
                    case '>':
                        x += 1;
                        break;
                    case '^':
                        y += 1;
                        break;
                    case 'v':
                        y -= 1;
                        break;
                }

                santas[santaIndex] = new Coordinate2D(x, y);
                var positionKey = santas[santaIndex].ToString();
                if (visitCounts.ContainsKey(positionKey))
                {
                    visitCounts[positionKey] += 1;
                }
                else
                {
                    visitCounts.Add(positionKey, 1);
                }

                santaIndex += 1;
                if (santaIndex >= numberOfSantas)
                {
                    santaIndex = 0;
                }
            }

            return visitCounts.Count;
        }
    }
}
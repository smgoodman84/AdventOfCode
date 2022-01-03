using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2015.Day03
{
    public class Day03 : Day
    {
        public Day03() : base(2015, 3, @"Day03/input.txt", "", "")
        {
        }

        public override void Initialise()
        {
        }

        public override string Part1()
        {
            var visitCounts = new Dictionary<string, int>();
            var position = new Coordinate2D(0, 0);

            visitCounts.Add(position.ToString(), 1);

            foreach(var c in InputLines.Single())
            {
                var x = position.X;
                var y = position.Y;

                switch(c)
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

                position = new Coordinate2D(x, y);
                var positionKey = position.ToString();
                if (visitCounts.ContainsKey(positionKey))
                {
                    visitCounts[positionKey] += 1;
                }
                else
                {
                    visitCounts.Add(positionKey, 1);
                }
            }


            return visitCounts.Count.ToString();
        }

        public override string Part2()
        {
            return "";
        }
    }
}
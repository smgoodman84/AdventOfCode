using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;

namespace AdventOfCode2020.Day06
{
    public class Day06 : Day
    {
        public Day06() : base(2020, 6, "Day06/input_2020_06.txt", "6630", "3437")
        {

        }

        private List<GroupDecleration> _groupDeclerations;

        public override void Initialise()
        {
            _groupDeclerations = LineGrouper.GroupLines(InputLines)
                .Select(CreateGroupDecleration)
                .ToList();
        }

        private static GroupDecleration CreateGroupDecleration(List<string> lines)
        {
            var individualDeclerations = lines
                .Select(l => new IndividualDecleration
                {
                    YesAnswers = l.Trim().ToCharArray()
                })
                .ToList();

            var groupDecleration = new GroupDecleration
            {
                IndividualDeclerations = individualDeclerations
            };

            return groupDecleration;
        }

        public override string Part1()
        {
            return _groupDeclerations
                .Sum(gd => gd.GetUniqueYesCount())
                .ToString();
        }

        public override string Part2()
        {
            return _groupDeclerations
                .Sum(gd => gd.GetConsistentYesCount())
                .ToString();
        }

        private class GroupDecleration
        {
            public List<IndividualDecleration> IndividualDeclerations { get; set; }

            public int GetUniqueYesCount()
            {
                return IndividualDeclerations
                    .SelectMany(id => id.YesAnswers)
                    .Distinct()
                    .Count();
            }

            public int GetConsistentYesCount()
            {
                var uniqueAnswers = 
                    IndividualDeclerations
                    .SelectMany(id => id.YesAnswers)
                    .Distinct();

                var allYes = uniqueAnswers
                    .Where(a => IndividualDeclerations.All(d => d.YesAnswers.Contains(a)))
                    .ToList();

                return allYes.Count();
            }
        }

        private class IndividualDecleration
        {
            public char[] YesAnswers { get; set; }
        }
    }
}

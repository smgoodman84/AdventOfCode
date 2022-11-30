using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Shared.FileProcessing
{
	public static class LineGrouper
    {
        public static List<List<string>> GroupLines(IEnumerable<string> lines)
        {
            var groups = new List<List<string>>();
            var currentLines = new List<string>();
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    groups.Add(currentLines);
                    currentLines = new List<string>();
                }
                else
                {
                    currentLines.Add(line);
                }
            }

            if (currentLines.Any())
            {
                groups.Add(currentLines);
            }

            return groups;
        }
    }
}


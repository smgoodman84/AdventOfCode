using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Shared.FileProcessing
{
	public static class LineGrouper
    {
        public static List<List<string>> GroupLinesBySeperator(
            IEnumerable<string> lines,
            Func<string, bool> isSeparator = null
            )
        {
            isSeparator ??= x => string.IsNullOrWhiteSpace(x);

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


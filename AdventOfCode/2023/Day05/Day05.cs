using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;

namespace AdventOfCode._2023.Day05
{
    public class Day05 : Day
    {
        public Day05() : base(2023, 5, "Day05/input_2023_05.txt", "", "", false)
        {

        }

        private Dictionary<string, Map> _maps;
        private List<Item> _seeds;
        public override void Initialise()
        {
            _maps = new Dictionary<string, Map>();
            var seedLine = InputLines[0];
            _seeds = seedLine
                .Replace("seeds: ", "")
                .Split(" ")
                .Select(long.Parse)
                .Select(x => new Item
                {
                    Type = "seed",
                    Number = x
                })
                .ToList();

            var groups = LineGrouper.GroupLinesBySeperator(InputLines.Skip(2));
            foreach (var group in groups)
            {
                var titleLine = group[0].Replace(" map:", "");
                var titleSplit = titleLine.Split("-to-");
                var source = titleSplit[0];
                var destination = titleSplit[1];
                var rangeMaps = group.Skip(1).Select(x => new RangeMap(x)).ToList();
                _maps.Add(
                    source,
                    new Map
                    {
                        Source = source,
                        Destination = destination,
                        RangeMaps = rangeMaps
                    });
            }
        }

        public override string Part1()
        {
            var locations = new List<Item>();
            foreach (var seed in _seeds)
            {
                var currentItem = seed;
                TraceLine(currentItem.ToString());
                while (currentItem.Type != "location")
                {
                    var map = _maps[currentItem.Type];
                    map.TryMapItem(currentItem, out var nextItem);
                    currentItem = nextItem;
                    TraceLine(currentItem?.ToString());
                }
                locations.Add(currentItem);
            }

            var lowestLocation = locations.Min(l => l.Number);

            return lowestLocation.ToString();
        }

        public override string Part2()
        {
            return string.Empty;
        }

        private class Item
        {
            public string Type { get; set; }
            public long Number { get; set; }

            public override string ToString()
            {
                return $"{Type} {Number}";
            }
        }

        private class Map
        {
            public string Source { get; set; }
            public string Destination { get; set; }
            public List<RangeMap> RangeMaps { get; set; }

            public bool TryMapItem(Item sourceItem, out Item? destinationItem)
            {
                if (!sourceItem.Type.Equals(Source))
                {
                    destinationItem = null;
                    return false;
                }

                var mapToUse = RangeMaps
                    .Where(rm => rm.SourceStart <= sourceItem.Number)
                    .Where(rm => sourceItem.Number <= rm.SourceEnd)
                    .FirstOrDefault();

                destinationItem = new Item
                {
                    Type = Destination,
                    Number = sourceItem.Number + (mapToUse?.Offset ?? 0)
                };

                return true;
            }
        }

        private class RangeMap
        {
            public long SourceStart { get; set; }
            public long DestinationStart { get; set; }
            public long Range { get; set; }
            public long SourceEnd => SourceStart + Range - 1;
            public long DestinationEnd => DestinationStart + Range - 1;
            public long Offset => DestinationStart - SourceStart;

            public RangeMap(string description)
            {
                var split = description.Split(" ");
                DestinationStart = long.Parse(split[0]);
                SourceStart = long.Parse(split[1]);
                Range = long.Parse(split[2]);
            }
        }
    }
}

using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;
using AdventOfCode.Shared.Numbers;

namespace AdventOfCode._2023.Day05
{
    public class Day05 : Day
    {
        public Day05() : base(2023, 5, "Day05/input_2023_05.txt", "910845529", "77435348")
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
            var currentType = "seed";
            var currentRanges = new List<NumberRange>();
            for(var index = 0; index < _seeds.Count - 1; index += 2)
            {
                currentRanges.Add(NumberRange.FromLength(_seeds[index].Number, _seeds[index + 1].Number));
            }

            while (currentType != "location")
            {
                var map = _maps[currentType];

                currentRanges = currentRanges.SelectMany(r => ApplyMaps(r, map.RangeMaps)).ToList();

                currentType = map.Destination;
            }


            return currentRanges.Min(r => r.Start).ToString();
        }

        private List<NumberRange> ApplyMaps(NumberRange range, List<RangeMap> rangeMaps)
        {

            var mappedRanges = new List<NumberRange>();
            var stillUnmapped = new List<NumberRange> { range };
            foreach (var mapRange in rangeMaps)
            {
                stillUnmapped = stillUnmapped
                    .SelectMany(x => x.Except(mapRange.SourceRange))
                    .ToList();

                var toMap = range.Intersect(mapRange.SourceRange);

                var mapped = toMap
                    .Select(m => new NumberRange(m.Start + mapRange.Offset, m.End + mapRange.Offset))
                    .ToList();

                mappedRanges.AddRange(mapped);
            }
            mappedRanges.AddRange(stillUnmapped);

            return mappedRanges;
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
                    .Where(rm => rm.SourceRange.Contains(sourceItem.Number))
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
            public NumberRange SourceRange { get; set; }
            public NumberRange DestinationRange { get; set; }
            public long Offset => DestinationRange.Start - SourceRange.Start;

            public RangeMap(string description)
            {
                var split = description.Split(" ").Select(long.Parse).ToArray();
                DestinationRange = NumberRange.FromLength(split[0], split[2]);
                SourceRange = NumberRange.FromLength(split[1], split[2]);
            }

            public override string ToString()
            {
                return $"{SourceRange} -> {DestinationRange}";
            }
        }
    }
}

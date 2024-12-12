using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2024.Day12;

public class Day12 : Day
{
    public Day12() : base(2024, 12, "Day12/input_2024_12.txt", "1473276", "", true)
    {
    }

    private Grid2D<Plot> _map;
    private List<Plot> _regionHeads = new List<Plot>();

    public override void Initialise()
    {
        var height = InputLines.Count;
        var width = InputLines[0].Length;

        _map = new Grid2D<Plot>(width, height);
        
        var y = 0;
        foreach (var line in InputLines)
        {
            var x = 0;
            foreach (var c in line)
            {
                var plot = new Plot
                {
                    Location = new Coordinate2D(x, y),
                    PlotType = c,
                };
                _map.Write(x, y, plot);

                x += 1;
            }
            y += 1;
        }

        SetRegions();
        SetFenceCounts();
    }

    private void SetFenceCounts()
    {
        foreach (var plot in _map.ReadAll())
        {
            var fenceCount = 0;
            var neighbourLocations = plot.Location.Neighbours();

            foreach (var neighbourLocation in neighbourLocations)
            {
                if (!_map.IsInGrid(neighbourLocation))
                {
                    fenceCount += 1;
                    continue;
                }

                var neighbour = _map.Read(neighbourLocation);
                if (plot.RegionId != neighbour.RegionId)
                {
                    fenceCount += 1;
                }
            }

            plot.FenceCount = fenceCount;
        }
    }

    private void SetRegions(int regionId = 1)
    {
        var firstUnsetPlot = _map.ReadAll()
            .FirstOrDefault(x => !x.RegionId.HasValue);
        
        if (firstUnsetPlot == null)
        {
            return;
        }


        TraceLine($"Filling from {firstUnsetPlot.Location} as Region {regionId}");
        _regionHeads.Add(firstUnsetPlot);
        FillRegion(firstUnsetPlot.PlotType, firstUnsetPlot, regionId);
        SetRegions(regionId + 1);
    }

    private void FillRegion(char plotType, Plot plot, int regionId)
    {
        if (plot.RegionId.HasValue)
        {
            return;
        }

        if (plot.PlotType != plotType)
        {
            return;
        }

        // TraceLine($"Setting {plot.Location} as Region {regionId}");
        plot.RegionId = regionId;

        var neighbourLocations = plot.Location.Neighbours();
        foreach (var neighbourLocation in neighbourLocations)
        {
            if (!_map.IsInGrid(neighbourLocation))
            {
                continue;
            }

            var neighbour = _map.Read(neighbourLocation);
            FillRegion(plotType, neighbour, regionId);
        }
    }

    public override string Part1()
    {
        var prices = _map.ReadAll()
            .GroupBy(p => p.RegionId)
            .ToDictionary(g => g.Key, g => g.Count() * g.Sum(p => p.FenceCount));

        var total = prices.Sum(p => p.Value);

        return total.ToString();
    }

    public override string Part2()
    {
        return string.Empty;
    }

    private class Plot
    {
        public Coordinate2D Location { get; set; }
        public int? RegionId { get; set; }
        public char PlotType { get; set; }
        public int FenceCount { get; set; }
    }
}
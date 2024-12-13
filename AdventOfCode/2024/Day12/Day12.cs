using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2024.Day12;

public class Day12 : Day
{
    // not 915892
    public Day12() : base(2024, 12, "Day12/input_2024_12.txt", "1473276", "901100", false)
    {
    }

    private Grid2D<Plot> _map;
    private List<int> _regionIds = new ();
    private List<Plot> _regionHeads = new ();
    private Dictionary<int, List<int>> _regionContains = new ();

    public override void Initialise()
    {
        var height = InputLines.Count;
        var width = InputLines[0].Length;

        _map = new Grid2D<Plot>(width, height);
        
        var y = height - 1;
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
            y -= 1;
        }

        SetRegions();
        SetFenceCounts();
        SetContains();
    }

    private void SetContains()
    {
        var allRegionIds = _regionHeads
            .Select(x => x.RegionId.Value)
            .ToList();

        _regionContains = allRegionIds
            .ToDictionary(
                rid => rid,
                rid => new List<int>());

        var plotsByRegion = _map.ReadAll()
            .GroupBy(p => p.RegionId.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var regionId in allRegionIds)
        {
            var plotsForThisRegion = plotsByRegion[regionId];

            var allNeighboursLocations = plotsForThisRegion
                .SelectMany(p => p.Location.Neighbours())
                .Distinct();

            if (allNeighboursLocations.Any(l => !_map.IsInGrid(l)))
            {
                continue;
            }
            
            var allNeighbours = allNeighboursLocations
                .Select(_map.Read)
                .Where(p => p.RegionId != regionId);

            var neighbourGroups = allNeighbours
                .GroupBy(p => p.RegionId.Value);
            
            if (neighbourGroups.Count() == 1)
            {
                var containerRegionId = neighbourGroups.First().Key;
                _regionContains[containerRegionId].Add(regionId);
            }
        }

        /*
UUUUU
1UU3U
111UU
*/
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


        // TraceLine($"Filling from {firstUnsetPlot.Location} as Region {regionId}");
        _regionIds.Add(regionId);
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

    private Dictionary<string, int> _edgeCountCache = new ();
    private int CountEdges(int regionId, bool countOffGrid, List<FenceLocation> fenceLocations)
    {
        var cacheKey = $"{regionId}_{countOffGrid}";
        if (_edgeCountCache.TryGetValue(cacheKey, out var cachedResult))
        {
            return cachedResult;
        }

        var firstPlot = _map.ReadAll()
            .FirstOrDefault(x => x.RegionId == regionId);

        TraceLine($"Region {regionId} {countOffGrid}: First location {firstPlot.Location}");

        var result = CountEdges(regionId, firstPlot, firstPlot, Direction.Down, true, countOffGrid, fenceLocations);

        TraceLine($"Region {regionId} {countOffGrid}: boundaryFences {result}");

        foreach (var containedRegionId in _regionContains[regionId])
        {
            var containedRegionFenceLocations = new List<FenceLocation>();
            TraceLine($"Region {regionId} {countOffGrid}: Getting Inner fences for {containedRegionId}");
            var containedFences = CountEdges(containedRegionId, false, containedRegionFenceLocations);

            if (!fenceLocations.Any(f => containedRegionFenceLocations.Any(crlf => f.Equals(crlf))))
            {
                TraceLine($"Region {regionId} {countOffGrid}: Inner fences for {containedRegionId} - {containedFences}");
                result += containedFences;
            }
            else
            {
                TraceLine($"Region {regionId} {countOffGrid}: Already counted inner fences for {containedRegionId} - {containedFences}");
            }
        }
/*
        var allOtherRegionIds = _regionHeads
            .Select(x => x.RegionId.Value)
            .Where(x => x != regionId)
            .ToList();

        foreach (var otherRegionId in allOtherRegionIds)
        {
            var contained = _map.ReadAll()
                .Where(p => p.RegionId == otherRegionId)
                .All(p => AllNeighboursInRegions(p, regionId, otherRegionId));

            if (contained)
            {
                TraceLine($"Region {regionId} {countOffGrid}: Getting Inner fences for {otherRegionId}");
                var containedFences = CountEdges(otherRegionId, false);
                TraceLine($"Region {regionId} {countOffGrid}: Inner fences for {otherRegionId} - {containedFences}");
                result += containedFences;
            }
        }
*/
        _edgeCountCache.Add(cacheKey, result);
        return result;
    }

    private bool AllNeighboursInRegions(Plot plot, params int[] regionIds)
    {
        var neighbourLocations = plot.Location.Neighbours();

        if (neighbourLocations.Any(l => !_map.IsInGrid(l)))
        {
            return false;
        }

        var neighbourPlots = neighbourLocations
            .Select(_map.Read)
            .ToList();

        return neighbourPlots.All(p => regionIds.Any(rid => p.RegionId == rid));
    }

    private int CountEdges(
        int regionId,
        Plot startPlot,
        Plot currentPlot,
        Direction currentDirection,
        bool starting,
        bool countOffGrid,
        List<FenceLocation> fenceLocations)
    {
        TraceLine($"> Region {regionId} {countOffGrid}: Checking {currentPlot.Location} {currentDirection}");
        if (!starting
            && currentPlot.Location == startPlot.Location
            && currentDirection == Direction.Down)
            {
                TraceLine($"> Region {regionId} {countOffGrid}: Complete");
                return 0;
            }
        
        fenceLocations.Add(new FenceLocation(currentPlot.Location, currentDirection));

        var nextDirection = NextDirection(currentDirection);
        var nextLocation = currentPlot.Location.Neighbour(nextDirection);

        if (!_map.IsInGrid(nextLocation))
        {
            TraceLine($"> Region {regionId} {countOffGrid}: Gone off grid {nextLocation}");
            var turnCount = countOffGrid ? 1 : 1;
            return turnCount + CountEdges(regionId, startPlot, currentPlot, nextDirection, false, countOffGrid, fenceLocations);
        }

        var nextPlot = _map.Read(nextLocation);
        if (nextPlot.RegionId != regionId)
        {
            TraceLine($"> Region {regionId} {countOffGrid}: Gone out of region {nextLocation}");
            return 1 + CountEdges(regionId, startPlot, currentPlot, nextDirection, false, countOffGrid, fenceLocations);
        }

        var innerTurnLocation = nextLocation.Neighbour(currentDirection);
        if (_map.IsInGrid(innerTurnLocation))
        {
            var innerTurnPlot = _map.Read(innerTurnLocation);
            if (innerTurnPlot.RegionId == regionId)
            {
                var newDirection = InnerTurnDirection(currentDirection);
                TraceLine($"> Region {regionId} {countOffGrid}: inner turn to {innerTurnLocation}");

                return 1 + CountEdges(regionId, startPlot, innerTurnPlot, newDirection, false, countOffGrid, fenceLocations);
            }
        }

        TraceLine($"> Region {regionId} {countOffGrid}: continuing to {nextLocation}");
        return CountEdges(regionId, startPlot, nextPlot, currentDirection, false, countOffGrid, fenceLocations);
    }

    private Direction NextDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Down: return Direction.Right;
            case Direction.Right: return Direction.Up;
            case Direction.Up: return Direction.Left;
            case Direction.Left: return Direction.Down;
        }

        throw new Exception($"Unknown direction {direction}");
    }

    private Direction InnerTurnDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left: return Direction.Up;
            case Direction.Up: return Direction.Right;
            case Direction.Right: return Direction.Down;
            case Direction.Down: return Direction.Left;
        }

        throw new Exception($"Unknown direction {direction}");
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
        // return CountEdges(1, true).ToString();

        var prices = _map.ReadAll()
            .GroupBy(p => p.RegionId.Value)
            .ToDictionary(
                g => g.Key,
                g => {
                    var plotCount = g.Count();
                    var fenceLocations = new List<FenceLocation>();
                    var fenceCount = CountEdges(g.Key, true, fenceLocations);
                    return (plotCount * fenceCount, fenceLocations);
                });

        var total = prices.Sum(p => p.Value.Item1);

        return total.ToString();
    }

    private class Plot
    {
        public Coordinate2D Location { get; set; }
        public int? RegionId { get; set; }
        public char PlotType { get; set; }
        public int FenceCount { get; set; }
    }

    private class FenceLocation
    {
        public FenceLocation(Coordinate2D location, Direction side)
        {
            switch (side)
            {
                case Direction.Up:
                    CanoncialLocation = location;
                    CanoncicalSide = side;
                    break;
                case Direction.Down:
                    CanoncialLocation = location.Neighbour(Direction.Down);
                    CanoncicalSide = Direction.Up;
                    break;
                case Direction.Left:
                    CanoncialLocation = location;
                    CanoncicalSide = side;
                    break;
                case Direction.Right:
                    CanoncialLocation = location.Neighbour(Direction.Right);
                    CanoncicalSide = Direction.Left;
                    break;
            }
        }

        public Coordinate2D CanoncialLocation { get; set; }
        public Direction CanoncicalSide { get; set; }

        public override string ToString()
        {
            return $"{CanoncialLocation} {CanoncicalSide}";
        }

        public override bool Equals(object obj)
        {
            var fenceLocation = obj as FenceLocation;

            if (fenceLocation == null)
            {
                return false;
            }

            return CanoncialLocation.Equals(fenceLocation.CanoncialLocation)
                && CanoncicalSide == fenceLocation.CanoncicalSide;
        }
    }
}
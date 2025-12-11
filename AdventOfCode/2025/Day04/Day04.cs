using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2025.Day04;

public class Day04 : Day
{
    public Day04() : base(2025, 4, "Day04/input_2025_04.txt", "1527", "8690", false)
    {

    }

    private Grid2D<bool> _map;
    
    public override void Initialise()
    {
        _map = Grid2D<bool>.CreateWithCartesianCoordinates(
            InputLines, 
            (coords, c) => c == '@');
    }

    public override string Part1()
    {
        var accessible = 0;
        foreach (var coord in _map.AllCoordinates())
        {
            if (!_map.Read(coord))
            {
                continue;
            }
            
            var surroundingPaperCount = 0;
            foreach (var neighbour in coord.AllNeighbours())
            {
                if (_map.IsInGrid(neighbour) && _map.Read(neighbour))
                {
                    surroundingPaperCount += 1;
                }
            }

            if (surroundingPaperCount < 4)
            {
                TraceLine($"{coord} is accessible");
                accessible += 1;
            }
        }
        return accessible.ToString();
    }

    public override string Part2()
    {
        var totalRemoved = 0;
        int removed = 0;
        do
        {
            removed = RemoveRolls();
            totalRemoved += removed;
        } while (removed > 0);

        TraceLine(_map.Print(x => x ? '@' : ' '));
        
        return totalRemoved.ToString();
    }

    private int RemoveRolls()
    {
        var removedCount = 0;
        foreach (var coord in _map.AllCoordinates())
        {
            if (!_map.Read(coord))
            {
                continue;
            }
            
            var surroundingPaperCount = 0;
            foreach (var neighbour in coord.AllNeighbours())
            {
                if (_map.IsInGrid(neighbour) && _map.Read(neighbour))
                {
                    surroundingPaperCount += 1;
                }
            }

            if (surroundingPaperCount < 4)
            {
                TraceLine($"{coord} is being removed");
                _map.Write(coord, false);
                removedCount += 1;
            }
        }

        return removedCount;
    }
}
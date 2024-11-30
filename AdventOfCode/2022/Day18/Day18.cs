using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2022.Day18;

public class Day18 : Day
{
    public Day18() : base(2022, 18, "Day18/input_2022_18.txt", "4580", "2610")
    {

    }

    List<Coordinate3D> _lava;
    Grid3D<State> _map;
    public override void Initialise()
    {
        _lava = InputLines
            .Select(l => new Coordinate3D(l))
            .ToList();

        var minX = _lava.Min(l => l.X) - 1;
        var maxX = _lava.Max(l => l.X) + 1;
        var minY = _lava.Min(l => l.Y) - 1;
        var maxY = _lava.Max(l => l.Y) + 1;
        var minZ = _lava.Min(l => l.Z) - 1;
        var maxZ = _lava.Max(l => l.Z) + 1;

        _map = new Grid3D<State>((int)minX, (int)minY, (int)minZ, (int)maxX, (int)maxY, (int)maxZ);

        foreach(var lava in _lava)
        {
            _map.Write(lava, State.Lava);
        }
    }

    public override string Part1()
    {
        var surfaceArea = _lava.Sum(l => l.Neighbours().Count(IsNotLava));

        return surfaceArea.ToString();
    }

    public override string Part2()
    {
        FillWithSteam(new Coordinate3D(_map.MinX, _map.MinY, _map.MinZ));

        var surfaceArea = _lava.Sum(l => l.Neighbours().Count(IsSteam));

        return surfaceArea.ToString();
    }

    private enum State
    {
        Air,
        Lava,
        Steam
    }

    private bool IsNotLava(Coordinate3D coordinate) => !IsLava(coordinate);
    private bool IsLava(Coordinate3D coordinate)
    {
        if (!_map.IsInGrid(coordinate))
        {
            return false;
        }

        return _map.Read(coordinate) == State.Lava;
    }

    private bool IsSteam(Coordinate3D coordinate)
    {
        if (!_map.IsInGrid(coordinate))
        {
            return true;
        }

        return _map.Read(coordinate) == State.Steam;
    }

    private void FillWithSteam(Coordinate3D coordinate)
    {
        if (!_map.IsInGrid(coordinate))
        {
            return;
        }

        var currentState = _map.Read(coordinate);
        if (currentState == State.Air)
        {
            _map.Write(coordinate, State.Steam);
            foreach (var neighbour in coordinate.Neighbours())
            {
                FillWithSteam(neighbour);
            }
        }
    }
}
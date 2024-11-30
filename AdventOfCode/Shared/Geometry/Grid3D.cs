using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Shared.Geometry;

public class Grid3D<T>
{
    private T[,,] _grid;

    public int Width { get; }
    public int Height { get; }
    public int Depth { get; }

    public int MinX { get; }
    public int MaxX { get; }
    public int MinY { get; }
    public int MaxY { get; }
    public int MinZ { get; }
    public int MaxZ { get; }

    public Grid3D(int width, int height, int depth)
        : this(0, 0, 0, width - 1, height - 1, depth - 1)
    {
    }

    public Grid3D(int minX, int minY, int minZ, int maxX, int maxY, int maxZ)
    {
        var height = maxY - minY + 1;
        var width = maxX - minX + 1;
        var depth = maxZ - minZ + 1;

        MinX = minX;
        MinY = minY;
        MinZ = minZ;
        MaxX = maxX;
        MaxY = maxY;
        MaxZ = maxZ;

        _grid = new T[width, height, depth];
        Width = width;
        Height = height;
        Depth = depth;
    }

    public bool IsInGrid(int x, int y, int z)
    {
        return MinX <= x && x <= MaxX
                         && MinY <= y && y <= MaxY
                         && MinZ <= z && z <= MaxZ;
    }

    public bool IsInGrid(Coordinate3D coordinate)
    {
        return IsInGrid((int)coordinate.X, (int)coordinate.Y, (int)coordinate.Z);
    }

    public T Read(int x, int y, int z)
    {
        return _grid[x - MinX, y - MinY, z - MinZ];
    }

    public T Read(Coordinate3D coordinate)
    {
        return Read((int)coordinate.X, (int)coordinate.Y, (int)coordinate.Z);
    }

    public void Write(int x, int y, int z, T value)
    {
        _grid[x - MinX, y - MinY, z - MinZ] = value;
    }

    public void Write(Coordinate3D coordinate, T value)
    {
        Write((int)coordinate.X, (int)coordinate.Y, (int)coordinate.Z, value);
    }

    public IEnumerable<T> ReadAll()
    {
        foreach (var y in AllY())
        {
            foreach (var x in AllX())
            {
                foreach (var z in AllZ())
                {
                    yield return Read(x, y, z);
                }
            }
        }
    }

    public IEnumerable<Coordinate3D> GetAllCoordinates()
    {
        foreach (var y in AllY())
        {
            foreach (var x in AllX())
            {
                foreach (var z in AllZ())
                {
                    yield return new Coordinate3D(x, y, z);
                }
            }
        }
    }

    public IEnumerable<int> AllX()
    {
        return Enumerable.Range(MinX, Width);
    }

    public IEnumerable<int> AllY()
    {
        return Enumerable.Range(MinY, Height);
    }

    public IEnumerable<int> AllZ()
    {
        return Enumerable.Range(MinZ, Depth);
    }
}
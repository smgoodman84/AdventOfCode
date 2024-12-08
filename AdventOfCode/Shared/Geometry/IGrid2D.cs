using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Shared.Geometry;

public interface IGrid2D<T>
{
    long MinX { get; }
    long MinY { get; }
    long MaxX { get; }
    long MaxY { get; }

    long Width { get; }
    long Height { get; }

    bool IsInGrid(int x, int y);
    bool IsInGrid(long x, long y);
    bool IsInGrid(Coordinate2D coordinate);

    T Read(int x, int y);
    T Read(long x, long y);
    T Read(Coordinate2D coordinate);

    void Write(long x, long y, T value);
    void Write(Coordinate2D coordinate, T value);

    IEnumerable<T> ReadAll();
    IEnumerable<Coordinate2D> FindCoordinates(Func<T, bool> predicate);
}

public static class IGridExtensions
{
    public static string Print<T>(this IGrid2D<T> grid, Func<T, char> elementMapper)
    {
        var result = new StringBuilder();

        var minX = grid.MinX;
        var maxX = grid.MaxX;
        var minY = grid.MinY;
        var maxY = grid.MaxY;

        var width = grid.Width;
        var height = grid.Height;

        result.AppendLine($"Origin {minX},{minY}");

        // var spaceCount = 0;

        result.Append("      ");
        for (var x = minX; x <= maxX; x++)
        {
            result.Append($"{Pad(x)[0]}");
        }
        result.AppendLine();
        result.Append("      ");
        for (var x = minX; x <= maxX; x++)
        {
            result.Append($"{Pad(x)[1]}");
        }
        result.AppendLine();
        result.Append("      ");
        for (var x = minX; x <= maxX; x++)
        {
            result.Append($"{Pad(x)[2]}");
        }
        result.AppendLine();
        result.Append("      ");
        for (var x = minX; x <= maxX; x++)
        {
            result.Append($"{Pad(x)[3]}");
        }
        result.AppendLine();
        result.AppendLine();

        for (var y = minY; y <= maxY; y++)
        {
            result.Append($"{Pad(y)}: ");
            for (var x = minX; x <= maxX; x++)
            {
                var value = elementMapper(grid.Read(x, y));
                result.Append(value);
            }
            result.AppendLine();
        }

        return result.ToString();
    }

    private static string Pad(long value)
    {
        if (value >= 0)
        {
            return $" {value:000}";
        }
        return $"{value:000}";
    }
}
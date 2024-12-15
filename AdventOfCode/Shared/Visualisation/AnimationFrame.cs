using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public class AnimationFrame : IDisposable
{
    private int _scaleFactor;
    private Image<Rgba32> _frame;
    public AnimationFrame(int width, int height, Color color, int scaleFactor)
    {
        _scaleFactor = scaleFactor;
        _frame = new Image<Rgba32>(width, height, color);
    }

    public void SetColor(int x, int y, Color color)
    {
        var scaledX = x * _scaleFactor;
        var scaledY = y * _scaleFactor;

        for (var xOffset = 0; xOffset < _scaleFactor; xOffset += 1)
        {
            for (var yOffset = 0; yOffset < _scaleFactor; yOffset += 1)
            {
                _frame[scaledX + xOffset, scaledY + yOffset] = color;
            }
        }
    }

    public Image<Rgba32> GetImage()
    {
        return _frame;
    }

    public void Dispose()
    {
        _frame?.Dispose();
    }
}
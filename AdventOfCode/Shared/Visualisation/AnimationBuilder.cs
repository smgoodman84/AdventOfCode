using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public class AnimationBuilder : IDisposable
{
    private Image<Rgba32> _allFrames;
    private int _width;
    private int _height;
    private Color _color;
    private string _filename;
    private int _scaleFactor;
    public AnimationBuilder(int width, int height, Color color, string filename, int scaleFactor = 1)
    {
        _width = width * scaleFactor;
        _height = height * scaleFactor;
        _color = color;
        _allFrames = new Image<Rgba32>(_width, _height, color);
        _filename = filename;
        _scaleFactor = scaleFactor;
    }

    public void CreateFrame(Action<AnimationFrame> frameAction)
    {
        var frame = new AnimationFrame(_width, _height, _color, _scaleFactor);
        frameAction(frame);
        _allFrames.Frames.AddFrame(frame.GetImage().Frames.RootFrame);
    }

    public void Save()
    {
        if (_filename.EndsWith(".gif"))
        {
            _allFrames.SaveAsGif(_filename);
            return;
        }
        if (_filename.EndsWith(".png"))
        {
            _allFrames.SaveAsPng(_filename);
            return;
        }
    }

    public void Dispose()
    {
        _allFrames?.Dispose();
    }
}

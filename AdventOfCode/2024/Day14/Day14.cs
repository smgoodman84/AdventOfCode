using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace AdventOfCode._2024.Day14;

public class Day14 : Day
{
    public Day14() : base(2024, 14, "Day14/input_2024_14.txt", "225943500", "6377", true)
    {
    }

    private List<Robot> _robots = new ();
    public override void Initialise()
    {
        _robots = InputLines.Select(ParseRobot).ToList();
    }

    private Robot ParseRobot(string description)
    {
        var split = description.Split(' ');
        var position = split[0].Split('=')[1];
        var velocty = split[1].Split('=')[1];

        var positionSplit = position.Split(',');
        var veloctySplit = velocty.Split(',');

        return new Robot
        {
            InitialPosition = new Coordinate2D(
                int.Parse(positionSplit[0]),
                int.Parse(positionSplit[1])),
            Velocity = new Vector2D(
                int.Parse(veloctySplit[0]),
                int.Parse(veloctySplit[1]))
        };
    }

    public override string Part1()
    {
        var mapWidth = 101;
        var mapHeight = 103;

        var newPositions = _robots
            .Select(r => r.GetPositionAfterTime(100, mapWidth, mapHeight))
            .ToList();

        var left = newPositions.Where(p => p.X < mapWidth / 2).ToList();
        var right = newPositions.Where(p => p.X > mapWidth / 2).ToList();

        var topLeft = left.Where(p => p.Y < mapHeight / 2).ToList();
        var topRight = right.Where(p => p.Y < mapHeight / 2).ToList();
        var bottomLeft = left.Where(p => p.Y > mapHeight / 2).ToList();
        var bottomRight = right.Where(p => p.Y > mapHeight / 2).ToList();

        var safetyFactor
            = topLeft.Count
            * topRight.Count
            * bottomLeft.Count
            * bottomRight.Count;

        return safetyFactor.ToString();
    }

    public override string Part2()
    {
        var mapWidth = 101;
        var mapHeight = 103;
        var result = -1;

        // Found by finding the correct frame from all frames
        var treeIdentifyingPixels = Enumerable.Range(55, 12)
            .Select(x => new Coordinate2D(x, 67));

        using Image<Rgba32> allFrames = new(mapWidth, mapHeight, Color.Black);

        var time = 1;
        while (time < mapWidth * mapHeight)
        {
            using Image<Rgba32> image = new(mapWidth, mapHeight, Color.Black);
            var newPositions = _robots
                .Select(r => r.GetPositionAfterTime(time, mapWidth, mapHeight))
                .Distinct()
                .OrderBy(p => p.Y)
                .ThenBy(p => p.X)
                .ToList();

            var robotIndex = 0;
            for (var y = 0; y < mapHeight; y += 1)
            {
                for (var x = 0; x < mapWidth; x += 1)
                {
                    if (robotIndex < newPositions.Count
                        && newPositions[robotIndex].X == x
                        && newPositions[robotIndex].Y == y)
                    {
                        image[x, y] = Color.White;
                        robotIndex += 1;
                    }
                }
            }

            if (result == -1 
                && treeIdentifyingPixels.All(p => newPositions.Contains(p)))
            {
                image.SaveAsGif("2024_14_2_tree.gif");
                result = time;
            }

            allFrames.Frames.AddFrame(image.Frames.RootFrame);

            time += 1;
        }

        allFrames.SaveAsGif("2024_14_2_all_frames.gif");

        return result.ToString();
    }

    private class Robot
    {
        public Coordinate2D InitialPosition { get; set; }
        public Vector2D Velocity { get; set; }

        public Coordinate2D GetPositionAfterTime(int time, int width, int height)
        {
            var newX = InitialPosition.X + Velocity.X * time;
            var newY = InitialPosition.Y + Velocity.Y * time;

            var wrappedX = newX % width;
            if (wrappedX < 0)
            {
                wrappedX += width;
            }

            var wrappedY = newY % height;
            if (wrappedY < 0)
            {
                wrappedY += height;
            }

            return new Coordinate2D(wrappedX, wrappedY);
        }
    }
}
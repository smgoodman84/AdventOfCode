using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2020.Day20;

public class Day20 : Day
{
    public Day20() : base(2020, 20, "Day20/input_2020_20.txt", "8425574315321", string.Empty)
    {

    }

    private List<Tile> _tiles;

    public override void Initialise()
    {
        var lines = InputLines.ToList();
        lines.Add(string.Empty);

        _tiles = new List<Tile>();
        var tilelines = new List<string>();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                var tile = new Tile(tilelines);
                _tiles.Add(tile);
                tilelines = new List<string>();
            }
            else
            {
                tilelines.Add(line);
            }
        }
        LinkNeighbours();
    }

    public override string Part1()
    {
        foreach (var tile in _tiles)
        {
            // Trace($"Tile {tile.TileId}: {tile.Neighbours.Count} neighbours");
        }

        var corners = _tiles.Where(t => t.Neighbours.Count == 4);

        long result = 1;
        foreach (var corner in corners)
        {
            result *= corner.TileId;
        }

        return result.ToString();
    }

    private void LinkNeighbours()
    {
        var lookups = _tiles
            .SelectMany(t => t.GetLookups());

        var groups = lookups.GroupBy(l => l.Edge);

        foreach (var group in groups)
        {
            foreach (var tile1 in group)
            {
                foreach (var tile2 in group)
                {
                    if (tile1.Tile.TileId != tile2.Tile.TileId)
                    {
                        tile1.Tile.AddNeighbour(
                            new Neighbour
                            {
                                NeighbourTile = tile2.Tile,
                                ThisEdge = tile1.EdgeType,
                                ThatEdge = tile2.EdgeType
                            });
                    }
                }
            }
        }
    }

    public override string Part2()
    {
        var firstCorner = _tiles.First(t => t.Neighbours.Count == 4);

        if (firstCorner.Neighbours.Any(n => n.ThisEdge == "Top"))
        {
            firstCorner.FlipVertically();
        }

        if (firstCorner.Neighbours.Any(n => n.ThisEdge == "Left"))
        {
            firstCorner.FlipHorizontally();
        }

        var columnHeads = new List<Tile>();
        var currentTile = firstCorner;
        columnHeads.Add(currentTile);
        do
        {
            var nextTileNeighbour = currentTile.Neighbours.First(n => n.ThisEdge == "Right");
            var nextTile = nextTileNeighbour.NeighbourTile;
            switch (nextTileNeighbour.ThatEdge)
            {
                case "Top":
                    nextTile.FlipVertically();
                    nextTile.RotateClockwise();
                    break;
                case "TopReverse":
                    nextTile.RotateClockwise();
                    break;
                case "Bottom":
                    nextTile.RotateCounterClockwise();
                    break;
                case "BottomReverse":
                    nextTile.FlipVertically();
                    nextTile.RotateCounterClockwise();
                    break;
                case "Left":
                    break;
                case "LeftReverse":
                    nextTile.FlipVertically();
                    break;
                case "Right":
                    nextTile.FlipHorizontally();
                    break;
                case "RightReverse":
                    nextTile.FlipVertically();
                    nextTile.FlipHorizontally();
                    break;
            }
            currentTile = nextTile;
            columnHeads.Add(currentTile);
        } while (currentTile.Neighbours.Count > 4);

        foreach (var columnHead in columnHeads)
        {
            OrientColumn(columnHead);
        }

        var image = new List<string>();
        currentTile = firstCorner;
        do
        {
            var nextTileNeighbour = currentTile.Neighbours.First(n => n.ThisEdge == "Right");
            currentTile = nextTileNeighbour.NeighbourTile;
        } while (currentTile.Neighbours.Count > 4);

        return string.Empty;
    }

    private void OrientColumn(Tile columnHead)
    {
        var currentTile = columnHead;
        do
        {
            var nextTileNeighbour = currentTile.Neighbours.First(n => n.ThisEdge == "Bottom");
            var nextTile = nextTileNeighbour.NeighbourTile;
            switch (nextTileNeighbour.ThatEdge)
            {
                case "Top":
                    break;
                case "TopReverse":
                    nextTile.FlipHorizontally();
                    break;
                case "Bottom":
                    break;
                    nextTile.FlipVertically();
                case "BottomReverse":
                    nextTile.FlipVertically();
                    nextTile.FlipHorizontally();
                    break;
                case "Left":
                    nextTile.RotateCounterClockwise();
                    break;
                case "LeftReverse":
                    nextTile.RotateCounterClockwise();
                    break;
                case "Right":
                    nextTile.RotateClockwise();
                    break;
                case "RightReverse":
                    nextTile.FlipHorizontally();
                    nextTile.RotateClockwise();
                    break;
            }
            currentTile = nextTile;
        } while (currentTile.Neighbours.Count > 4);
    }

    private class TileLookup
    {
        public string Edge { get; set; }
        public string EdgeType { get; set; }
        public Tile Tile { get; set; }
    }

    private class Neighbour
    {
        public string ThisEdge { get; set; }
        public string ThatEdge { get; set; }
        public Tile NeighbourTile { get; set; }
    }

    private class Tile
    {
        public string Left { get; set; }
        public string Right { get; set; }
        public string Top { get; set; }
        public string Bottom { get; set; }

        public string LeftReverse { get; set; }
        public string RightReverse { get; set; }
        public string TopReverse { get; set; }
        public string BottomReverse { get; set; }

        public int TileId { get; private set; }
        public List<Neighbour> Neighbours { get; set; } = new List<Neighbour>();
        public List<string> Lines { get; set; }
        public Tile(List<string> lines)
        {
            TileId = int.Parse(lines[0]
                .Replace("Tile ", string.Empty)
                .Replace(":", string.Empty));

            Lines = lines.Skip(1).ToList();

            SetEdges();
        }

        private void SetEdges()
        {
            Top = Lines.First();
            Bottom = Lines.Last();
            Left = string.Join("", Lines.Select(l => l.First()));
            Right = string.Join("", Lines.Select(l => l.Last()));

            TopReverse = string.Join("", Top.Reverse());
            BottomReverse = string.Join("", Bottom.Reverse());
            LeftReverse = string.Join("", Left.Reverse());
            RightReverse = string.Join("", Right.Reverse());
        }

        public void FlipVertically()
        {
            Lines.Reverse();
            SetEdges();

            foreach (var neighbour in Neighbours)
            {
                AdjustNeighbours(neighbour, "Top", "Bottom");
                AdjustNeighbours(neighbour, "TopReverse", "BottomReverse");
                AdjustNeighbours(neighbour, "Bottom", "Top");
                AdjustNeighbours(neighbour, "BottomReverse", "TopReverse");

                AdjustNeighbours(neighbour, "Left", "LeftReverse");
                AdjustNeighbours(neighbour, "LeftReverse", "Left");
                AdjustNeighbours(neighbour, "Right", "RightReverse");
                AdjustNeighbours(neighbour, "RightReverse", "Right");
            }
        }

        public void FlipHorizontally()
        {
            Lines = Lines
                .Select(l => string.Join("", l.Reverse()))
                .ToList();
            SetEdges();

            foreach (var neighbour in Neighbours)
            {
                AdjustNeighbours(neighbour, "Top", "TopReverse");
                AdjustNeighbours(neighbour, "TopReverse", "Top");
                AdjustNeighbours(neighbour, "Bottom", "BottomReverse");
                AdjustNeighbours(neighbour, "BottomReverse", "Bottom");

                AdjustNeighbours(neighbour, "Left", "Right");
                AdjustNeighbours(neighbour, "Right", "Left");
                AdjustNeighbours(neighbour, "LeftReverse", "RightReverse");
                AdjustNeighbours(neighbour, "RightReverse", "LeftReverse");
            }
        }

        private void AdjustNeighbours(Neighbour neighbour, string thisEdge, string thatEdge)
        {
            if (neighbour.ThatEdge != thisEdge)
            {
                return;
            }

            neighbour.ThisEdge = thatEdge;
            var backLinks = neighbour.NeighbourTile.Neighbours.Where(n => n.NeighbourTile == this && n.ThatEdge == thisEdge);
            foreach (var backLink in backLinks)
            {
                backLink.ThatEdge = thatEdge;
            }
        }

        public void AddNeighbour(Neighbour neighbour)
        {
            Neighbours.Add(neighbour);
        }

        public List<TileLookup> GetLookups()
        {
            return new List<TileLookup>
            {
                new TileLookup { Edge = Top, EdgeType = "Top", Tile = this },
                new TileLookup { Edge = Bottom, EdgeType = "Bottom", Tile = this },
                new TileLookup { Edge = Left, EdgeType = "Left", Tile = this },
                new TileLookup { Edge = Right, EdgeType = "Right", Tile = this },
                new TileLookup { Edge = TopReverse, EdgeType = "TopReverse", Tile = this },
                new TileLookup { Edge = BottomReverse, EdgeType = "BottomReverse", Tile = this },
                new TileLookup { Edge = LeftReverse, EdgeType = "LeftReverse", Tile = this },
                new TileLookup { Edge = RightReverse, EdgeType = "RightReverse", Tile = this },
            };
        }

        internal void RotateClockwise()
        {
            var arrays = Lines
                .Select(l => l.ToCharArray())
                .ToArray();

            var width = arrays.First().Length;
            var height = arrays.Length;

            var newLines = new List<string>();

            for(var x = 0; x < width; x++)
            {
                var line = string.Empty;
                for (var y = height - 1; y >= 0; y --)
                {
                    line += arrays[y][x];
                }
                newLines.Add(line);
            }
            SetEdges();

            foreach (var neighbour in Neighbours)
            {
                AdjustNeighbours(neighbour, "Top", "Right");
                AdjustNeighbours(neighbour, "TopReverse", "RightReverse");
                AdjustNeighbours(neighbour, "Bottom", "Left");
                AdjustNeighbours(neighbour, "BottomReverse", "LeftReverse");

                AdjustNeighbours(neighbour, "Left", "TopReverse");
                AdjustNeighbours(neighbour, "Right", "BottomReverse");
                AdjustNeighbours(neighbour, "LeftReverse", "Top");
                AdjustNeighbours(neighbour, "RightReverse", "Bottom");
            }
        }

        internal void RotateCounterClockwise()
        {
            RotateClockwise();
            RotateClockwise();
            RotateClockwise();
        }

        private List<string> GetImageWithoutBorder()
        {
            var actualHeight = Lines.Count - 2;
            var actualWidth = Lines.First().Length - 2;

            var lines = Lines
                .Skip(1)
                .Take(actualHeight)
                .Select(l => l.Substring(1, actualWidth))
                .ToList();

            return lines;
        }
    }
}
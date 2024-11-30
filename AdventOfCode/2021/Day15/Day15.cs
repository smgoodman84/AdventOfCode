using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.Graphs;

namespace AdventOfCode._2021.Day15;

public class Day15 : Day
{
    public Day15() : base(2021, 15, "Day15/input_2021_15.txt", "361", "2838", false)
    {

    }

    public override string Part1() => FindShortestPath(1, 1);
    public override string Part2() => FindShortestPath(5, 5);

    public string FindShortestPath(int tileWidth, int tileHeight)
    {
        var fileNodes = InputLines
            .SelectMany((l, y) => l.Select((d, x) => new NodeData(x, y, int.Parse(d.ToString()))).ToArray())
            .ToDictionary(n => n.GetIdentifier(), n => n);

        var fileNodeWidth = fileNodes.Values.Max(n => n.Coordinate.X) + 1;
        var fileNodeHeight = fileNodes.Values.Max(n => n.Coordinate.Y) + 1;

        var totalWidth = (int)fileNodeWidth * tileWidth;
        var totalHeight = (int)fileNodeHeight * tileHeight;

        var graph = new Graph<NodeData>();
        for (var tileX = 0; tileX < tileWidth; tileX++)
        {
            for (var tileY = 0; tileY < tileHeight; tileY++)
            {
                foreach (var node in fileNodes.Values)
                {
                    var distance = WrapRisk(node.Distance + tileX + tileY);
                    var x = tileX * fileNodeWidth + node.Coordinate.X;
                    var y = tileY * fileNodeHeight + node.Coordinate.Y;

                    var graphNode = new GraphNode<NodeData>(new NodeData(x, y, distance));
                    graph.AddNode(graphNode);
                }
            }
        }

        for (var x = 0; x < totalWidth; x += 1)
        {
            for (var y = 0; y < totalHeight; y += 1)
            {
                graph.TryGetNode(new Coordinate2D(x, y).ToString(), out var thisNode);
                var distanceToEnterNode = thisNode.Data.Distance;

                if (graph.TryGetNode(new Coordinate2D(x - 1, y).ToString(), out var leftNode))
                {
                    graph.AddEdge(new GraphEdge<NodeData>
                    {
                        Source = leftNode,
                        Destination = thisNode,
                        Distance = distanceToEnterNode
                    });
                }

                if (graph.TryGetNode(new Coordinate2D(x + 1, y).ToString(), out var rightNode))
                {
                    graph.AddEdge(new GraphEdge<NodeData>
                    {
                        Source = rightNode,
                        Destination = thisNode,
                        Distance = distanceToEnterNode
                    });
                }

                if (graph.TryGetNode(new Coordinate2D(x, y - 1).ToString(), out var upNode))
                {
                    graph.AddEdge(new GraphEdge<NodeData>
                    {
                        Source = upNode,
                        Destination = thisNode,
                        Distance = distanceToEnterNode
                    });
                }

                if (graph.TryGetNode(new Coordinate2D(x, y + 1).ToString(), out var downNode))
                {
                    graph.AddEdge(new GraphEdge<NodeData>
                    {
                        Source = downNode,
                        Destination = thisNode,
                        Distance = distanceToEnterNode
                    });
                }
            }
        }

        var startCoordinate = new Coordinate2D(0, 0);
        var endCoordinate = new Coordinate2D(totalWidth - 1, totalHeight - 1);

        graph.TryGetNode(startCoordinate.ToString(), out var startNode);
        graph.TryGetNode(endCoordinate.ToString(), out var endNode);

        var result = graph.GetShortestPathNodesAndDistance(startNode, endNode, 5000);

        return result.Distance.ToString();
    }

    private int WrapRisk(int risk)
    {
        if (risk > 9)
        {
            return risk - 9;
        }

        return risk;
    }

    private class NodeData : IGraphNodeData
    {
        public NodeData(long x, long y, int distance)
        {
            Coordinate = new Coordinate2D(x, y);
            Distance = distance;
        }

        public List<NodeData> Neighbours { get; } = new List<NodeData>();
        public int Distance { get; set; }
        public Coordinate2D Coordinate { get; set; }
        public string GetIdentifier() => Coordinate.ToString();

        public override string ToString()
        {
            return $"{Coordinate} - {Distance}";
        }
    }
}
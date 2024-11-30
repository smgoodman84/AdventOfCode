namespace AdventOfCode.Shared.Graphs;

public class GraphEdge<TNode> where TNode : IGraphNodeData
{
    public GraphNode<TNode> Source { get; set; }
    public GraphNode<TNode> Destination { get; set; }
    public int Distance { get; set; }
}
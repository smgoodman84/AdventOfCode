namespace AdventOfCode.Shared.Graphs;

public class GraphNode<T> where T : IGraphNodeData
{
	public T Data { get; }

	public GraphNode(T data)
	{
		Data = data;
	}

	public override string ToString()
	{
		return Data.ToString();
	}
}
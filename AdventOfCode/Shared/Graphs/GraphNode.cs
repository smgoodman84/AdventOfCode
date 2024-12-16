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

	public override bool Equals(object obj)
	{
		var node = obj as GraphNode<T>;
		if (node == null)
		{
			return false;
		}
		return node.Data.GetIdentifier().Equals(Data.GetIdentifier());
	}

	public override int GetHashCode()
	{
		return Data.GetIdentifier().GetHashCode();
	}
}
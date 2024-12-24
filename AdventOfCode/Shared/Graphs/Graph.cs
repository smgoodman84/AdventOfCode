using System.Collections.Generic;
using AdventOfCode.Shared.DataStructures;
using System.Linq;

namespace AdventOfCode.Shared.Graphs;

public class Graph<TNode> where TNode : IGraphNodeData
{
    private Dictionary<string, GraphNode<TNode>> _nodes = new Dictionary<string, GraphNode<TNode>>();
    private Dictionary<string, List<GraphEdge<TNode>>> _neighbours = new Dictionary<string, List<GraphEdge<TNode>>>();

    public void AddNode(GraphNode<TNode> node)
    {
        _nodes.Add(node.Data.GetIdentifier(), node);
    }

    public void RemoveNode(GraphNode<TNode> node)
    {
        _nodes.Remove(node.Data.GetIdentifier());
    }

    public List<GraphNode<TNode>> AllNodes()
    {
        return _nodes.Values.ToList();
    }

    public bool TryGetNode(string nodeIdentifier, out GraphNode<TNode> value)
    {
        return _nodes.TryGetValue(nodeIdentifier, out value);
    }

    public void AddEdge(GraphEdge<TNode> edge)
    {
        var sourceIdentifier = edge.Source.Data.GetIdentifier();
        if (!_neighbours.ContainsKey(sourceIdentifier))
        {
            _neighbours.Add(sourceIdentifier, new List<GraphEdge<TNode>>());
        }

        _neighbours[sourceIdentifier].Add(edge);
    }

    public void RemoveEdge(GraphEdge<TNode> edge)
    {
        var sourceIdentifier = edge.Source.Data.GetIdentifier();
        var destinationIdentifier = edge.Destination.Data.GetIdentifier();
        if (!_neighbours.ContainsKey(sourceIdentifier))
        {
            return;
        }

        _neighbours[sourceIdentifier].RemoveAll(e => e.Destination.Data.GetIdentifier() == destinationIdentifier);
    } 

    public int GetLongestPath(GraphNode<TNode> start, GraphNode<TNode> end, int maxDistance)
    {
        // Untested
        throw new System.Exception("Does not work");
        /*
        var queue = new SimplePriorityQueue(maxDistance);
        foreach (var node in _nodes)
        {
            queue.SetPriority(node.Key, 0);
        }

        Dictionary<string, GraphNode<TNode>> _incompleteNodes = _nodes.ToDictionary(n => n.Key, n => n.Value);
        Dictionary<string, GraphNode<TNode>> _previousNodes = _nodes.ToDictionary(n => n.Key, n => (GraphNode<TNode>)null);
        Dictionary<string, int> _distances = _nodes.ToDictionary(n => n.Key, n => 0);

        var startIdentifier = start.Data.GetIdentifier();

        _distances[startIdentifier] = 0;
        queue.SetPriority(startIdentifier, 0);

        while (_incompleteNodes.Any())
        {
            var minimumIncompleteKey = queue.PopReverse();
            _incompleteNodes.Remove(minimumIncompleteKey);

            var minimumIncomplete = _nodes[minimumIncompleteKey];
            var minimumIncompleteNeighbours = GetNeighbours(minimumIncompleteKey);


            // System.Console.WriteLine();
            // System.Console.WriteLine($"Processing {minimumIncompleteKey} ({_distances[minimumIncompleteKey]})");
            foreach (var neighbour in minimumIncompleteNeighbours)
            {
                var neighbourIdentifier = neighbour.Destination.Data.GetIdentifier();
                // System.Console.WriteLine($"Neighbour {neighbourIdentifier}");
                if (_incompleteNodes.ContainsKey(neighbourIdentifier))
                {
                    var alt = _distances[minimumIncompleteKey] + neighbour.Distance;

                    // System.Console.WriteLine($"alt {neighbourIdentifier} = {alt} - {_distances[neighbourIdentifier]}");
                    if (alt > _distances[neighbourIdentifier])
                    {
                        queue.SetPriority(neighbourIdentifier, alt);
                        _distances[neighbourIdentifier] = alt;
                        _previousNodes[neighbourIdentifier] = minimumIncomplete;
                    }
                }
            }
        }

        var endIdentifier = end.Data.GetIdentifier();

        var longestPath = new List<string>();
        var current = endIdentifier;
        while (current != startIdentifier)
        {
            longestPath.Insert(0, current);
            current = _previousNodes[current].Data.GetIdentifier();
        }

        return _distances[endIdentifier];
        */
    }

    public int GetShortestPathDistance(GraphNode<TNode> start, GraphNode<TNode> end, int maxDistance)
    {
        var result = GetShortestPathNodesAndDistance(start, end, maxDistance);

        return result.Distance;
    }

    public List<GraphNode<TNode>> GetShortestPathNodes(GraphNode<TNode> start, GraphNode<TNode> end, int maxDistance)
    {
        var result = GetShortestPathNodesAndDistance(start, end, maxDistance);

        return result.Nodes;
    }

    public (int Distance, List<GraphNode<TNode>> Nodes) GetShortestPathNodesAndDistance(
        GraphNode<TNode> start,
        GraphNode<TNode> end,
        int maxDistance)
    {
        var queue = new SimplePriorityQueue(maxDistance);
        foreach (var node in _nodes)
        {
            queue.SetPriority(node.Key, maxDistance);
        }

        Dictionary<string, GraphNode<TNode>> _incompleteNodes = _nodes.ToDictionary(n => n.Key, n => n.Value);
        Dictionary<string, GraphNode<TNode>> _previousNodes = _nodes.ToDictionary(n => n.Key, n => (GraphNode<TNode>)null);
        Dictionary<string, int> _distances = _nodes.ToDictionary(n => n.Key, n => maxDistance);

        var startIdentifier = start.Data.GetIdentifier();

        _distances[startIdentifier] = 0;
        queue.SetPriority(startIdentifier, 0);

        while (_incompleteNodes.Any())
        {
            var minimumIncompleteKey = queue.Pop();
            _incompleteNodes.Remove(minimumIncompleteKey);

            var minimumIncomplete = _nodes[minimumIncompleteKey];
            var minimumIncompleteNeighbours = GetNeighbours(minimumIncompleteKey);

            foreach (var neighbour in minimumIncompleteNeighbours)
            {
                var neighbourIdentifier = neighbour.Destination.Data.GetIdentifier();
                if (_incompleteNodes.ContainsKey(neighbourIdentifier))
                {
                    var alt = _distances[minimumIncompleteKey] + neighbour.Distance;
                    if (alt < _distances[neighbourIdentifier])
                    {
                        queue.SetPriority(neighbourIdentifier, alt);
                        _distances[neighbourIdentifier] = alt;
                        _previousNodes[neighbourIdentifier] = minimumIncomplete;
                    }
                }
            }
        }

        var endIdentifier = end.Data.GetIdentifier();

        var nodes = new List<GraphNode<TNode>>
        {
            end
        };

        var current = end;
        var currentIdentifier = current.Data.GetIdentifier();
        while (currentIdentifier != startIdentifier)
        {
            current = _previousNodes[currentIdentifier];
            currentIdentifier = current.Data.GetIdentifier();
            nodes.Add(current);
        }

        nodes.Reverse();
        return (_distances[endIdentifier], nodes);
    }

    public List<GraphEdge<TNode>> GetNeighbours(GraphNode<TNode> node)
    {
        return GetNeighbours(node.Data.GetIdentifier());
    }

    public List<GraphEdge<TNode>> GetNeighbours(string nodeKey)
    {
        if (!_neighbours.ContainsKey(nodeKey))
        {
            return new List<GraphEdge<TNode>>();
        }

        return _neighbours[nodeKey];
    }
}
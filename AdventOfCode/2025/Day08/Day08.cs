using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.Numbers;

namespace AdventOfCode._2025.Day08;

public class Day08 : Day
{
    public Day08() : base(2025, 8, "Day08/input_2025_08.txt", "57970", "8520040659", false)
    {

    }

    private List<JunctionBox> _junctionBoxes;
    private List<DistanceInfo> _distances;
    private List<Circuit> _circuits;
    public override void Initialise()
    {
        _circuits = InputLines
            .Select((l, ix) => new Circuit(ix))
            .ToList();
        
        _junctionBoxes = InputLines
            .Select((l, ix) => new JunctionBox(new Coordinate3D(l), ix, _circuits[ix]))
            .ToList();
        
        foreach (var box in _junctionBoxes)
        {
            _circuits[box.Id].Boxes.Add(box);
        }
    }

    public override string Part1()
    {
        _distances = new List<DistanceInfo>();
        for (var i = 0; i < _junctionBoxes.Count; i++)
        {
            for (var j = 0; j < i; j++)
            {
                _distances.Add(new DistanceInfo(_junctionBoxes[i], _junctionBoxes[j]));
            }
        }

        var orderedDistance = _distances.OrderBy(d => d.Distance).ToList();
        foreach (var connection in orderedDistance.Take(1000))
        {
            MergeCircuits(connection.Box1, connection.Box2);
        }

        var orderedCircuits = _circuits.OrderByDescending(c => c.Size).ToList();

        var result = 1;
        foreach (var circuit in orderedCircuits.Take(3))
        {
            result *= circuit.Size;
        }

        return result.ToString();
    }

    private void MergeCircuits(JunctionBox connectionBox1, JunctionBox connectionBox2)
    {
        var circuit1 = connectionBox1.Circuit;
        var circuit2 = connectionBox2.Circuit;

        TraceLine($"Merging circuits {circuit1.Id} and {circuit2.Id} - boxes {connectionBox1.Location} and {connectionBox2.Location}");
        if (circuit1 == circuit2)
        {
            TraceLine($"Nothing to do - already the same circuit");
            return;
        }
        
        foreach (var box in circuit2.Boxes)
        {
            box.Circuit = circuit1;
        }
        
        circuit1.Boxes.AddRange(circuit2.Boxes);
        circuit2.Boxes.Clear();
        
        
        TraceLine($"Circuit {circuit1.Id} is now {circuit1.Size} boxes");
        TraceLine($"Circuit {circuit2.Id} is now {circuit2.Size} boxes");
    }

    public override string Part2()
    {
        var orderedDistances = _distances.OrderBy(d => d.Distance).ToList();
        foreach (var connection in orderedDistances)
        {
            MergeCircuits(connection.Box1, connection.Box2);
            if (connection.Box1.Circuit.Size == _junctionBoxes.Count)
            {
                var result = connection.Box1.Location.X * connection.Box2.Location.X;
                return result.ToString();
            }
        }

        return string.Empty;
    }
    
    private class DistanceInfo(JunctionBox box1, JunctionBox box2)
    {
        public JunctionBox Box1 = box1;
        public JunctionBox Box2 = box2;
        public double Distance = box1.DistanceTo(box2);
    }

    private class Circuit(int id)
    {
        public readonly int Id = id;
        public List<JunctionBox> Boxes = new List<JunctionBox>();
        public int Size => Boxes.Count;
    }

    private class JunctionBox(Coordinate3D location, int id, Circuit circuit)
    {
        public readonly Coordinate3D Location = location;
        public readonly int Id = id;
        public Circuit Circuit = circuit;

        public double DistanceTo(JunctionBox that)
        {
            return Location.DistanceTo(that.Location);
        }
    }
}
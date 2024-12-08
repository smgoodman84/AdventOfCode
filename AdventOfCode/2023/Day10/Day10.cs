using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2023.Day10;

public class Day10 : Day
{
    public Day10() : base(2023, 10, "Day10/input_2023_10.txt", "6738", "579", false)
    {

    }

    private Grid2D<Node> _map;
    private Coordinate2D _start;
    public override void Initialise()
    {
        _map = new Grid2D<Node>(InputLines[0].Length, InputLines.Count);

        {
            var y = InputLines.Count - 1;
            foreach (var line in InputLines)
            {
                var x = 0;
                foreach (var c in line)
                {
                    _map.Write(x, y, new Node(c));
                    x += 1;
                }
                y -= 1;
            }
        }

        foreach (var y in Enumerable.Range((int)_map.MinY, (int)_map.Height))
        {
            foreach (var x in Enumerable.Range((int)_map.MinX, (int)_map.Width))
            {
                var node = _map.Read(x, y);
                switch (node.Type)
                {
                    case '|':
                        node.North = _map.IsInGrid(x, y + 1) ? _map.Read(x, y + 1) : null;
                        node.South = _map.IsInGrid(x, y - 1) ? _map.Read(x, y - 1) : null;
                        break;
                    case '-':
                        node.East = _map.IsInGrid(x + 1, y) ? _map.Read(x + 1, y) : null;
                        node.West = _map.IsInGrid(x - 1, y) ? _map.Read(x - 1, y) : null;
                        break;
                    case 'L':
                        node.North = _map.IsInGrid(x, y + 1) ? _map.Read(x, y + 1) : null;
                        node.East = _map.IsInGrid(x + 1, y) ? _map.Read(x + 1, y) : null;
                        break;
                    case 'J':
                        node.North = _map.IsInGrid(x, y + 1) ? _map.Read(x, y + 1) : null;
                        node.West = _map.IsInGrid(x - 1, y) ? _map.Read(x - 1, y) : null;
                        break;
                    case '7':
                        node.South = _map.IsInGrid(x, y - 1) ? _map.Read(x, y - 1) : null;
                        node.West = _map.IsInGrid(x - 1, y) ? _map.Read(x - 1, y) : null;
                        break;
                    case 'F':
                        node.South = _map.IsInGrid(x, y - 1) ? _map.Read(x, y - 1) : null;
                        node.East = _map.IsInGrid(x + 1, y) ? _map.Read(x + 1, y) : null;
                        break;
                    case 'S':
                        _start = new Coordinate2D(x, y);
                        _map.Read(x, y).MinDistance = 0;
                        break;
                }
            }
        }
    }

    public override string Part1()
    {
        (var anyOnLoop, var maxDistance) = GetMaxDistance(_start, null, 0);

        var furthest = (maxDistance ?? 0) / 2;

        return furthest.ToString();
    }

    private (bool OnLoop, int? Distance) GetMaxDistance(Coordinate2D current, Coordinate2D previous, int currentDistance)
    {
        var currentNode = _map.Read(current);
        if (current.Equals(_start) && previous != null)
        {
            TraceLine($"Loop Complete: {currentDistance}");
            currentNode.OnLoop = true;
            return (true, currentDistance);
        }

        if (!currentNode.MinDistance.HasValue || currentDistance < currentNode.MinDistance)
        {
            currentNode.MinDistance = currentDistance;
        }

        var north = current.North();
        var south = current.South();
        var east = current.East();
        var west = current.West();

        int? maxNorth = null;
        int? maxSouth = null;
        int? maxEast = null;
        int? maxWest = null;

        var anyOnLoop = false;
        if ((previous == null || !previous.Equals(north)) && CanMoveNorth(current))
        {
            // TraceLine($"({current.X},{current.Y}) = {currentDistance} - Going North");
            (var onLoop, maxNorth) = GetMaxDistance(north, current, currentDistance + 1);
            if (onLoop)
            {
                anyOnLoop = true;
                _map.Read(north).OnLoop = true;
            }
            // TraceLine($"({current.X},{current.Y}) = {currentDistance} - North {maxNorth}");
        }

        if ((previous == null || !previous.Equals(south)) && CanMoveSouth(current))
        {
            // TraceLine($"({current.X},{current.Y}) = {currentDistance} - Going South");
            (var onLoop, maxSouth) = GetMaxDistance(south, current, currentDistance + 1);
            if (onLoop)
            {
                anyOnLoop = true;
                _map.Read(south).OnLoop = true;
            }
            // TraceLine($"({current.X},{current.Y}) = {currentDistance} - South {maxSouth}");
        }

        if ((previous == null || !previous.Equals(east)) && CanMoveEast(current))
        {
            // TraceLine($"({current.X},{current.Y}) = {currentDistance} - Going East");
            (var onLoop, maxEast) = GetMaxDistance(east, current, currentDistance + 1);
            if (onLoop)
            {
                anyOnLoop = true;
                _map.Read(east).OnLoop = true;
            }
            // TraceLine($"({current.X},{current.Y}) = {currentDistance} - East {maxEast}");
        }

        if ((previous == null || !previous.Equals(west)) && CanMoveWest(current))
        {
            // TraceLine($"({current.X},{current.Y}) = {currentDistance} - Going West");
            (var onLoop, maxWest) = GetMaxDistance(west, current, currentDistance + 1);
            if (onLoop)
            {
                anyOnLoop = true;
                _map.Read(west).OnLoop = true;
            }
            // TraceLine($"({current.X},{current.Y}) = {currentDistance} - West {maxWest}");
        }

        var maxDistance = new[] { maxNorth, maxSouth, maxEast, maxWest }
            .Where(x => x.HasValue)
            .Select(x => x.Value)
            .Max();

        return (anyOnLoop, maxDistance);
    }

    public override string Part2()
    {
        var insideLoop = 0;
        foreach (var y in Enumerable.Range((int)_map.MinY, (int)_map.Height))
        {
            var inside = false;
            var moveIn = ' ';
            var moveOut = ' ';
            foreach (var x in Enumerable.Range((int)_map.MinX, (int)_map.Width))
            {
                var node = _map.Read(x, y);
                if (inside && !node.OnLoop)
                {
                    insideLoop += 1;
                    node.Inside = true;
                    TraceLine($"{x},{y} - Inside");
                }

                if (y == 5) TraceLine($"{x} - {node.Type}: Inside - {inside}");
                if (node.OnLoop)
                {
                    if (node.Type == moveIn)
                    {
                        if (y == 5) TraceLine($"{x} - {node.Type}: Moved In");
                        inside = true;
                        moveIn = ' ';
                        moveOut = ' ';
                    }

                    if (node.Type == moveOut)
                    {
                        if (y == 5) TraceLine($"{x} - {node.Type}: Moved Out");
                        inside = false;
                        moveIn = ' ';
                        moveOut = ' ';
                    }

                    if (node.Type == '|' || node.Type == 'S')
                    {
                        inside = !inside;
                        if (y == 5) TraceLine($"{x} - {node.Type}: Flipped");
                        moveIn = ' ';
                        moveOut = ' ';
                    }

                    if (node.Type == 'L')
                    {
                        if (inside)
                        {
                            moveOut = '7';
                            moveIn = 'J';
                        }
                        else
                        {
                            moveOut = 'J';
                            moveIn = '7';
                        }
                    }

                    if (node.Type == 'F')
                    {
                        if (inside)
                        {
                            moveOut = 'J';
                            moveIn = '7';
                        }
                        else
                        {
                            moveOut = '7';
                            moveIn = 'J';
                        }
                    }
                }
            }
        }



        foreach (var y in Enumerable.Range((int)_map.MinY, (int)_map.Height).OrderByDescending(x => x))
        {
            Trace($"{y.ToString().PadLeft(3, '0')}");
            foreach (var x in Enumerable.Range((int)_map.MinX, (int)_map.Width))
            {
                var node = _map.Read(x, y);
                if (node.OnLoop)
                {
                    Trace(BoxLookup(node.Type));
                }
                else if (node.Inside)
                {
                    Trace('X');
                }
                else if (node.Type == '.')
                {
                    Trace('.');
                }
                else
                {
                    Trace(' ');
                }
            }
            TraceLine();
        }

        return insideLoop.ToString();
    }

    private char BoxLookup(char c)
    {
        switch (c)
        {
            case 'J': return '┘';
            case 'F': return '┌';
            case 'L': return '└';
            case '7': return '┐';
        }

        return c;
    }

    private bool CanMoveNorth(Coordinate2D coordinate)
    {
        var northCoordinate = coordinate.North();
        if (!_map.IsInGrid(northCoordinate))
        {
            return false;
        }

        var here = _map.Read(coordinate);
        var north = _map.Read(northCoordinate);

        return (here.North != null || here.Type == 'S')
               && (north.South != null || north.Type == 'S');
    }

    private bool CanMoveSouth(Coordinate2D coordinate)
    {
        var southCoordinate = coordinate.South();
        if (!_map.IsInGrid(southCoordinate))
        {
            return false;
        }

        var here = _map.Read(coordinate);
        var south = _map.Read(southCoordinate);

        return (here.South != null || here.Type == 'S')
               && (south.North != null || south.Type == 'S');
    }

    private bool CanMoveEast(Coordinate2D coordinate)
    {
        var eastCoordinate = coordinate.East();
        if (!_map.IsInGrid(eastCoordinate))
        {
            return false;
        }

        var here = _map.Read(coordinate);
        var east = _map.Read(eastCoordinate);

        return (here.East != null || here.Type == 'S')
               && (east.West != null || east.Type == 'S');
    }

    private bool CanMoveWest(Coordinate2D coordinate)
    {
        var westCoordinate = coordinate.West();
        if (!_map.IsInGrid(westCoordinate))
        {
            return false;
        }

        var here = _map.Read(coordinate);
        var west = _map.Read(westCoordinate);

        return (here.West != null || here.Type == 'S')
               && (west.East != null || west.Type == 'S');
    }

    private class Node
    {
        public Node(char type)
        {
            Type = type;
        }

        public char Type { get; set; }
        public bool OnLoop { get; set; } = false;
        public bool Inside { get; set; } = false;
        public int? MinDistance { get; set; }

        public Node North { get; set; }
        public Node East { get; set; }
        public Node South { get; set; }
        public Node West { get; set; }
    }
}
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2023.Day10
{
    public class Day10 : Day
    {
        public Day10() : base(2023, 10, "Day10/input_2023_10.txt", "", "", true)
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
                            break;
                    }
                }
            }
        }

        public override string Part1()
        {
            var maxDistance = GetMaxDistance(_start, null, 0) ?? 0;

            var furthest = maxDistance / 2;

            return furthest.ToString();
        }

        private int? GetMaxDistance(Coordinate2D current, Coordinate2D? previous, int currentDistance)
        {
            if (currentDistance > 100000)
            {
                return 100000;
            }

            if (current.Equals(_start) && previous != null)
            {
                TraceLine($"Loop Complete: {currentDistance}");
                return currentDistance;
            }

            var north = current.North();
            var south = current.South();
            var east = current.East();
            var west = current.West();

            int? maxNorth = null;
            int? maxSouth = null;
            int? maxEast = null;
            int? maxWest = null;

            if ((previous == null || !previous.Equals(north)) && CanMoveNorth(current))
            {
                TraceLine($"({current.X},{current.Y}) = {currentDistance} - Going North");
                maxNorth = GetMaxDistance(north, current, currentDistance + 1);
                TraceLine($"({current.X},{current.Y}) = {currentDistance} - North {maxNorth}");
            }

            if ((previous == null || !previous.Equals(south)) && CanMoveSouth(current))
            {
                TraceLine($"({current.X},{current.Y}) = {currentDistance} - Going South");
                maxSouth = GetMaxDistance(south, current, currentDistance + 1);
                TraceLine($"({current.X},{current.Y}) = {currentDistance} - South {maxSouth}");
            }

            if ((previous == null || !previous.Equals(east)) && CanMoveEast(current))
            {
                TraceLine($"({current.X},{current.Y}) = {currentDistance} - Going East");
                maxEast = GetMaxDistance(east, current, currentDistance + 1);
                TraceLine($"({current.X},{current.Y}) = {currentDistance} - East {maxEast}");
            }

            if ((previous == null || !previous.Equals(west)) && CanMoveWest(current))
            {
                TraceLine($"({current.X},{current.Y}) = {currentDistance} - Going West");
                maxWest = GetMaxDistance(west, current, currentDistance + 1);
                TraceLine($"({current.X},{current.Y}) = {currentDistance} - West {maxWest}");
            }

            return new[] { maxNorth, maxSouth, maxEast, maxWest }
                .Where(x => x.HasValue)
                .Select(x => x.Value)
                .Max();
        }

        public override string Part2()
        {
            return string.Empty;
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
            public int X { get; set; }
            public int Y { get; set; }

            public Node? North { get; set; }
            public Node? East { get; set; }
            public Node? South { get; set; }
            public Node? West { get; set; }
        }
    }
}


using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2024.Day15;

public class Day15 : Day
{
    public Day15() : base(2024, 15, "Day15/input_2024_15.txt", "1517819", "1538862", true)
    {
    }

    private Warehouse _warehouse;
    private Warehouse _warehousePartTwo;
    private List<Direction> _robotMovements = new ();
    public override void Initialise()
    {
        var mapLines = new List<string>();
        var movementLines = new List<string>();
        var pastBlank = false;
        foreach (var line in InputLines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                pastBlank = true;
                continue;
            }

            if (!pastBlank)
            {
                mapLines.Add(line);
            }
            else
            {
                movementLines.Add(line);
            }
        }

        _warehouse = new Warehouse(mapLines);

        var mapLinesPartTwo = mapLines
            .Select(line => string.Join("", line.Select(MapInputToPartTwo)))
            .ToList();

        _warehousePartTwo = new Warehouse(mapLinesPartTwo);

        _robotMovements = movementLines
            .SelectMany(line => line.Select(MapDirection))
            .ToList();
    }

    private string MapInputToPartTwo(char input)
    {
        switch (input)
        {
            case '#': return "##";
            case 'O': return "[]";
            case '@': return "@.";
            case '.': return "..";
        }

        throw new Exception("Unknown location type");
    }

    private Direction MapDirection(char c)
    {
        switch (c)
        {
            case '^': return Direction.Up;
            case 'v': return Direction.Down;
            case '<': return Direction.Left;
            case '>': return Direction.Right;
        }

        throw new Exception("Unknown direction");
    }

    public override string Part1()
    {
        // _warehouse.Render();
        foreach (var direction in _robotMovements)
        {
            _warehouse.Move(direction);
            // _warehouse.Render();
        }

        var result = _warehouse.GetGpsCoordinateTotal();

        return result.ToString();
    }

    public override string Part2()
    {
        var step = 0;

        // Console.WriteLine($"Step {step}");
        _warehousePartTwo.Render();
        foreach (var direction in _robotMovements)
        {
            step += 1;
            _warehousePartTwo.MovePartTwo(direction);

            // Console.WriteLine($"Step {step} {direction}");
            _warehousePartTwo.Render();
        }

        var result = _warehousePartTwo.GetGpsCoordinateTotal();

        return result.ToString();
    }

    private class Warehouse
    {
        private Grid2D<WarehouseLocation> _warehouseMap;
        private Coordinate2D _robotLocation;
        public Warehouse(List<string> mapLines)
        {
            _warehouseMap = Grid2D<WarehouseLocation>.CreateWithScreenCoordinates(
                mapLines,
                (coord, c) => new WarehouseLocation(coord, c)
                );

            _robotLocation = _warehouseMap.ReadAll()
                .First(x => x.LocationType == LocationType.Robot)
                .Location;
        }

        public void Render()
        {
            return;

            foreach (var y in _warehouseMap.YIndexes())
            {
                foreach (var x in _warehouseMap.XIndexes())
                {
                    Console.Write(LocationTypeChar(_warehouseMap.Read(x, y).LocationType));
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private char LocationTypeChar(LocationType locationType)
        {
            switch (locationType)
            {
                case LocationType.Wall: return '#';
                case LocationType.Box: return 'O';
                case LocationType.Robot: return '@';
                case LocationType.Empty: return '.';
                case LocationType.BoxLeft: return '[';
                case LocationType.BoxRight: return ']';
            }

            throw new Exception("Unknown location type");
        }

        public void Move(Direction direction)
        {
            var currentLocation = _robotLocation;
            var robotWarehouse = _warehouseMap.Read(_robotLocation);

            var firstNeighbour = _robotLocation.Neighbour(direction);
            var firstWarehouse = _warehouseMap.Read(firstNeighbour);
            
            while (true)
            {
                currentLocation = currentLocation.Neighbour(direction);
                var currentWarehouse = _warehouseMap.Read(currentLocation);

                if (currentWarehouse.LocationType == LocationType.Wall)
                {
                    return;
                }

                if (currentWarehouse.LocationType == LocationType.Empty)
                {
                    _robotLocation = firstNeighbour;
                    robotWarehouse.LocationType = LocationType.Empty;
                    currentWarehouse.LocationType = LocationType.Box;
                    firstWarehouse.LocationType = LocationType.Robot;

                    return;
                }
            }
        }

        public void MovePartTwo(Direction direction)
        {
            if (direction == Direction.Up || direction == Direction.Down)
            {
                MovePartTwoUpDown(direction);
                return;
            }

            MovePartTwoLeftRight(direction);
        }

        public void MovePartTwoUpDown(Direction direction)
        {
            var currentLocation = _robotLocation;
            var robotWarehouse = _warehouseMap.Read(_robotLocation);

            var firstNeighbour = _robotLocation.Neighbour(direction);
            var firstWarehouse = _warehouseMap.Read(firstNeighbour);
            
            if (CanMoveTo(firstWarehouse, direction))
            {
                MoveTo(LocationType.Robot, firstWarehouse, direction);
                robotWarehouse.LocationType = LocationType.Empty;
                _robotLocation = firstNeighbour;
            }
        }

        private bool CanMoveTo(WarehouseLocation location, Direction direction)
        {
            if (location.LocationType == LocationType.Empty)
            {
                return true;
            }

            if (location.LocationType == LocationType.BoxLeft)
            {
                var boxRightLocation = location.Location.Right();
                return CanMoveTo(_warehouseMap.Read(location.Location.Neighbour(direction)), direction)
                    && CanMoveTo(_warehouseMap.Read(boxRightLocation.Neighbour(direction)), direction);
            }

            if (location.LocationType == LocationType.BoxRight)
            {
                var boxLeftLocation = location.Location.Left();
                return CanMoveTo(_warehouseMap.Read(location.Location.Neighbour(direction)), direction)
                    && CanMoveTo(_warehouseMap.Read(boxLeftLocation.Neighbour(direction)), direction);
            }

            return false;
        }

        private void MoveTo(LocationType locationType, WarehouseLocation location, Direction direction)
        {
            // Console.WriteLine($"Moving {locationType} to {location}");

            if (location.LocationType == LocationType.BoxLeft)
            {
                var boxLeft = location;
                var boxLeftLocation = boxLeft.Location;
                var boxRightLocation = boxLeftLocation.Right();
                var boxRight = _warehouseMap.Read(boxRightLocation);

                MoveTo(LocationType.BoxLeft, _warehouseMap.Read(boxLeft.Location.Neighbour(direction)), direction);
                boxLeft.LocationType = LocationType.Empty;

                MoveTo(LocationType.BoxRight, _warehouseMap.Read(boxRight.Location.Neighbour(direction)), direction);
                boxRight.LocationType = LocationType.Empty;

                Render();
            }

            if (location.LocationType == LocationType.BoxRight)
            {
                var boxRight = location;
                var boxRightLocation = boxRight.Location;
                var boxLeftLocation = boxRightLocation.Left();
                var boxLeft = _warehouseMap.Read(boxLeftLocation);

                MoveTo(LocationType.BoxLeft, _warehouseMap.Read(boxLeft.Location.Neighbour(direction)), direction);
                boxLeft.LocationType = LocationType.Empty;

                MoveTo(LocationType.BoxRight, _warehouseMap.Read(boxRight.Location.Neighbour(direction)), direction);
                boxRight.LocationType = LocationType.Empty;

                Render();
            }

            if (location.LocationType == LocationType.Empty)
            {
                // Should always be empty
                location.LocationType = locationType;
            }
        }

        public void MovePartTwoLeftRight(Direction direction)
        {
            var currentLocation = _robotLocation;
            var robotWarehouse = _warehouseMap.Read(_robotLocation);

            var firstNeighbour = _robotLocation.Neighbour(direction);
            var firstWarehouse = _warehouseMap.Read(firstNeighbour);

            var previousLocations = new Stack<WarehouseLocation>();
            
            while (true)
            {
                currentLocation = currentLocation.Neighbour(direction);
                var currentWarehouse = _warehouseMap.Read(currentLocation);

                if (currentWarehouse.LocationType == LocationType.Wall)
                {
                    return;
                }

                if (currentWarehouse.LocationType == LocationType.BoxLeft
                    || currentWarehouse.LocationType == LocationType.BoxRight)
                {
                    previousLocations.Push(currentWarehouse);
                    continue;
                }

                if (currentWarehouse.LocationType == LocationType.Empty)
                {
                    _robotLocation = firstNeighbour;
                    while (previousLocations.TryPop(out var previousLocation))
                    {
                        currentWarehouse.LocationType = previousLocation.LocationType;
                        currentWarehouse = previousLocation;
                    }
                    robotWarehouse.LocationType = LocationType.Empty;
                    firstWarehouse.LocationType = LocationType.Robot;

                    return;
                }
            }
        }

        public long GetGpsCoordinateTotal()
        {
            var result = _warehouseMap
                .ReadAll()
                .Sum(x => x.GpsCoordinate());

            return result;
        }
    }

    private class WarehouseLocation
    {
        public Coordinate2D Location { get; set; }
        public LocationType LocationType { get; set; }
        public WarehouseLocation(Coordinate2D location, char locationType)
        {
            Location = location;
            LocationType = GetLocationType(locationType);
        }

        public long GpsCoordinate()
        {
            if (LocationType == LocationType.Box
                || LocationType == LocationType.BoxLeft)
            {
                return Location.Y * 100 + Location.X;
            }

            return 0;
        }

        private LocationType GetLocationType(char locationType)
        {
            switch (locationType)
            {
                case '#': return LocationType.Wall;
                case 'O': return LocationType.Box;
                case '@': return LocationType.Robot;
                case '.': return LocationType.Empty;
                case '[': return LocationType.BoxLeft;
                case ']': return LocationType.BoxRight;
            }

            throw new Exception("Unknown location type");
        }

        public override string ToString()
        {
            return $"{Location} {LocationType}";
        }
    }

    private enum LocationType
    {
        Wall,
        Box,
        BoxLeft,
        BoxRight,
        Robot,
        Empty,
    }
}
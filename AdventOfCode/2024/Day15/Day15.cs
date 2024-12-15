using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2024.Day15;

public class Day15 : Day
{
    public Day15() : base(2024, 15, "Day15/input_2024_15.txt", "1517819", "", true)
    {
    }

    private Warehouse _warehouse;
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

        _robotMovements = movementLines
            .SelectMany(line => line.Select(MapDirection))
            .ToList();
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
        return string.Empty;
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
            }

            throw new Exception("Unknown location type");
        }

        public void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up: Move(x => x.Up()); break;
                case Direction.Down: Move(x => x.Down()); break;
                case Direction.Left: Move(x => x.Left()); break;
                case Direction.Right: Move(x => x.Right()); break;
            }
        }

        public void Move(Func<Coordinate2D, Coordinate2D> getNeighbour)
        {
            var currentLocation = _robotLocation;
            var robotWarehouse = _warehouseMap.Read(_robotLocation);

            var firstNeighbour = getNeighbour(_robotLocation);
            var firstWarehouse = _warehouseMap.Read(firstNeighbour);
            
            while (true)
            {
                currentLocation = getNeighbour(currentLocation);
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
            if (LocationType != LocationType.Box)
            {
                return 0;
            }

            return Location.Y * 100 + Location.X;
        }

        private LocationType GetLocationType(char locationType)
        {
            switch (locationType)
            {
                case '#': return LocationType.Wall;
                case 'O': return LocationType.Box;
                case '@': return LocationType.Robot;
                case '.': return LocationType.Empty;
            }

            throw new Exception("Unknown location type");
        }
    }

    private enum LocationType
    {
        Wall,
        Box,
        Robot,
        Empty,
    }
}
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;
using AdventOfCode.Shared.PrimitiveExtensions;

namespace AdventOfCode._2024.Day08;

public class Day08 : Day
{
    public Day08() : base(2024, 8, "Day08/input_2024_08.txt", "341", "1134")
    {

    }

    private List<Antenna> _antennas = new List<Antenna>();
    private List<char> _frequencies = new List<char>();
    private int _height = 0;
    private int _width = 0;

    public override void Initialise()
    {
        _height = InputLines.Count;
        _width = InputLines[0].Length;

        var y = 0;
        foreach (var line in InputLines)
        {
            var x = 0;
            foreach (var c in line)
            {
                if (c != '.')
                {
                    _antennas.Add(new Antenna(c, x, y));
                }

                x += 1;
            }
            
            y += 1;
        }

        _frequencies = _antennas
            .Select(a => a.Frequency)
            .Distinct()
            .ToList();
    }

    public override string Part1()
    {
        var allAntinodes = new Dictionary<char, List<Coordinate2D>>();
        foreach (var frequency in _frequencies)
        {
            var antinodes = new List<Coordinate2D>();

            var antennas = _antennas
                .Where(a => a.Frequency == frequency)
                .ToList();

            foreach ((var antennaA, var antennaB) in antennas.AllPairs())
            {
                var delta1 = antennaA.Location.Subtract(antennaB.Location);
                var coordinate1 = antennaA.Location.Add(delta1);
                if (IsInGrid(coordinate1))
                {
                    antinodes.Add(coordinate1);
                }

                var delta2 = antennaB.Location.Subtract(antennaA.Location);
                var coordinate2 = antennaB.Location.Add(delta2);
                if (IsInGrid(coordinate2))
                {
                    antinodes.Add(coordinate2);
                }
            }

            allAntinodes.Add(frequency, antinodes);
        }
        
        var distinctAntinodes = allAntinodes
            .SelectMany(x => x.Value)
            .Distinct()
            .ToList();

        var antinodeCount = distinctAntinodes.Count();

        return antinodeCount.ToString();
    }

    public override string Part2()
    {
        var allAntinodes = new Dictionary<char, List<Coordinate2D>>();
        foreach (var frequency in _frequencies)
        {
            var antinodes = new List<Coordinate2D>();

            var antennas = _antennas
                .Where(a => a.Frequency == frequency)
                .ToList();

            foreach ((var antennaA, var antennaB) in antennas.AllPairs())
            {
                var delta1 = antennaA.Location.Subtract(antennaB.Location);
                var coordinate1 = antennaB.Location.Add(delta1);
                while (IsInGrid(coordinate1))
                {
                    antinodes.Add(coordinate1);
                    coordinate1 = coordinate1.Add(delta1);
                }

                var delta2 = antennaB.Location.Subtract(antennaA.Location);
                var coordinate2 = antennaA.Location.Add(delta2);
                while (IsInGrid(coordinate2))
                {
                    antinodes.Add(coordinate2);
                    coordinate2 = coordinate2.Add(delta2);
                }
            }

            allAntinodes.Add(frequency, antinodes);
        }

        var distinctAntinodes = allAntinodes
            .SelectMany(x => x.Value)
            .Distinct()
            .ToList();

        var antinodeCount = distinctAntinodes.Count();

        return antinodeCount.ToString();
    }

    private bool IsInGrid(Coordinate2D coordinate)
    {
        return coordinate.X >= 0 && coordinate.X < _width
            && coordinate.Y >= 0 && coordinate.Y < _height;
    }

    private class Antenna
    {
        public char Frequency { get; set; }
        public Coordinate2D Location { get; set; }

        public Antenna(char frequency, int x, int y)
        {
            Frequency = frequency;
            Location = new Coordinate2D(x, y);
        }

        public override string ToString()
        {
            return $"{Frequency} {Location}";
        }
    }
}
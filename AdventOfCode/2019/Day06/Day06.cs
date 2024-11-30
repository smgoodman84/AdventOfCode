using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2019.Day06;

public class Day06 : Day
{
    public Day06() : base(2019, 6, "Day06/input_2019_06.txt", "294191", "424")
    {

    }

    private IEnumerable<Orbit> _orbits;
    public override void Initialise()
    {
        _orbits = InputLines
            .Select(Orbit.Parse); ;
    }

    public override string Part1()
    {
        return CountOrbits().ToString();
    }

    public override string Part2()
    {
        return CalculateOrbitalTransfers("YOU", "SAN").ToString();
    }

    public int CountOrbits()
    {
        return new OrbitCounter(_orbits).GetOrbitCount();
    }

    public int CalculateOrbitalTransfers(string start, string end)
    {
        var orbiterDictionary = _orbits.ToDictionary(o => o.Orbiter, o => o.Orbited);
        var startPath = GetPath(orbiterDictionary, start);
        var endPath = GetPath(orbiterDictionary, end);

        var commonPath = new List<string>();
        var index = 0;
        while (startPath[index] == endPath[index])
        {
            commonPath.Add(startPath[index]);
            index += 1;
        }

        return startPath.Length + endPath.Length - (commonPath.Count() * 2);
    }

    private string[] GetPath(Dictionary<string, string> orbiterDictionary, string location)
    {
        var path = new List<string>();
        while (orbiterDictionary.ContainsKey(location))
        {
            location = orbiterDictionary[location];
            path.Add(location);
        }
        return path.ToArray().Reverse().ToArray();
    }

    private class OrbitCounter
    {
        private Dictionary<string, List<string>> _orbitTree;
        private Dictionary<string, int> _orbitCounts = new Dictionary<string, int>();
        public OrbitCounter(IEnumerable<Orbit> orbits)
        {
            _orbitTree = new Dictionary<string, List<string>>();
            foreach (var orbit in orbits)
            {
                if (!_orbitTree.ContainsKey(orbit.Orbited))
                {
                    _orbitTree.Add(orbit.Orbited, new List<string>());
                }

                _orbitTree[orbit.Orbited].Add(orbit.Orbiter);
            }
        }

        public int GetOrbitCount()
        {
            return _orbitTree.Keys.Sum(CalculateOrbitCount);
        }

        private int CalculateOrbitCount(string orbited)
        {
            if (_orbitCounts.ContainsKey(orbited))
            {
                return _orbitCounts[orbited];
            }

            var result = 0;
            if (_orbitTree.ContainsKey(orbited))
            {
                foreach (var orbiter in _orbitTree[orbited])
                {
                    result += 1 + CalculateOrbitCount(orbiter);
                }
            }

            _orbitCounts.Add(orbited, result);

            return result;
        }
    }

    private class Orbit
    {
        public string Orbiter { get; private set; }
        public string Orbited { get; private set; }
        public static Orbit Parse(string orbit)
        {
            var objects = orbit.Split(")");
            return new Orbit()
            {
                Orbited = objects[0],
                Orbiter = objects[1]
            };
        }
    }
}
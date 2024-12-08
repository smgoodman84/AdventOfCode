using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2020.Day13;

public class Day13 : Day
{
    public Day13() : base(2020, 13, "Day13/input_2020_13.txt", "4808", string.Empty)
    {

    }

    private int _earliestDeparture;
    private List<int> _busIds;
    private List<BusTime> _busTimes;

    public override void Initialise()
    {
        _earliestDeparture = int.Parse(InputLines[0]);

        _busIds = InputLines[1].Split(',')
            .Where(lines => lines != "x")
            .Select(int.Parse)
            .ToList();

        _busTimes = InputLines[1].Split(',')
            .Select((l, i) => new BusTime
            {
                BusId = l == "x" ? -1 : int.Parse(l),
                Time = i
            })
            .Where(b => b.BusId != -1)
            .ToList();
    }

    public override string Part1()
    {
        var buses = _busIds
            .Select(CalculateNextDeparture)
            .ToList();

        var nextBus = buses
            .OrderBy(b => b.NextDeparture)
            .First();

        var wait = nextBus.NextDeparture - _earliestDeparture;
        return (wait * nextBus.BusId).ToString();
    }

    private Bus CalculateNextDeparture(int busId)
    {
        var divider = _earliestDeparture / busId;
        var nextDeparture = busId * divider;
        if (nextDeparture < _earliestDeparture)
        {
            nextDeparture += busId;
        }

        return new Bus
        {
            NextDeparture = nextDeparture,
            BusId = busId
        };
    }

    public override string Part2()
    {
        return string.Empty;
        /*
        var biggestBusId = _busTimes.OrderByDescending(bt => bt.BusId).First();

        var timestampForBiggestBus = AllTheLongs()
            .Select(x => x * biggestBusId.BusId)
            .First(t => MeetsRequirements(t, biggestBusId));

        var result = timestampForBiggestBus - biggestBusId.Time;

        return result.ToString();
        */
    }


    /*
    7,13,x,x,59,x,31,19


    7 * x1 = t
    13 * x2 = t + 1
    59 * x3 = t + 4
    31 * x4 = t + 6
    19 * x5 = t + 7

    */

    private IEnumerable<long> AllTheLongs()
    {
        long i = 0;
        while (i <= long.MaxValue)
        {
            yield return i;
            i += 1;
        }
    }

    private bool MeetsRequirements(long biggestBusTimestamp, BusTime biggestBusId)
    {
        foreach(var busTime in _busTimes)
        {
            var requiredTime = biggestBusTimestamp - biggestBusId.Time + busTime.Time;
            if (requiredTime % busTime.BusId != 0)
            {
                return false;
            }
        }

        return true;
    }

    private class Bus
    {
        public int BusId { get; set; }
        public int NextDeparture { get; set; }
    }

    private class BusTime
    {
        public long BusId { get; set; }
        public long Time { get; set; }
    }
}
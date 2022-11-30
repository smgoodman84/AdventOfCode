using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2020.Day16
{
    public class Day16 : Day
    {
        public Day16() : base(2020, 16, "Day16/input_2020_16.txt", "23925", "964373157673")
        {

        }

        private List<FieldRanges> _fieldRanges;
        private TicketData _yourTicket;
        private List<TicketData> _nearbyTickets;

        public override void Initialise()
        {
            var index = 0;

            _fieldRanges = new List<FieldRanges>();
            while (!string.IsNullOrWhiteSpace(InputLines[index]))
            {
                _fieldRanges.Add(new FieldRanges(InputLines[index]));
                index += 1;
            }

            index += 2;
            _yourTicket = new TicketData(InputLines[index]);

            index += 3;
            _nearbyTickets = new List<TicketData>();
            while (index < InputLines.Count)
            {
                _nearbyTickets.Add(new TicketData(InputLines[index]));
                index += 1;
            }
        }

        public override string Part1()
        {
            var invalidValues = _nearbyTickets
                .SelectMany(ValuesNotInRange)
                .ToList();

            return invalidValues.Sum().ToString();
        }

        public override string Part2()
        {
            var validTickets = _nearbyTickets
                .Where(t => !ValuesNotInRange(t).Any())
                .ToList();

            var ticketFieldCount = validTickets.First().Values.Length;

            foreach(var ticket in validTickets)
            {
                for (var index = 0; index < ticketFieldCount; index++)
                {
                    foreach (var fieldRange in _fieldRanges)
                    {
                        fieldRange.InvalidateTicketIndexIfOutOfRange(index, ticket.Values[index]);
                    }
                }
            }

            foreach (var fieldRange in _fieldRanges)
            {
                fieldRange.SetPossibleIndexes(ticketFieldCount);
            }

            var allocatedIndexes = new Dictionary<string, int>();
            foreach (var fieldRange in _fieldRanges.OrderBy(fr => fr.PossibleIndexes.Count))
            {
                var possibleUnallocatedIndexes = fieldRange
                    .PossibleIndexes
                    .Where(pi => !allocatedIndexes.Values.Contains(pi))
                    .ToList();

                if (possibleUnallocatedIndexes.Count == 1)
                {
                    allocatedIndexes.Add(fieldRange.FieldName, possibleUnallocatedIndexes.First());
                }
            }

            // Console.WriteLine("*****************");
            // Console.WriteLine("Allocated Indexes");
            // Console.WriteLine("*****************");
            long result = 1;
            foreach (var kvp in allocatedIndexes)
            {
                if (kvp.Key.StartsWith("departure"))
                {
                    result *= _yourTicket.Values[kvp.Value];
                }
                // Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }

            return result.ToString();
        }

        private IEnumerable<int> ValuesNotInRange(TicketData ticket)
        {
            var invalidValues = ticket
                .Values
                .Where(v => !_fieldRanges.Any(fr => fr.InRange(v)))
                .ToList();

            return invalidValues;
        }

        private class FieldRanges
        {
            public string FieldName { get; set; }
            public List<IntRange> ValidRanges { get; set; }
            public FieldRanges(string line)
            {
                var split = line.Split(": ");
                FieldName = split[0];

                ValidRanges = split[1]
                    .Split(" or ")
                    .Select(r => new IntRange(r))
                    .ToList();
            }

            public List<int> PossibleIndexes { get; set; }
            public HashSet<int> InvalidatedTicketIndexes = new HashSet<int>();
            public void InvalidateTicketIndexIfOutOfRange(int index, int value)
            {
                if (!InRange(value))
                {
                    InvalidatedTicketIndexes.Add(index);
                }
            }

            public void SetPossibleIndexes(int maxIndex)
            {
                PossibleIndexes = Enumerable
                    .Range(0, maxIndex)
                    .Where(index => !InvalidatedTicketIndexes.Contains(index))
                    .ToList();

                // var possibleIndexString = string.Join(",", PossibleIndexes);
                // Console.WriteLine($"Possible - {FieldName}: {possibleIndexString}");
            }

            public bool InRange(int value) => ValidRanges.Any(r => r.InRange(value));
        }

        private class TicketData
        {
            public int[] Values { get; set; }
            public TicketData(string line)
            {
                Values = line
                    .Split(",")
                    .Select(int.Parse)
                    .ToArray();
            }
        }

        private class IntRange
        {
            public IntRange(string range)
            {
                var split = range.Split("-");
                Min = int.Parse(split[0]);
                Max = int.Parse(split[1]);
            }

            public int Min { get; set; }
            public int Max { get; set; }

            public bool InRange(int value) => Min <= value && value <= Max;
        }
    }
}

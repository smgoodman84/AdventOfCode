using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2022.Day20
{
    public class Day20 : Day
    {
        private static readonly Regex BlueprintRegex = new Regex("Blueprint (?<blueprint>[0-9]*): (?<robotcosts>.*)");

        public Day20() : base(2022, 20, "Day20/input_2022_20.txt", "9945", "")
        {

        }

        private List<Item> _items;
        public override void Initialise()
        {
            _items = InputLines
                .Select((x, i) => new Item
                {
                    Value = int.Parse(x),
                    OriginalPosition = i,
                    CurrentPosition = i
                })
                .ToList();
        }

        public override string Part1()
        {
            Console.WriteLine("We have");
            PrintList();

            foreach (var item in _items.ToList())
            {
                if (item.Value != 0)
                {
                    var currentPosition = item.CurrentPosition;

                    

                    var newPosition = currentPosition + item.Value;

                    if (newPosition < 0)
                    {
                        var actualNewPosition = newPosition;
                        while (actualNewPosition < 0)
                        {
                            actualNewPosition += _items.Count - 1;
                        }

                        if (currentPosition > actualNewPosition)
                        {
                            item.CurrentPosition = actualNewPosition;
                            for (var i = currentPosition - 1; i >= actualNewPosition; i--)
                            {
                                _items[i].CurrentPosition += 1;
                            }
                            _items.RemoveAt(currentPosition);
                            _items.Insert(actualNewPosition, item);
                        }
                        else if (currentPosition < actualNewPosition)
                        {
                            item.CurrentPosition = actualNewPosition;
                            for (var i = currentPosition + 1; i <= actualNewPosition; i++)
                            {
                                _items[i].CurrentPosition -= 1;
                            }
                            _items.RemoveAt(currentPosition);
                            _items.Insert(actualNewPosition, item);
                        }
                    }
                    else if (newPosition >= _items.Count)
                    {
                        var actualNewPosition = newPosition;
                        while (actualNewPosition >= _items.Count)
                        {
                            actualNewPosition -= (_items.Count - 1);
                        }

                        if (currentPosition > actualNewPosition)
                        {
                            item.CurrentPosition = actualNewPosition;
                            for (var i = currentPosition - 1; i >= actualNewPosition; i--)
                            {
                                _items[i].CurrentPosition += 1;
                            }
                            _items.RemoveAt(currentPosition);
                            _items.Insert(actualNewPosition, item);
                        }
                        else if (currentPosition < actualNewPosition)
                        {
                            item.CurrentPosition = actualNewPosition;
                            for (var i = currentPosition + 1; i <= actualNewPosition; i++)
                            {
                                _items[i].CurrentPosition -= 1;
                            }
                            _items.RemoveAt(currentPosition);
                            _items.Insert(actualNewPosition, item);
                        }
                    }
                    else if (currentPosition > newPosition)
                    {
                        item.CurrentPosition = newPosition;
                        for (var i = currentPosition - 1; i >= newPosition; i--)
                        {
                            _items[i].CurrentPosition += 1;
                        }
                        _items.RemoveAt(currentPosition);
                        _items.Insert(newPosition, item);
                    }
                    else if (currentPosition < newPosition)
                    {
                        item.CurrentPosition = newPosition;
                        for (var i = currentPosition + 1; i <= newPosition; i++)
                        {
                            _items[i].CurrentPosition -= 1;
                        }
                        _items.RemoveAt(currentPosition);
                        _items.Insert(newPosition, item);
                    }
                    

                    /*

                    var newPosition = (item.CurrentPosition + item.Value + _items.Count) % _items.Count;
                    if (item.Value < 0)
                    {
                        newPosition = (newPosition + _items.Count - 1) % _items.Count;
                    }

                    item.CurrentPosition = newPosition;
                    if (currentPosition < newPosition)
                    {
                        for (var i = currentPosition + 1; i <= newPosition; i++)
                        {
                            _items[i].CurrentPosition -= 1;
                        }
                        _items.RemoveAt(currentPosition);
                        _items.Insert(newPosition, item);
                    }

                    if (currentPosition > newPosition)
                    {
                        for (var i = currentPosition - 1; i >= newPosition; i--)
                        {
                            _items[i].CurrentPosition += 1;
                        }
                        _items.RemoveAt(currentPosition);
                        _items.Insert(newPosition, item);
                    }

                    /*
                    if (item.Value < 0)
                    {
                        if (newPosition == 0)
                        {
                            newPosition = _items.Count + 1;
                            item.CurrentPosition = newPosition;
                        }

                        _items.Insert(newPosition - 1, item);
                    }
                    else
                    {
                        _items.Insert(newPosition, item);
                    }*/


                    Console.WriteLine($"Moved {item}");
                    // Console.WriteLine("We have");
                    // PrintList();

                    var incorrectlyPlaced = _items
                        .Select((x, i) => (x, i, x.CurrentPosition != i))
                        .ToList();

                    if (incorrectlyPlaced.Any(x => x.Item3))
                    {
                        throw new Exception("We messed up");
                    }
                }
            }

            var zeroIndex = _items
                .Select((x, i) => (x.Value, i))
                .Where(x => x.Value == 0)
                .Select(x => x.i)
                .Single();

            var oneThousand = _items[(zeroIndex + 1000) % _items.Count];
            var twoThousand = _items[(zeroIndex + 2000) % _items.Count];
            var threeThousand = _items[(zeroIndex + 3000) % _items.Count];

            var result = oneThousand.Value + twoThousand.Value + threeThousand.Value;

            return result.ToString();
        }

        public override string Part2()
        {
            return "";
        }

        private class Item
        {
            public int Value { get; set; }
            public int OriginalPosition { get; set; }
            public int CurrentPosition { get; set; }

            public override string ToString()
            {
                return $"{Value},{OriginalPosition},{CurrentPosition}";
            }
        }

        private void PrintList()
        {
            foreach (var item in _items)
            {
                Console.WriteLine(item.ToString());
            }
        }
    }
}

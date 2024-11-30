using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2022.Day20;

public class Day20 : Day
{
    public Day20() : base(2022, 20, "Day20/input_2022_20.txt", "9945", "3338877775442")
    {

    }

    public override string Part1()
    {
        return Decrypt(1, 1);
    }

    public override string Part2()
    {
        return Decrypt(811589153L, 10);
    }

    private string Decrypt(long decryptionKey, int iterations)
    {
        var _items = InputLines
            .Select((x, i) => new Item
            {
                Value = long.Parse(x) * decryptionKey,
                OriginalPosition = i,
                CurrentPosition = i
            })
            .ToList();

        // Console.WriteLine("We have");
        // PrintList();

        foreach (var iteration in Enumerable.Range(1, iterations))
        {
            foreach (var item in _items.OrderBy(i => i.OriginalPosition).ToList())
            {
                if (item.Value != 0)
                {
                    var currentPosition = item.CurrentPosition;
                    var newPosition = currentPosition + item.Value;

                    if (newPosition < 0)
                    {
                        long count = (_items.Count - 1);
                        var divideBy = -newPosition / count;
                        var add = (divideBy + 1L) * count;
                        var actualNewPosition = newPosition + add;

                        if (currentPosition > actualNewPosition)
                        {
                            item.CurrentPosition = actualNewPosition;
                            for (var i = currentPosition - 1; i >= actualNewPosition; i--)
                            {
                                _items[(int)i].CurrentPosition += 1;
                            }
                            _items.RemoveAt((int)currentPosition);
                            _items.Insert((int)actualNewPosition, item);
                        }
                        else if (currentPosition < actualNewPosition)
                        {
                            item.CurrentPosition = actualNewPosition;
                            for (var i = currentPosition + 1; i <= actualNewPosition; i++)
                            {
                                _items[(int)i].CurrentPosition -= 1;
                            }
                            _items.RemoveAt((int)currentPosition);
                            _items.Insert((int)actualNewPosition, item);
                        }
                    }
                    else if (newPosition >= _items.Count)
                    {
                        long count = _items.Count - 1;
                        var divideBy = newPosition / count;
                        var subtract = divideBy * count;
                        var actualNewPosition = newPosition - subtract;

                        if (currentPosition > actualNewPosition)
                        {
                            item.CurrentPosition = actualNewPosition;
                            for (var i = currentPosition - 1; i >= actualNewPosition; i--)
                            {
                                _items[(int)i].CurrentPosition += 1;
                            }
                            _items.RemoveAt((int)currentPosition);
                            _items.Insert((int)actualNewPosition, item);
                        }
                        else if (currentPosition < actualNewPosition)
                        {
                            item.CurrentPosition = actualNewPosition;
                            for (var i = currentPosition + 1; i <= actualNewPosition; i++)
                            {
                                _items[(int)i].CurrentPosition -= 1;
                            }
                            _items.RemoveAt((int)currentPosition);
                            _items.Insert((int)actualNewPosition, item);
                        }
                    }
                    else if (currentPosition > newPosition)
                    {
                        item.CurrentPosition = newPosition;
                        for (var i = currentPosition - 1; i >= newPosition; i--)
                        {
                            _items[(int)i].CurrentPosition += 1;
                        }
                        _items.RemoveAt((int)currentPosition);
                        _items.Insert((int)newPosition, item);
                    }
                    else if (currentPosition < newPosition)
                    {
                        item.CurrentPosition = newPosition;
                        for (var i = currentPosition + 1; i <= newPosition; i++)
                        {
                            _items[(int)i].CurrentPosition -= 1;
                        }
                        _items.RemoveAt((int)currentPosition);
                        _items.Insert((int)newPosition, item);
                    }


                    // Console.WriteLine($"Moved {item}");
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

    private class Item
    {
        public long Value { get; set; }
        public long OriginalPosition { get; set; }
        public long CurrentPosition { get; set; }

        public override string ToString()
        {
            return $"{Value},{OriginalPosition},{CurrentPosition}";
        }
    }

    private void PrintList(List<Item> items)
    {
        foreach (var item in items)
        {
            Console.WriteLine(item.ToString());
        }
    }
}
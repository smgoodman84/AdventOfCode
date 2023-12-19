using System;
using System.Collections.Generic;

namespace AdventOfCode.Shared.Numbers
{
    public class NumberRange
    {
        public NumberRange(long start, long end)
        {
            Start = start;
            End = end;
            Length = End - Start + 1;
        }

        public static NumberRange FromLength(long start, long length)
        {
            return new NumberRange(start, start + length - 1);
        }

        public long Start { get; }
        public long End { get; }
        public long Length { get; }

        public bool Contains(long value)
        {
            return Start <= value && value <= End;
        }

        public bool Contains(NumberRange range)
        {
            return Contains(range.Start) && Contains(range.End);
        }

        public bool CanAdd(NumberRange range)
        {
            return Contains(range)
                || range.Contains(this)
                || (range.Start < Start && Start - 1 <= range.End)
                || (Start < range.Start && range.Start - 1 <= End);
        }

        public NumberRange Add(NumberRange range)
        {
            if (Contains(range))
            {
                return this;
            }

            if (range.Contains(this))
            {
                return range;
            }

            if (range.Start < Start && Start - 1 <= range.End)
            {
                return new NumberRange(range.Start, End);
            }

            if (Start < range.Start && range.Start - 1 <= End)
            {
                return new NumberRange(Start, range.End);
            }

            throw new Exception("Cannot Add Range");
        }

        public bool CanSubtract(NumberRange range)
        {
            return (range.End < Start)
                || (End < range.Start)
                || (Start < range.Start && End <= range.End)
                || (range.Start <= Start && range.End < End);
        }

        public NumberRange Subtract(NumberRange range)
        {
            if (range.End < Start)
            {
                return this;
            }

            if (End < range.Start)
            {
                return this;
            }

            if (Start < range.Start && End <= range.End)
            {
                return new NumberRange(Start, range.Start - 1);
            }

            if (range.Start <= Start && range.End < End)
            {
                return new NumberRange(range.End + 1, End);
            }

            throw new Exception("Cannot Subtract Range");
        }


        public IEnumerable<NumberRange> Except(NumberRange range)
        {
            if (range.End < Start)
            {
                return new[] { this };
            }

            if (End < range.Start)
            {
                return new[] { this };
            }

            if (Start < range.Start && End <= range.End)
            {
                return new[] { new NumberRange(Start, range.Start - 1) };
            }

            if (range.Start <= Start && range.End < End)
            {
                return new[] { new NumberRange(range.End + 1, End) };
            }

            if (range.Start <= Start && range.End >= End)
            {
                return Array.Empty<NumberRange>();
            }

            // range.Start > Start && range.End < End
            return new[]
            {
                new NumberRange(Start, range.Start - 1),
                new NumberRange(range.End + 1, End)
            };
        }

        public IEnumerable<NumberRange> Intersect(NumberRange range)
        {
            if (range.End < Start)
            {
                return Array.Empty<NumberRange>();
            }

            if (End < range.Start)
            {
                return Array.Empty<NumberRange>();
            }

            if (Start < range.Start && End <= range.End)
            {
                return new[] { new NumberRange(range.Start, End) };
            }

            if (range.Start <= Start && range.End < End)
            {
                return new[] { new NumberRange(Start, range.End) };
            }

            if (range.Start <= Start && range.End >= End)
            {
                return new[] { this };
            }

            // range.Start > Start && range.End < End
            return new[] { range };
        }

        public override string ToString()
        {
            return $"{Start}..{End} ({Length})";
        }
    }
}


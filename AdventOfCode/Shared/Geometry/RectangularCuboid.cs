using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Shared.Geometry
{
    public class RectangularCuboid
    {
        public long Width { get; set; }
        public long Height { get; set; }
        public long Length { get; set; }

        public RectangularCuboid(long width, long height, long length)
        {
            Width = width;
            Length = length;
            Height = height;
        }

        public IEnumerable<long> Dimensions => new long[]{ Width, Length, Height };

        public long SurfaceArea =>
                2 * Length * Width +
                2 * Width * Height +
                2 * Height * Length;

        public long Volume => Width * Height * Length;

        public long SmallDimension => Dimensions.OrderBy(x => x).First();
        public long MiddleDimension => Dimensions.OrderBy(x => x).Skip(1).First();
        public long LargeDimension => Dimensions.OrderByDescending(x => x).First();

        public long SmallArea => SmallDimension * MiddleDimension;
    }
}

using AdventOfCode.Shared;
using AdventOfCode.Shared.Geometry;

namespace AdventOfCode._2023.Day22
{
    public class Day22 : Day
    {
        public Day22() : base(2023, 22, "Day22/input_2023_22.txt", "401", "63491", false)
        {

        }

        Grid3D<Space> _map;
        List<SandBlock> _blocks;
        public override void Initialise()
        {
            _blocks = InputLines.Select((l, i) => new SandBlock(l, i + 1)).ToList();

            // var minX = _blocks.Min(b => Math.Min(b.Start.X, b.End.X));
            var maxX = _blocks.Max(b => Math.Max(b.Start.X, b.End.X));

            // var minY = _blocks.Min(b => Math.Min(b.Start.Y, b.End.Y));
            var maxY = _blocks.Max(b => Math.Max(b.Start.Y, b.End.Y));

            // var minZ = _blocks.Min(b => Math.Min(b.Start.Z, b.End.Z));
            var maxZ = _blocks.Max(b => Math.Max(b.Start.Z, b.End.Z));


            _map = new Grid3D<Space>((int)maxX + 1, (int)maxY + 1, (int)maxZ + 1);
            foreach(var coordinate in _map.GetAllCoordinates())
            {
                _map.Write(coordinate, Space.Empty);
            }

            foreach(var block in _blocks)
            {
                foreach (var coordinate in block.GetAllCoordinates())
                {
                    _map.Write(coordinate, block);
                }
            }
        }

        public override string Part1()
        {
            Settle();

            var count = 0;
            foreach (var block in _blocks)
            {
                var canDisintegrate = CanDisintegrateSafely(block);
                // TraceLine($"Can Disintegrate = {canDisintegrate}: {block}");
                if (canDisintegrate)
                {
                    count += 1;
                }
            }

            return count.ToString();
        }

        public override string Part2()
        {
            long totalFallers = 0;
            foreach (var block in _blocks)
            {
                TraceLine($"Getting Fallers - {block}");
                var fallCount = GetCountThatWillFall(block, new HashSet<int>());
                TraceLine($"Fallers - {fallCount} - {block}");
                totalFallers += fallCount;
            }
            return totalFallers.ToString();
        }
        /*
        private Dictionary<int, int> _fallCountCache = new Dictionary<int, int>();
        private int GetCountThatWillFall(SandBlock sandBlock)
        {
            if (!_fallCountCache.ContainsKey(sandBlock.BlockNumber))
            {
                _fallCountCache[sandBlock.BlockNumber] = GetCountThatWillFallUncached(sandBlock);
            }

            return _fallCountCache[sandBlock.BlockNumber];
        }*/

        private int GetCountThatWillFall(SandBlock sandBlock, HashSet<int> theFallen)
        {
            var fallers = new List<SandBlock>()
            {
                sandBlock
            };
            theFallen.Add(sandBlock.BlockNumber);

            var somethingFell = true;
            while (somethingFell)
            {
                somethingFell = false;
                var newFallers = fallers.ToList();
                foreach (var faller in fallers)
                {
                    var nextFallers = GetImmediateFallers(faller, theFallen);
                    if (nextFallers.Any())
                    {
                        somethingFell = true;
                        newFallers.AddRange(nextFallers);
                        foreach (var nextFaller in nextFallers)
                        {
                            TraceLine($"{nextFaller.BlockNumber} Fell");
                            theFallen.Add(nextFaller.BlockNumber);
                        }
                    }
                }
                fallers = newFallers;
            }

            var fallCount = fallers.Count();
            return fallCount - 1;
        }
        /*
        private int GetCountThatWillFallOld(SandBlock sandBlock, List<int> theFallen)
        {
            var theFallenString = string.Join(", ", theFallen);
            TraceLine($"Getting Fallers - {sandBlock} - {theFallenString}");

            var immediateFallers = GetImmediateFallers(sandBlock, theFallen);
            var immediateFallerBlockNumbers = immediateFallers.Select(b => b.BlockNumber).ToList();


            var immediateFallenString = string.Join(", ", immediateFallerBlockNumbers);
            TraceLine($"Immediate Fallers - {sandBlock} - {immediateFallenString}");

            var totalFallers = 0;
            foreach (var faller in immediateFallers)
            {
                totalFallers += 1;

                var newFallen = theFallen.Concat(immediateFallerBlockNumbers).Distinct().ToList();
                totalFallers += GetCountThatWillFall(faller, newFallen);
            }
            return totalFallers;
        }
        */
        private List<SandBlock> GetImmediateFallers(SandBlock sandBlock, HashSet<int> theFallen)
        {
            var willFall = new List<SandBlock>();
            var supporting = GetSupporting(sandBlock);
            var supportingButNotFallen = supporting.Where(x => !theFallen.Contains(x.BlockNumber)).ToList();
            foreach (var block in supportingButNotFallen)
            {
                var supportedBySomethingElse = false;
                foreach (var supportedBy in GetSupportedBy(block))
                {
                    if (supportedBy != sandBlock && !theFallen.Contains(supportedBy.BlockNumber))
                    {
                        supportedBySomethingElse = true;
                    }
                }

                if (!supportedBySomethingElse)
                {
                    willFall.Add(block);
                }
            }
            return willFall;
        }

        private bool CanDisintegrateSafely(SandBlock sandBlock)
        {
            var supporting = GetSupporting(sandBlock);
            foreach(var block in supporting)
            {
                var supportedBySomethingElse = false;
                foreach(var supportedBy in GetSupportedBy(block))
                {
                    if (supportedBy != sandBlock)
                    {
                        supportedBySomethingElse = true;
                    }
                }

                if (!supportedBySomethingElse)
                {
                    return false;
                }
            }
            return true;
        }

        private List<SandBlock> GetSupporting(SandBlock sandBlock)
        {
            var result = new List<SandBlock>();
            foreach(var coordinate in sandBlock.GetCoordinatesAbove())
            {
                if (_map.IsInGrid(coordinate))
                {
                    var contents = _map.Read(coordinate);
                    if (contents != Space.Empty)
                    {
                        var supportingBlock = contents as SandBlock;
                        if (!result.Contains(supportingBlock))
                        {
                            result.Add(supportingBlock);
                        }
                    }
                }
            }
            return result;
        }

        private List<SandBlock> GetSupportedBy(SandBlock sandBlock)
        {
            var result = new List<SandBlock>();
            foreach (var coordinate in sandBlock.GetCoordinatesBelow())
            {
                if (_map.IsInGrid(coordinate))
                {
                    var contents = _map.Read(coordinate);
                    if (contents != Space.Empty)
                    {
                        var supportedByBlock = contents as SandBlock;
                        if (!result.Contains(supportedByBlock))
                        {
                            result.Add(supportedByBlock);
                        }
                    }
                }
            }
            return result;
        }

        private void Settle()
        {
            var anythingMoved = true;

            while (anythingMoved)
            {
                anythingMoved = false;
                foreach (var block in _blocks)
                {
                    var allBelowEmpty = true;
                    foreach (var coordinate in block.GetCoordinatesBelow())
                    {
                        if (coordinate.Z < 0)
                        {
                            allBelowEmpty = false; // floor is below
                        }
                        else if (_map.Read(coordinate) != Space.Empty)
                        {
                            allBelowEmpty = false;
                        }
                    }

                    if (allBelowEmpty)
                    {
                        // TraceLine($"Settling {block}");
                        anythingMoved = true;
                        foreach(var coordinate in block.GetAllCoordinates())
                        {
                            _map.Write(coordinate, Space.Empty);
                        }

                        // Backward is down in this orientiantion
                        block.Start = block.Start.Backward();
                        block.End = block.End.Backward();

                        foreach (var coordinate in block.GetAllCoordinates())
                        {
                            _map.Write(coordinate, block);
                        }
                        // TraceLine($"Settled {block}");
                    }
                }
            }
        }

        private class Space
        {
            public static Space Empty = new Space();
        }

        private class SandBlock : Space
        {
            public Coordinate3D Start { get; set; }
            public Coordinate3D End { get; set; }
            public int BlockNumber { get; set; }

            public SandBlock(string description, int blockNumber)
            {
                var coordSplit = description.Split("~");

                Start = new Coordinate3D(coordSplit[0]);
                End = new Coordinate3D(coordSplit[1]);

                BlockNumber = blockNumber;
            }

            private long MinX => Math.Min(Start.X, End.X);
            private long MaxX => Math.Max(Start.X, End.X);

            private long MinY => Math.Min(Start.Y, End.Y);
            private long MaxY => Math.Max(Start.Y, End.Y);

            private long MinZ => Math.Min(Start.Z, End.Z);
            private long MaxZ => Math.Max(Start.Z, End.Z);

            public IEnumerable<Coordinate3D> GetAllCoordinates()
            {
                for(var x = MinX; x <= MaxX; x += 1)
                {
                    for (var y = MinY; y <= MaxY; y += 1)
                    {
                        for (var z = MinZ; z <= MaxZ; z += 1)
                        {
                            yield return new Coordinate3D(x, y, z);
                        }
                    }
                }
            }

            public IEnumerable<Coordinate3D> GetCoordinatesBelow()
            {
                for (var x = MinX; x <= MaxX; x += 1)
                {
                    for (var y = MinY; y <= MaxY; y += 1)
                    {
                        yield return new Coordinate3D(x, y, MinZ - 1);
                    }
                }
            }

            public IEnumerable<Coordinate3D> GetCoordinatesAbove()
            {
                for (var x = MinX; x <= MaxX; x += 1)
                {
                    for (var y = MinY; y <= MaxY; y += 1)
                    {
                        yield return new Coordinate3D(x, y, MaxZ + 1);
                    }
                }
            }

            public override string ToString()
            {
                return $"{BlockNumber}: {Start} -> {End}";
            }
        }
    }
}

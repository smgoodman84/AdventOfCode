using AdventOfCode.Shared;

namespace AdventOfCode._2024.Day09;

public class Day09 : Day
{
    public Day09() : base(2024, 9, "Day09/input_2024_09.txt", "6382875730645", "6420913943576")
    {
    }


    private List<FileInfo> _fileInfo;
    private int[] _memory;

    public override void Initialise()
    {
        var lengths = InputLines[0]
            .Select(c => c - '0')
            .ToList();
        
        var memoryLength = lengths.Sum();
        _memory = new int[memoryLength];

        var fileId = 0;
        var currentBlock = 0;
        var nextLengthIsFile = true;
        _fileInfo = new List<FileInfo>();
        foreach(var length in lengths)
        {
            var memoryToSet = 0;
            while (memoryToSet < length)
            {
                var memoryLocation = currentBlock + memoryToSet;
                if (nextLengthIsFile)
                {
                    _memory[memoryLocation] = fileId;
                }
                else
                {
                    _memory[memoryLocation] = -1;
                }

                memoryToSet += 1;
            }
            
            if (nextLengthIsFile)
            {
                _fileInfo.Add(new FileInfo
                {
                    FileId = fileId,
                    StartBlock = currentBlock,
                    Length = length,
                });

                fileId += 1;
            }
            else
            {
                _fileInfo.Add(new FileInfo
                {
                    FileId = -1,
                    StartBlock = currentBlock,
                    Length = length,
                });
            }
            
            currentBlock += length;
            nextLengthIsFile = !nextLengthIsFile;
        }
    }

    public override string Part1()
    {
        var memory = _memory.ToArray();
        var endPointer = _memory.Length - 1;
        var startPointer = 0;

        while (startPointer < endPointer)
        {
            while (memory[startPointer] != -1)
            {
                startPointer += 1;
            }

            while (memory[endPointer] == -1)
            {
                endPointer -= 1;
            }

            if (startPointer < endPointer)
            {
                memory[startPointer] = memory[endPointer];
                memory[endPointer] = -1;
            }
        }

        var result = CheckSum(memory);

        return result.ToString();
    }

    private static long CheckSum(int[] memory)
    {
        var checkSum = 0L;
        for (var index = 0; index < memory.Length; index += 1)
        {
            if (memory[index] != -1)
            {
                checkSum += memory[index] * index;
            }
        }

        return checkSum;
    }

    public override string Part2()
    {
        var gapsOfLengthDict = _fileInfo
                .Where(fi => fi.FileId == -1)
                .Where(fi => fi.Length != 0)
                .GroupBy(fi => fi.Length)
                .ToDictionary(g => g.Key, g => g.OrderBy(gap => gap.StartBlock).ToList());

        var reversedFiles = _fileInfo
            .Where(fi => fi.FileId != -1)
            .OrderByDescending(fi => fi.StartBlock)
            .ToList();

        foreach (var fileToMove in reversedFiles)
        {
            var firstGapsOfLength = gapsOfLengthDict.Values
                .Select(gap => gap.FirstOrDefault())
                .ToList();
            
            var gapToUse = firstGapsOfLength
                .Where(gap => gap != null)
                .Where(gap => gap.Length >= fileToMove.Length)
                .Where(gap => gap.StartBlock < fileToMove.StartBlock)
                .OrderBy(gap => gap.StartBlock)
                .FirstOrDefault();

            if (gapToUse != null)
            {
                var newGap = new FileInfo
                {
                    FileId = -1,
                    StartBlock = fileToMove.StartBlock,
                    Length = fileToMove.Length,
                };

                // Move file
                fileToMove.StartBlock = gapToUse.StartBlock;

                // Remove/shrink gap in file's new location
                gapsOfLengthDict[gapToUse.Length].Remove(gapToUse);
                if (fileToMove.Length < gapToUse.Length)
                {
                    gapToUse.StartBlock += fileToMove.Length;
                    gapToUse.Length -= fileToMove.Length;

                    gapsOfLengthDict[gapToUse.Length].Add(gapToUse);
                    gapsOfLengthDict[gapToUse.Length] = gapsOfLengthDict[gapToUse.Length]
                        .OrderBy(gap => gap.StartBlock)
                        .ToList();
                }

                // Combine new gap with trailing gap
                var gapAfterNewGap = gapsOfLengthDict.Values
                    .SelectMany(gl => gl)
                    .FirstOrDefault(gap => gap.StartBlock == newGap.StartBlock + newGap.Length);
                
                if (gapAfterNewGap != null)
                {
                    gapsOfLengthDict[gapAfterNewGap.Length].Remove(gapAfterNewGap);
                    newGap.Length += gapAfterNewGap.Length;
                }

                // Combine new gap with leading gap
                var gapBeforeNewGap = gapsOfLengthDict.Values
                    .SelectMany(gl => gl)
                    .FirstOrDefault(gap => newGap.StartBlock == gap.StartBlock + gap.Length);
                
                if (gapBeforeNewGap != null)
                {
                    gapsOfLengthDict[gapBeforeNewGap.Length].Remove(gapBeforeNewGap);
                    newGap.StartBlock = gapBeforeNewGap.StartBlock;
                    newGap.Length += gapBeforeNewGap.Length;
                }

                // Add new gap
                if (!gapsOfLengthDict.ContainsKey(newGap.Length))
                {
                    gapsOfLengthDict.Add(newGap.Length, new List<FileInfo>());
                }
                gapsOfLengthDict[newGap.Length].Add(newGap);
                gapsOfLengthDict[newGap.Length] = gapsOfLengthDict[newGap.Length]
                    .OrderBy(g => g.StartBlock)
                    .ToList();
            }
        }

        var checksum = _fileInfo
            .Where(f => f.FileId != -1)
            .Sum(f => f.CheckSum());

        return checksum.ToString();
    }
    
    private class FileInfo
    {
        public int FileId { get; set; }
        public int StartBlock { get; set; }
        public int Length { get; set; }

        public long CheckSum()
        {
            var result = 0L;
            for (var index = StartBlock; index < StartBlock + Length; index += 1)
            {
                result += FileId * index;
            }

            return result;
        }
    }
}
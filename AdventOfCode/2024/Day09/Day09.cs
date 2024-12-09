using AdventOfCode.Shared;

namespace AdventOfCode._2024.Day09;

public class Day09 : Day
{
    public Day09() : base(2024, 9, "Day09/input_2024_09.txt", "6382875730645", "")
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
        return string.Empty;
    }

    private class FileInfo
    {
        public int FileId { get; set; }
        public int StartBlock { get; set; }
        public int Length { get; set; }
    }
}
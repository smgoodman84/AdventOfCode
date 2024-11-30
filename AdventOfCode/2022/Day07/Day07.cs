using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2022.Day07;

public class Day07 : Day
{
    public Day07() : base(2022, 7, "Day07/input_2022_07.txt", "1743217", "8319096")
    {

    }

    private Directory _rootDirectory;
    public override void Initialise()
    {
        var directories = new Dictionary<string, Directory>();
        Directory currentDirectory = null;

        foreach (var line in InputLines)
        {
            if (line.StartsWith("$ cd /"))
            {
                var newDirectory = line.Substring("$ cd ".Length);

                if (!directories.ContainsKey(newDirectory))
                {
                    directories[newDirectory] = new Directory(null, newDirectory);
                }
                currentDirectory = directories[newDirectory];

                if (newDirectory == "/")
                {
                    _rootDirectory = currentDirectory;
                }
            }
            else if (line.StartsWith("$ cd .."))
            {
                currentDirectory = currentDirectory.Parent;
            }
            else if (line.StartsWith("$ cd "))
            {
                var newDirectory = line.Substring("$ cd ".Length);
                var subDirectory = currentDirectory.GetChild(newDirectory) as Directory;
                if (subDirectory == null)
                {
                    subDirectory = new Directory(currentDirectory, newDirectory);
                    var fullPath = subDirectory.GetFullPath();
                    directories[fullPath] = subDirectory;
                    currentDirectory = directories[newDirectory];
                }
                else
                {
                    currentDirectory = subDirectory;
                }
            }
            else if (line.StartsWith("$ ls"))
            {

            }
            else if (line.StartsWith("dir"))
            {
                var newDirectory = line.Substring("dir ".Length);
                var subDirectory = currentDirectory.GetChild(newDirectory) as Directory;
                if (subDirectory == null)
                {
                    subDirectory = new Directory(currentDirectory, newDirectory);
                    var fullPath = subDirectory.GetFullPath();
                    directories[fullPath] = subDirectory;
                    currentDirectory.AddChild(subDirectory);
                }
            }
            else
            {
                var fileInfo = line.Split(" ");
                var fileSize = long.Parse(fileInfo[0]);
                var fileName = fileInfo[1];

                var file = new File(currentDirectory, fileName, fileSize);
                currentDirectory.AddChild(file);
            }
        }
    }

    public override string Part1()
    {
        return _rootDirectory
            .GetDirectoriesAtMost(100000)
            .Select(d => d.GetSize())
            .Sum()
            .ToString();
    }

    public override string Part2()
    {
        long requiredSpace = 30000000l;
        long totalDiskSpace = 70000000l;

        long currentUsedSpace = _rootDirectory.GetSize();
        long currentFreeSpace = totalDiskSpace - currentUsedSpace;

        long needToDeleteSpace = requiredSpace - currentFreeSpace;

        var allDirectories = _rootDirectory.GetAllDirectories();

        var allDirectoriesWithSize = allDirectories
            .Select(d => (Directory: d, Size: d.GetSize()))
            .OrderBy(di => di.Size)
            .First(di => di.Size > needToDeleteSpace);

        return allDirectoriesWithSize.Size.ToString();
    }

    private interface IFilesystemElement
    {
        public string Name { get; set; }

        public Directory Parent { get; set; }

        public long GetSize();

        public string GetFullPath();
    }

    private class Directory : IFilesystemElement
    {
        public Directory(Directory parent, string name)
        {
            Parent = parent;
            Name = name;
            Children = new List<IFilesystemElement>();
            _size = 0;
        }

        public string Name { get; set; }
        public Directory Parent { get; set; }
        public List<IFilesystemElement> Children { get; set; }

        public void AddChild(IFilesystemElement child)
        {
            Children.Add(child);
        }

        private long _size;
        public void IncreaseSize(long size)
        {
            _size += size;
            Parent?.IncreaseSize(size);
        }
        public long GetSize() => _size;

        public IFilesystemElement GetChild(string name)
            => Children.FirstOrDefault(c => c.Name == name);

        public string GetFullPath()
            => $"{Parent?.GetFullPath()}/{Name}";

        public IEnumerable<Directory> GetDirectoriesAtMost(long maxSize)
        {
            var subDirectories = Children
                .OfType<Directory>()
                .Where(d => d.GetSize() <= maxSize);

            var recurse = Children
                .OfType<Directory>()
                .SelectMany(sd => sd.GetDirectoriesAtMost(maxSize));

            return subDirectories
                .Concat(recurse);
        }

        public IEnumerable<Directory> GetAllDirectories()
        {
            var subDirectories = Children
                .OfType<Directory>();

            var recurse = Children
                .OfType<Directory>()
                .SelectMany(sd => sd.GetAllDirectories());

            return subDirectories
                .Concat(recurse);
        }
    }

    private class File : IFilesystemElement
    {
        public File(Directory parent, string name, long size)
        {
            Parent = parent;
            Name = name;
            Size = size;
            Parent.IncreaseSize(size);
        }

        public Directory Parent { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }

        public long GetSize() => Size;

        public string GetFullPath()
            => $"{Parent.GetFullPath()}/{Name}";
    }
}
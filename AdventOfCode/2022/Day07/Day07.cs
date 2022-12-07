using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using AdventOfCode.Shared;
using AdventOfCode.Shared.FileProcessing;
using AdventOfCode.Shared.PrimitiveExtensions;

namespace AdventOfCode._2022.Day07
{
    public class Day07 : Day
    {
        public Day07() : base(2022, 7, "Day07/input_2022_07.txt", "", "")
        {

        }

        private string _signal;
        public override void Initialise()
        {
            _signal = InputLines.FirstOrDefault();
        }

        public override string Part1()
        {
            var directories = new Dictionary<string, Directory>();
            Directory rootDirectory = null;
            Directory currentDirectory = null;
            var listing = false;

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
                        rootDirectory = currentDirectory;
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

            return rootDirectory
                .GetDirectoriesAtMost(100000)
                .Select(d => d.GetSize())
                .Sum()
                .ToString();
        }

        public override string Part2()
        {
            return "";
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
            }

            public string Name { get; set; }
            public Directory Parent { get; set; }
            public List<IFilesystemElement> Children { get; set; }

            public void AddChild(IFilesystemElement child)
            {
                Children.Add(child);
            }

            public long GetSize() => Children.Sum(c => c.GetSize());

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
        }

        private class File : IFilesystemElement
        {
            public File(Directory parent, string name, long size)
            {
                Parent = parent;
                Name = name;
                Size = size;
            }

            public Directory Parent { get; set; }
            public string Name { get; set; }
            public long Size { get; set; }

            public long GetSize() => Size;

            public string GetFullPath()
                => $"{Parent.GetFullPath()}/{Name}";
        }
    }
}


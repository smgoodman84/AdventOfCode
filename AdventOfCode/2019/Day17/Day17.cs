using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdventOfCode._2019.Intcode;
using AdventOfCode.Shared;

namespace AdventOfCode._2019.Day17
{
    public class Day17 : Day
    {
        public Day17() : base(2019, 17, "Day17/input_2019_17.txt", "6244", "1143523")
        {

        }

        private IntcodeMachine _ascii;
        private IOPipe _asciiOutput;
        private IOPipe _asciiInput;

        public override void Initialise()
        {
            _ascii = IntcodeMachine.Load(InputLines);
            _asciiInput = new IOPipe();
            _asciiOutput = new IOPipe();
            _ascii.SetOutput(_asciiOutput);
            _ascii.SetInput(_asciiInput);
        }

        public override string Part1()
        {
            return GetSumOfAlignmentParameters().ToString();
        }

        public override string Part2()
        {
            Initialise();
            return CollectSpaceDust().GetAwaiter().GetResult().ToString();
        }

        public int GetSumOfAlignmentParameters()
        {
            var task = GetSumOfAlignmentParametersAsync();
            task.Wait();
            return task.Result;
        }

        public async Task<int> GetSumOfAlignmentParametersAsync()
        {
            await _ascii.Execute();
            var mapInput = new List<char>();
            while (_asciiOutput.HasInputToRead())
            {
                var character = (char)await _asciiOutput.ReadInput();
                mapInput.Add(character);
                Trace(character);
            }

            var map = new Map(mapInput, TraceLine);

            var intersections = map.FindIntersections();

            var result = intersections.Sum(i => i.X * i.Y);

            return result;
        }

        public async Task<long> CollectSpaceDust()
        {
            await _ascii.Execute();
            var mapInput = new List<char>();
            while (_asciiOutput.HasInputToRead())
            {
                var character = (char)await _asciiOutput.ReadInput();
                mapInput.Add(character);
                Trace(character);
            }

            var map = new Map(mapInput, TraceLine);

            var directions = map.GetPathManual();

            Initialise();
            _ascii.Repair(0, 2);

            foreach(var direction in directions)
            {
                foreach(var c in direction)
                {
                    _asciiInput.Output(c);
                }
                _asciiInput.Output('\n');
            }
            _asciiInput.Output('n');
            _asciiInput.Output('\n');

            await _ascii.Execute();

            long result = 0;
            while (_asciiOutput.HasInputToRead())
            {
                result = await _asciiOutput.ReadInput();
            }

            return result;
        }

        private class Map
        {
            private int _height;
            private int _width;

            private char[,] _map;

            private Point _robotPosition;

            private char[] _directions = new[] { '^', '<', 'v', '>' };
            private readonly Action<string> _traceLine;

            public Map(List<char> input, Action<string> traceLine)
            {
                _traceLine = traceLine;
                _width = input.IndexOf('\n');
                _height = input.Count / _width;

                _map = new char[_width, _height];
                var x = 0;
                var y = 0;
                foreach (var c in input)
                {
                    if (_directions.Contains(c))
                    {
                        _robotPosition = new Point(x, y);
                    }

                    if (c == '\n')
                    {
                        x = 0;
                        y += 1;
                    }
                    else
                    {
                        _map[x, y] = c;
                        x += 1;
                    }
                }
            }

            private bool IsScaffold(int x, int y)
            {
                if (x < 0 || y < 0 || x >= _width || y >= _height)
                {
                    return false;
                }

                return _map[x, y] == '#';
            }

            public IEnumerable<Point> FindIntersections()
            {
                for (var y = 0; y < _height; y++)
                {
                    for (var x = 0; x < _width; x++)
                    {
                        if (IsScaffold(x, y)
                            && IsScaffold(x + 1, y)
                            && IsScaffold(x - 1, y)
                            && IsScaffold(x, y + 1)
                            && IsScaffold(x, y - 1))
                        {
                            yield return new Point(x, y);
                        }
                    }
                }
            }

            public List<string> GetPathManual()
            {
                return new List<string>
                {
                    "A,B,A,C,B,A,C,A,C,B",
                    "L,12,L,8,L,8",
                    "L,12,R,4,L,12,R,6",
                    "R,4,L,12,L,12,R,6"
                };
            }

            public List<string> GetPath()
            {
                var position = _robotPosition;
                var direction = _map[_robotPosition.X, _robotPosition.Y];

                List<char> directions = new List<char>();

                var stillExploring = true;
                while (stillExploring)
                {
                    var next = GetNextMapValue(position, direction);
                    switch (next)
                    {
                        case '#':
                            directions.Add('A');
                            switch (direction)
                            {
                                case '^':
                                    position = new Point(position.X, position.Y - 1);
                                    break;
                                case 'v':
                                    position = new Point(position.X, position.Y + 1);
                                    break;
                                case '<':
                                    position = new Point(position.X - 1, position.Y);
                                    break;
                                case '>':
                                    position = new Point(position.X + 1, position.Y);
                                    break;
                            }
                            break;
                        case '.':
                            var turnLeftDirection = _directions[(Array.IndexOf(_directions, direction) + 1) % 4];
                            var turnRightDirection = _directions[(Array.IndexOf(_directions, direction) + 3) % 4];
                            if (GetNextMapValue(position, turnLeftDirection) == '#')
                            {
                                directions.Add('L');
                                direction = turnLeftDirection;
                            }
                            else if (GetNextMapValue(position, turnRightDirection) == '#')
                            {
                                directions.Add('R');
                                direction = turnRightDirection;
                            }
                            else
                            {
                                stillExploring = false;
                            }
                            break;
                        default:
                            throw new Exception($"Unexpected map value {position} {next}");
                    }
                }

                _traceLine(string.Join(",", directions));

                var newDirections = ConsolidateAdvancement(directions);

                var compressedDirections = CompressDirections(newDirections);

                return compressedDirections;
            }

            private class MatchInfo
            {
                public int MatchStart;
                public int MatchLength;
                public int Offset;
                public string Value;
            }

            private List<string> CompressDirections(List<string> directions)
            {
                var matchInfo = new List<MatchInfo>();
                for (var offset = 1; offset < directions.Count - 1; offset++)
                {
                    var initialPosition = 0;
                    var shiftedPosition = offset;
                    var matching = false;
                    var matchStart = 0;
                    var matchLength = 0;
                    while (shiftedPosition < directions.Count)
                    {
                        if (directions[initialPosition] == directions[shiftedPosition])
                        {
                            if (matching)
                            {
                                matchLength += 1;
                            }
                            else
                            {
                                matching = true;
                                matchStart = initialPosition;
                                matchLength = 1;
                            }
                        }
                        else
                        {
                            if (matching)
                            {
                                matching = false;
                                if (matchLength % 2 == 0)
                                {
                                    matchInfo.Add(new MatchInfo
                                    {
                                        MatchStart = matchStart,
                                        MatchLength = matchLength,
                                        Offset = offset,
                                        Value = string.Join(",", directions.Skip(matchStart).Take(matchLength))
                                    });
                                }
                                initialPosition = matchStart;
                                shiftedPosition = matchStart + offset;
                            }
                        }

                        initialPosition += 1;
                        shiftedPosition += 1;
                    }
                }

                var exploded = matchInfo.SelectMany(Explode);

                var ordered = matchInfo.OrderByDescending(x => x.MatchLength);

                var targets = matchInfo
                    .Where(m => new[] { "L,12,L,8,L,8", "L,12,R,4,L,12,R,6", "R,4,L,12,L,12,R,6" }.Contains(m.Value))
                    .ToList();


                var grouped = targets
                    .GroupBy(x => x.Value)
                    //.Where(g => g.Key.Length <= 20) // Functions have to be 20 characters or less
                    //.Where(g => g.Count() <= 10) // Can only call 10 functions
                    //.Select(g => ((g.Key.Length - 2) * (g.Count() + 1), g))
                    //.OrderByDescending(t => t.Item1)
                    .ToList();

                var functionNames = new List<char> { 'A', 'B', 'C' };
                var functions = grouped.Select((g, i) => (functionNames[i], g.Key)).ToList();
                var directionsString = string.Join(",", directions);

                var functionList = new List<int>();
                for (var i = 0; i < directionsString.Length;)
                {
                    foreach(var function in functions)
                    {
                        if (directionsString.Substring(i, function.Item2.Length) == function.Item2)
                        {
                            functionList.Add(function.Item1);
                            i += function.Item2.Length + 1;
                        }
                    }
                }

                var result = new List<string>();
                result.Add(string.Join(",", functionList));
                result.AddRange(functions.Select(f => f.Item2));

                return result;
            }


            private IEnumerable<MatchInfo> Explode(MatchInfo matchInfo)
            {
                if (matchInfo.MatchLength == 2)
                {
                    return new List<MatchInfo> { matchInfo };
                }

                var result = new List<MatchInfo>();
                for (var division = 2; division < matchInfo.MatchLength; division += 2)
                {
                    result.AddRange(Explode(new MatchInfo
                    {
                        MatchLength = division,
                        MatchStart = matchInfo.MatchStart,
                        Offset = matchInfo.Offset,
                        Value = matchInfo.Value.Substring(0, NthIndex(matchInfo.Value, ',', division))
                    }));

                    result.AddRange(Explode(new MatchInfo
                    {
                        MatchLength = matchInfo.MatchLength - division,
                        MatchStart = matchInfo.MatchStart + division,
                        Offset = matchInfo.Offset,
                        Value = matchInfo.Value.Substring(NthIndex(matchInfo.Value, ',', division) + 1)
                    }));
                }

                return result;
            }
        
            private int NthIndex(string haystack, char needle, int count)
            {
                var index = 0;
                for (index = 0; count > 0; index++)
                {
                    if (haystack[index] == needle)
                    {
                        count -= 1;
                    }
                }

                return index;
            }

            private static List<string> ConsolidateAdvancement(List<char> directions)
            {
                var newDirections = new List<string>();
                var advanceCount = 0;
                var inAs = false;
                foreach (var d in directions)
                {
                    if (d == 'A')
                    {
                        if (inAs)
                        {
                            advanceCount += 1;
                        }
                        else
                        {
                            inAs = true;
                            advanceCount = 1;
                        }
                    }
                    else
                    {
                        if (inAs)
                        {
                            newDirections.Add(advanceCount.ToString());
                        }
                        newDirections.Add(d.ToString());
                        inAs = false;
                    }
                }
                if (inAs)
                {
                    newDirections.Add(advanceCount.ToString());
                }

                return newDirections;
            }

            private char GetNextMapValue(Point position, char direction)
            {
                var nextPosition = GetNextPosition(position, direction);

                if (nextPosition.X < 0 || nextPosition.X >= _width
                    || nextPosition.Y < 0 || nextPosition.Y >= _height)
                {
                    return '.';
                }

                return _map[nextPosition.X, nextPosition.Y];

                throw new Exception($"Unexpected Direction {direction}");
            }

            private Point GetNextPosition(Point position, char direction)
            {
                switch (direction)
                {
                    case '^': return new Point(position.X, position.Y - 1);
                    case 'v': return new Point(position.X, position.Y + 1);
                    case '<': return new Point(position.X - 1, position.Y);
                    case '>': return new Point(position.X + 1, position.Y);
                }

                throw new Exception($"Unexpected Direction {direction}");
            }
        }
    }
}

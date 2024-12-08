using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AdventOfCode._2019.Intcode;
using AdventOfCode.Shared;

namespace AdventOfCode._2019.Day13;

public class Day13 : Day
{
    public Day13() : base(2019, 13, "Day13/input_2019_13.txt", "200", "9803")
    {

    }

    private void Reset()
    {
        _gameOutput = new IOPipe();
        _gameInput = new IOPipe();
        _canvas = new Dictionary<int, Dictionary<int, char>>();
        _game = IntcodeMachine.Load(InputLines);
        _game.SetOutput(_gameOutput);
    }

    private IntcodeMachine _game;
    private IOPipe _gameOutput;
    private IOPipe _gameInput;
    private Dictionary<int, Dictionary<int, char>> _canvas;
    private bool _render = false;

    private readonly Dictionary<int, char> _characters = new Dictionary<int, char>
    {
        {0, ' '},
        {1, '#'},
        {2, '+'},
        {3, '*'},
        {4, 'o'},
    };

    public override string Part1()
    {
        Reset();
        return Execute().CountCharacters(2).ToString();
    }

    public override string Part2()
    {
        Reset();
        return ExecuteWithInput().GetAwaiter().GetResult().GetScore().ToString();
    }

    private Day13 Execute()
    {
        ExecuteAsync().Wait();
        return this;
    }

    private async Task ExecuteAsync()
    {
        await _game.Execute();

        if (_render)
        {
            Console.Clear();
        }

        while (_gameOutput.HasInputToRead())
        {
            var x = (int)await _gameOutput.ReadInput();
            var y = (int)await _gameOutput.ReadInput();
            var character = (int)await _gameOutput.ReadInput();

            Render(x, y, _characters[character]);
        }

        if (_render)
        {
            Console.WriteLine();
        }
    }

    private int _score = 0;
    private void SetScore(int score)
    {
        _score = score;
        var x = 0;
        foreach (var c in $"Score: {_score.ToString().PadLeft(10, '0')}")
        {
            Render(x, 0, c);
            x += 1;
        }
    }

    public int GetScore()
    {
        return _score;
    }

    private int _paddleX;
    private int _ballX;
    private Task _gameTask;
    public async Task<Day13> ExecuteWithInput()
    {
        _game.Repair(0, 2);
        _game.SetInput(_gameInput);
        _gameTask = _game.Execute();

        var inputTask = HandleInput();

        if (_render)
        {
            Console.Clear();
        }
        SetScore(0);
        while (!_gameTask.IsCompleted || _gameOutput.HasInputToRead())
        {
            if (_gameTask.IsCompleted)
            {
                // var stop = true;
            }
            var x = (int)await _gameOutput.ReadInput();
            var y = (int)await _gameOutput.ReadInput();
            var character = (int)await _gameOutput.ReadInput();

            switch (character)
            {
                case 3: _paddleX = x; break;
                case 4: _ballX = x; break;
            }

            if (x == -1)
            {
                SetScore(character);
            }
            else
            {
                Render(x, y + 2, _characters[character]);
            }
        }

        if (_render)
        {
            Console.WriteLine();
        }

        return this;
    }


    private IEnumerable<int> Left(int count) => RepeatingList(-1, count);
    private IEnumerable<int> Right(int count) => RepeatingList(1, count);
    private IEnumerable<int> Stop(int count) => RepeatingList(0, count);

    private IEnumerable<int> RepeatingList(int value, int count)
    {
        while (count > 0)
        {
            yield return value;
            count -= 1;
        }
    }

    private IEnumerable<int> ParseInputLine(string line)
    {
        var direction = line[0];
        var count = int.Parse(line.Substring(1));
        switch (direction)
        {
            case 'L': return Left(count);
            case 'R': return Right(count);
            case 'S': return Stop(count);
        }
        return new List<int>();
    }

    public async Task HandleInput()
    {
        await Task.Run(async () => 
        {
            var i = 0;
            var inputFile = "Day13/GameInput.txt";
            var preparedInput = File.ReadAllLines(inputFile)
                .Select(ParseInputLine)
                .SelectMany(x => x)
                .ToArray();

            while (!_gameTask.IsCompleted)
            {
                var output = 0;
                    
                if (i < preparedInput.Length)
                {
                    output = preparedInput[i];
                    i += 1;
                }
                else
                {
                    if (true)
                    {

                        if (_paddleX < _ballX)
                        {
                            File.AppendAllLines(inputFile, new[] { "R1" });
                            output = 1;
                        }
                        else if (_ballX < _paddleX)
                        {
                            output = -1;
                            File.AppendAllLines(inputFile, new[] { "L1" });
                        }
                        else
                        {
                            output = 0;
                            File.AppendAllLines(inputFile, new[] { "S1" });
                        }
                    }
                    else
                    {
                        /*
                        var input = Console.ReadKey();
                        switch (input.Key)
                        {
                            case ConsoleKey.LeftArrow:
                                output = -1;
                                File.AppendAllLines(inputFile, new[] { "L1" });
                                break;
                            case ConsoleKey.RightArrow:
                                File.AppendAllLines(inputFile, new[] { "R1" });
                                output = 1;
                                break;
                        }
                        */
                    }
                }
                    
                _gameInput.Output(output);
                await Task.Delay(1);
            }
        });
    }

    public int CountCharacters(int characterCode)
    {
        var character = _characters[characterCode];

        var result = _canvas.SelectMany(x => x.Value.Values)
            .Count(c => c == character);

        return result;
    }
        
    private void Render(int x, int y, char character)
    {
        if (!_canvas.ContainsKey(x))
        {
            _canvas.Add(x, new Dictionary<int, char>());
        }

        if (!_canvas[x].ContainsKey(y))
        {
            _canvas[x].Add(y, character);
        }
        else
        {
            _canvas[x][y] = character;
        }

        if (_render)
        {
            Console.SetCursorPosition(x, y + 1);
            Console.Write(character);
            Console.SetCursorPosition(0, 0);
        }
    }
}
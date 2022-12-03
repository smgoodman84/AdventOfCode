using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventOfCode._2019.Intcode;
using AdventOfCode.Shared;

namespace AdventOfCode._2019.Day11
{
    public class Day11 : Day
    {
        private const string part2Result = "\n  ## #### #    #### ####  ##  #  # ### \n   # #    #    #    #    #  # #  # #  #\n   # ###  #    ###  ###  #    #### #  #\n   # #    #    #    #    # ## #  # ### \n#  # #    #    #    #    #  # #  # #   \n ##  #### #### #### #     ### #  # #   \n";
        public Day11() : base(2019, 11, "Day11/input_2019_11.txt", "2172", part2Result)
        {

        }

        private Point _location;
        private int _direction;
        private HullCanvas _hull;
        private IntcodeMachine _controller;
        private IOPipe _robotToControllerPipe;
        private IOPipe _controllerToRobotPipe;
        public override void Initialise()
        {
            Reset();
        }

        private void Reset()
        {
            _location = new Point(0, 0);
            _direction = 0;

            _hull = new HullCanvas();

            _controller = IntcodeMachine.Load(InputLines);

            _robotToControllerPipe = new IOPipe();
            _controllerToRobotPipe = new IOPipe();

            _controller.SetInput(_robotToControllerPipe);
            _controller.SetOutput(_controllerToRobotPipe);
        }

        public override string Part1()
        {
            return "";
            //return Execute().GetAwaiter().GetResult().GetEverWhiteCount().ToString();
        }

        public override string Part2()
        {
            Reset();
            return StartOnWhite().Execute().GetAwaiter().GetResult().RenderHull();
        }

        private const int Black = 0;
        private const int White = 1;

        private readonly Point[] _directions = {
            new Point(0, 1), // Up
            new Point(1, 0), // Right
            new Point(0, -1), // Down
            new Point(-1, 0), // Left
        };

        public Day11 StartOnWhite()
        {
            _hull.SetColour(_location, White);
            return this;
        }

        public string RenderHull()
        {
            return _hull.Render();
        }

        public async Task<Day11> Execute()
        {
            var controllerTask = _controller.Execute();

            while (!controllerTask.IsCompleted)
            {
                _robotToControllerPipe.Output(_hull.GetColour(_location));
                var colourToPaint = (int)await _controllerToRobotPipe.ReadInput();
                _hull.SetColour(_location, colourToPaint);
                TraceLine($"Set {_location} colour to {colourToPaint}");
                if (controllerTask.IsCompleted)
                {
                    break;
                }
                var directionToTurn = (int)await _controllerToRobotPipe.ReadInput();
                switch (directionToTurn)
                {
                    case 0: _direction += 3;
                        TraceLine($"Turning left");
                        break;
                    case 1: _direction += 1;
                        TraceLine($"Turning right");
                        break;
                }

                _direction = _direction % 4;
                _location = new Point(
                    _location.X + _directions[_direction].X,
                    _location.Y + _directions[_direction].Y
                );
            }

            return this;
        }

        public int GetEverWhiteCount()
        {
            return _hull.GetEverWhiteCount();
        }

        private class HullCanvas
        {
            private readonly List<Point> _whitePanels = new List<Point>();
            private readonly List<Point> _wasEverWhitePanels = new List<Point>();
            public int GetColour(Point location)
            {
                if (IsWhite(location))
                {
                    return White;
                }

                return Black;
            }

            private bool IsWhite(Point location)
            {
                return _whitePanels.Any(p => p.Equals(location));
            }

            private void MakeBlack(Point location)
            {
                if (IsWhite(location))
                {
                    _whitePanels.RemoveAll(p => p.Equals(location));
                }
            }

            private void MakeWhite(Point location)
            {
                if (IsWhite(location))
                {
                    return;
                }

                _whitePanels.Add(location);
                if (!_wasEverWhitePanels.Any(p => p.Equals(location)))
                {
                    _wasEverWhitePanels.Add(location);
                }
            }

            public void SetColour(Point location, int colour)
            {
                switch (colour)
                {
                    case White:
                        MakeWhite(location);
                        break;
                    case Black:
                        MakeBlack(location);
                        break;
                }
            }

            public int GetEverWhiteCount()
            {
                return _wasEverWhitePanels.Count;
            }

            public string Render()
            {
                var stringBuilder = new StringBuilder();

                var minX = _whitePanels.Min(p => p.X);
                var minY = _whitePanels.Min(p => p.Y);
                var maxX = _whitePanels.Max(p => p.X);
                var maxY = _whitePanels.Max(p => p.Y);

                stringBuilder.Append("\n");
                foreach (var y in Enumerable.Range(minY, maxY - minY + 1).Reverse())
                {
                    foreach (var x in Enumerable.Range(minX, maxX - minX + 1))
                    {
                        if (IsWhite(new Point(x, y)))
                        {
                            stringBuilder.Append("#");
                        }
                        else
                        {
                            stringBuilder.Append(" ");
                        }
                    }
                    stringBuilder.Append("\n");
                }

                return stringBuilder.ToString();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using AdventOfCode._2019.Intcode;

namespace AdventOfCode2019.Day11
{
    public class Robot
    {
        private const int Black = 0;
        private const int White = 1;
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
                    case White: MakeWhite(location);
                        break;
                    case Black: MakeBlack(location);
                        break;
                }
            }

            public int GetEverWhiteCount()
            {
                return _wasEverWhitePanels.Count;
            }

            public void Render()
            {
                var minX = _whitePanels.Min(p => p.X);
                var minY = _whitePanels.Min(p => p.Y);
                var maxX = _whitePanels.Max(p => p.X);
                var maxY = _whitePanels.Max(p => p.Y);

                foreach (var y in Enumerable.Range(minY, maxY - minY + 1).Reverse())
                {
                    foreach (var x in Enumerable.Range(minX, maxX - minX + 1))
                    {
                        if (IsWhite(new Point(x, y)))
                        {
                            Console.Write("#");
                        }
                        else
                        {
                            Console.Write(" ");
                        }
                    }
                    Console.WriteLine();
                }
            }
        }
        
        private readonly HullCanvas _hull = new HullCanvas();
        private readonly IntcodeMachine _controller = IntcodeMachine.LoadFromFile("Day11/RobotController.txt");
        private readonly IOPipe _robotToControllerPipe = new IOPipe();
        private readonly IOPipe _controllerToRobotPipe = new IOPipe();

        private readonly Point[] _directions = {
            new Point(0, 1), // Up
            new Point(1, 0), // Right
            new Point(0, -1), // Down
            new Point(-1, 0), // Left
        };

        private Point _location = new Point(0, 0);
        private int _direction = 0;
        public Robot()
        {
            _controller.SetInput(_robotToControllerPipe);
            _controller.SetOutput(_controllerToRobotPipe);
            
        }

        public Robot StartOnWhite()
        {
            _hull.SetColour(_location, White);
            return this;
        }

        public Robot RenderHull()
        {
            _hull.Render();
            return this;
        }

        public async Task<Robot> Execute()
        {
                
            var controllerTask = _controller.Execute();

            while (!controllerTask.IsCompleted)
            {
                _robotToControllerPipe.Output(_hull.GetColour(_location));
                var colourToPaint = (int)await _controllerToRobotPipe.ReadInput();
                _hull.SetColour(_location, colourToPaint);
                if (controllerTask.IsCompleted)
                {
                    break;
                }
                var directionToTurn = (int)await _controllerToRobotPipe.ReadInput();
                switch (directionToTurn)
                {
                    case 0: _direction += 3;
                        break;
                    case 1: _direction += 1;
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
    }
}
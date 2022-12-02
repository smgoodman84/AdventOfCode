using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace AdventOfCode2019.Day10
{
    public class AsteroidMap
    {
        public static AsteroidMap LoadFromFile(string filename)
        {
            var asteroids = File.ReadAllLines(filename)
                .SelectMany(ReadLine)
                .ToList();

            return new AsteroidMap(asteroids);
        }

        private static IEnumerable<Asteroid> ReadLine(string line, int y)
        {
            var x = 0;
            foreach (var c in line)
            {
                if (c == '#')
                {
                    yield return new Asteroid(x, y);
                }

                x += 1;
            }
        }

        private readonly List<Asteroid> _asteroids;

        private AsteroidMap(List<Asteroid> asteroids)
        {
            _asteroids = asteroids;
        }

        public int GetMaximumVisibility()
        {
            return _asteroids
                .Select(a => VisibleFrom(a).Count())
                .OrderByDescending(count => count)
                .First();
        }

        private Asteroid GetMaximumVisibilityAsteroid()
        {
            return _asteroids
                .Select(a => (a, VisibleFrom(a).Count()))
                .OrderByDescending(x => x.Item2)
                .Select(x => x.Item1)
                .First();
        }

        private IEnumerable<Asteroid> VisibleFrom(Asteroid originAsteroid)
        {
            var asteroids = _asteroids.Where(a => a != originAsteroid).ToList();

            foreach (var asteroidToSee in asteroids)
            {
                var otherAsteroids = asteroids.Where(a => a != asteroidToSee);
                if (!otherAsteroids.Any(potentialBlocker => BlocksView(potentialBlocker, asteroidToSee, originAsteroid)))
                {
                    yield return asteroidToSee;
                }
            }
        }

        public int GetNthDestroyedAsteroidLocation(int n, bool render = true)
        {
            var orderedAsteroids = DestroyInOrder()
                .ToList();

            if (render)
            {
                var renderList = orderedAsteroids
                    .Select((a, i) => (a, i + 1))
                    .OrderBy(x => x.Item1.Y)
                    .ThenBy(x => x.Item1.X)
                    .ToArray();

                var index = 0;
                foreach (var y in Enumerable.Range(0, orderedAsteroids.Max(a => a.Y) + 1))
                {
                    Console.WriteLine();
                    Console.Write($"{(y + 1).ToString().PadLeft(2, ' ')}: ");
                    foreach (var x in Enumerable.Range(0, orderedAsteroids.Max(a => a.X) + 1))
                    {
                        if (index < renderList.Length
                            && x == renderList[index].Item1.X
                            && y == renderList[index].Item1.Y)
                        {
                            Console.Write($"[{renderList[index].Item2.ToString().PadLeft(3, ' ')}]");
                            index += 1;
                        }
                        else
                        {
                            Console.Write("-   -");
                        }
                    }

                    Console.WriteLine();
                }
            }

            var asteroid = orderedAsteroids.Skip(n - 1).First();

            return (asteroid.X + 1) * 100 + (asteroid.Y + 1);
        }

        private IEnumerable<Asteroid> DestroyInOrder()
        {
            var laserAsteroid = GetMaximumVisibilityAsteroid();
            
            while (_asteroids.Any(a => a != laserAsteroid))
            {
                var visibleAsteroids = VisibleFrom(laserAsteroid).ToList();
                var orderedAsteroids = visibleAsteroids.OrderBy(a => a.AngleFrom(laserAsteroid)).ToList();

                foreach (var asteroid in orderedAsteroids)
                {
                    _asteroids.Remove(asteroid);
                    yield return asteroid;
                }

                //break;
            }
        }

        private bool BlocksView(Asteroid potentialBlocker, Asteroid asteroidToSee, Asteroid originAsteroid)
        {
            var asteroidToSeeX = asteroidToSee.X - originAsteroid.X;
            var asteroidToSeeY = asteroidToSee.Y - originAsteroid.Y;
            var potentialBlockerX = potentialBlocker.X - originAsteroid.X;
            var potentialBlockerY = potentialBlocker.Y - originAsteroid.Y;

            if (asteroidToSeeX >= 0 != potentialBlockerX >= 0
                || asteroidToSeeY >= 0 != potentialBlockerY >= 0)
            {
                return false;
            }
            
            // In the same sector
            // Adjust to be in the positive sector
            if (asteroidToSeeX < 0)
            {
                asteroidToSeeX *= -1;
                potentialBlockerX *= -1;
            }
                
            if (asteroidToSeeY < 0)
            {
                asteroidToSeeY *= -1;
                potentialBlockerY *= -1;
            }
                
            if (potentialBlockerX > asteroidToSeeX || potentialBlockerY > asteroidToSeeY)
            {
                return false;
            }

            return potentialBlockerX * asteroidToSeeY == asteroidToSeeX * potentialBlockerY;
        }

        private class Asteroid
        {
            public int X { get; }
            public int Y { get; }

            public Asteroid(int x, int y)
            {
                X = x;
                Y = y;
            }

            public double AngleFrom(Asteroid laserAsteroid)
            {
                double x = X - laserAsteroid.X;
                double y = Y - laserAsteroid.Y;
                var angle = Math.Atan2(x, -y) * 180 / Math.PI;
                if (angle < 0)
                {
                    angle += 360;
                }
                return angle;
            }
        }
    }
}
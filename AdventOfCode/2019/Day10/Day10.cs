using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Shared;

namespace AdventOfCode._2019.Day10;

public class Day10 : Day
{
    public Day10() : base(2019, 10, "Day10/input_2019_10.txt", "326", "")
    {

    }

    private List<Asteroid> _asteroids;
    public override void Initialise()
    {
        _asteroids = InputLines
            .SelectMany(ReadLine)
            .ToList();
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

    public override string Part1()
    {
        return GetMaximumVisibility().ToString();
    }

    public override string Part2()
    {
        return GetNthDestroyedAsteroidLocation(200).ToString();
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
            .Select(a => (Asteroid: a, VisibleFromCount: VisibleFrom(a).Count()))
            .OrderByDescending(x => x.VisibleFromCount)
            .Select(x => x.Asteroid)
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
                .Select((a, i) => (Asteroid: a, Index: i + 1))
                .OrderBy(x => x.Asteroid.Y)
                .ThenBy(x => x.Asteroid.X)
                .ToArray();

            var index = 0;
            foreach (var y in Enumerable.Range(0, orderedAsteroids.Max(a => a.Y) + 1))
            {
                TraceLine();
                Trace($"{(y + 1).ToString().PadLeft(2, ' ')}: ");
                foreach (var x in Enumerable.Range(0, orderedAsteroids.Max(a => a.X) + 1))
                {
                    if (index < renderList.Length
                        && x == renderList[index].Asteroid.X
                        && y == renderList[index].Asteroid.Y)
                    {
                        Trace($"[{renderList[index].Index.ToString().PadLeft(3, ' ')}]");
                        index += 1;
                    }
                    else
                    {
                        Trace("-   -");
                    }
                }

                TraceLine();
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
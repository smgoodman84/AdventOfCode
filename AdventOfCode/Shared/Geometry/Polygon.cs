using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Shared.Geometry;

public class Polygon
{
    public Polygon(IEnumerable<LineSegment> lineSegments)
    {
        LineSegments = lineSegments.ToList();
        
        Vertices = LineSegments
            .Select(l => l.Start)
            .ToList();
        
        _boundaryLeft = Vertices.Select(v => v.X).Min() - 1;
    }

    public Polygon(IEnumerable<Coordinate2D> vertices)
    {
        Vertices = vertices.ToList();
        
        LineSegments = Vertices
            .Select((v, i) 
                => new LineSegment(v, Vertices[(i + 1) % Vertices.Count]))
            .ToList();
        
        _boundaryLeft = Vertices.Select(v => v.X).Min() - 1;
    }

    public List<LineSegment> LineSegments { get; }
    public List<Coordinate2D> Vertices { get; }

    private long _boundaryLeft;

    public double CalculateArea()
    {
        // https://www.geeksforgeeks.org/area-of-a-polygon-with-given-n-ordered-vertices
        double area = 0.0;

        var coordinates = LineSegments.Select(x => x.Start).ToList();

        // Calculate value of shoelace formula
        var previous = coordinates.Last();
        foreach(var current in coordinates)
        {
            area += (previous.X + current.X) * (previous.Y - current.Y);
            previous = current;
        }

        return Math.Abs(area / 2.0);
    }

    private Dictionary<Coordinate2D, ContainsContext> _containsContextCache = new ();
    public bool ContainsUsingContext(Coordinate2D coordinate)
    {
        var context = GetContainsContext(coordinate);
        return context.LocationType != LocationType.Outside;
    }

    private ContainsContext GetContainsContext(Coordinate2D coordinate)
    {
        if (_containsContextCache.TryGetValue(coordinate, out var result))
        {
            return result;
        }


        var x = _boundaryLeft;
        var leftContext = new ContainsContext
        {
            LocationType = LocationType.Outside,
            EdgeType = EdgeType.None
        };

        while (x <= coordinate.X)
        {
            
        }
        var boundaryCoord = new Coordinate2D(_boundaryLeft, coordinate.Y);
        if (_containsContextCache.TryGetValue(boundaryCoord, out var boundaryContext))
        {
            return result;
        }

        for (var x = _boundaryLeft; x <= coordinate.X; x++)
        {
            // warm the cache to prevent stack overflow
            var cacheCoordinate = new Coordinate2D(x, coordinate.Y);
            var cacheResult = GetContainsContextUncached(cacheCoordinate);
            _containsContextCache.Add(cacheCoordinate, cacheResult);
        }

        result = GetContainsContextUncached(coordinate);
        _containsContextCache.Add(coordinate, result);
        return result;
    }
    
    private ContainsContext GetContainsContextUncached(Coordinate2D coordinate, ContainsContext leftContext)
    {
        Console.WriteLine($"Calculating {coordinate}");
        var lineSegments = LineSegments
            .Where(ls => ls.IsOnLineSegment(coordinate))
            .ToList();

        if (lineSegments.Count == 0)
        {
            var location = leftContext.LocationType switch
            {
                LocationType.MovingOut => LocationType.Outside,
                LocationType.MovingIn => LocationType.Inside,
                _ => leftContext.LocationType
            };

            return new ContainsContext
            {
                LocationType = location,
                EdgeType = leftContext.EdgeType
            };
        }
        
        if (lineSegments.Count == 1)
        {
            var line = lineSegments.Single();
            if (line.IsVertical)
            {
                var location = leftContext.LocationType switch
                {
                    LocationType.Outside => LocationType.MovingIn,
                    LocationType.Inside => LocationType.MovingOut,
                    LocationType.MovingOut => LocationType.MovingIn,
                    LocationType.MovingIn => LocationType.MovingOut,
                };

                return new ContainsContext
                {
                    LocationType = location,
                    EdgeType = leftContext.EdgeType
                };
            }
        }

        if (lineSegments.Count == 2)
        {
            // on a corner
            var horizontalLine = lineSegments.Single(ls => ls.IsHorizontal);
            var verticalLine = lineSegments.Single(ls => ls.IsVertical);
            
            if (horizontalLine.IsOnLineSegment(coordinate.Left()))
            {
                if (verticalLine.IsOnLineSegment(coordinate.Up()))
                {
                    switch (leftContext.EdgeType)
                    {
                        case EdgeType.Bottom:
                            return new ContainsContext
                            {
                                LocationType = LocationType.MovingOut,
                                EdgeType = EdgeType.None
                            };
                        case EdgeType.Top:
                            return new ContainsContext
                            {
                                LocationType = LocationType.Inside,
                                EdgeType = EdgeType.None
                            };
                    }
                }
                
                if (verticalLine.IsOnLineSegment(coordinate.Down()))
                {
                    switch (leftContext.EdgeType)
                    {
                        case EdgeType.Bottom:
                            return new ContainsContext
                            {
                                LocationType = LocationType.Inside,
                                EdgeType = EdgeType.None
                            };
                        case EdgeType.Top:
                            return new ContainsContext
                            {
                                LocationType = LocationType.MovingOut,
                                EdgeType = EdgeType.None
                            };
                    }
                }
            }
            
            if (horizontalLine.IsOnLineSegment(coordinate.Right()))
            {
                if (verticalLine.IsOnLineSegment(coordinate.Up()))
                {
                    switch (leftContext.LocationType)
                    {
                        case LocationType.Outside:
                        case LocationType.MovingOut:
                            return new ContainsContext
                            {
                                LocationType = LocationType.MovingIn,
                                EdgeType = EdgeType.Bottom
                            };
                        case LocationType.Inside:
                        case LocationType.MovingIn:
                            return new ContainsContext
                            {
                                LocationType = LocationType.Inside,
                                EdgeType = EdgeType.Top
                            };
                    }
                }
                
                if (verticalLine.IsOnLineSegment(coordinate.Down()))
                {
                    switch (leftContext.LocationType)
                    {
                        case LocationType.Outside:
                        case LocationType.MovingOut:
                            return new ContainsContext
                            {
                                LocationType = LocationType.MovingIn,
                                EdgeType = EdgeType.Top
                            };
                        case LocationType.Inside:
                        case LocationType.MovingIn:
                            return new ContainsContext
                            {
                                LocationType = LocationType.Inside,
                                EdgeType = EdgeType.Bottom
                            };
                    }
                }
            }
        }

        return leftContext;
    }
    
    private Dictionary<Coordinate2D, bool> _containsCache = new Dictionary<Coordinate2D, bool>();
    public bool Contains(Coordinate2D coordinate)
    {
        if (_containsCache.TryGetValue(coordinate, out var result))
        {
            return result;
        }

        result = ContainsUncached(coordinate);
        _containsCache.Add(coordinate, result);
        return result;
    }
    
    private bool ContainsUncached(Coordinate2D coordinate)
    {
        var startX = _boundaryLeft;

        var edgeType = EdgeType.None;
        var location = LocationType.Outside;
        for (var x = startX; x <= coordinate.X; x++)
        {
            var current = new Coordinate2D(x, coordinate.Y);
            var lineSegments = LineSegments
                .Where(ls => ls.IsOnLineSegment(current))
                .ToList();

            if (lineSegments.Count == 0)
            {
                location = location switch
                {
                    LocationType.MovingOut => LocationType.Outside,
                    LocationType.MovingIn => LocationType.Inside,
                    _ => location
                };
            }
            
            if (lineSegments.Count == 1)
            {
                var line = lineSegments.Single();
                if (line.IsVertical)
                {
                    location = location switch
                    {
                        LocationType.Outside => LocationType.MovingIn,
                        LocationType.Inside => LocationType.MovingOut,
                        LocationType.MovingOut => LocationType.MovingIn,
                        LocationType.MovingIn => LocationType.MovingOut,
                    };
                }
            }

            if (lineSegments.Count == 2)
            {
                // on a corner
                var horizontalLine = lineSegments.Single(ls => ls.IsHorizontal);
                var verticalLine = lineSegments.Single(ls => ls.IsVertical);
                
                if (horizontalLine.IsOnLineSegment(current.Left()))
                {
                    if (verticalLine.IsOnLineSegment(current.Up()))
                    {
                        switch (edgeType)
                        {
                            case EdgeType.Bottom:
                                edgeType = EdgeType.None;
                                location = LocationType.MovingOut;
                                break;
                            case EdgeType.Top:
                                edgeType = EdgeType.None;
                                location = LocationType.Inside;
                                break;
                        }
                    }
                    
                    if (verticalLine.IsOnLineSegment(current.Down()))
                    {
                        switch (edgeType)
                        {
                            case EdgeType.Bottom:
                                edgeType = EdgeType.None;
                                location = LocationType.Inside;
                                break;
                            case EdgeType.Top:
                                edgeType = EdgeType.None;
                                location = LocationType.MovingOut;
                                break;
                        }
                    }
                }
                
                if (horizontalLine.IsOnLineSegment(current.Right()))
                {
                    if (verticalLine.IsOnLineSegment(current.Up()))
                    {
                        switch (location)
                        {
                            case LocationType.Outside:
                            case LocationType.MovingOut:
                                edgeType = EdgeType.Bottom;
                                location = LocationType.MovingIn;
                                break;
                            case LocationType.Inside:
                            case LocationType.MovingIn:
                                edgeType = EdgeType.Top;
                                location = LocationType.Inside;
                                break;
                        }
                    }
                    
                    if (verticalLine.IsOnLineSegment(current.Down()))
                    {
                        switch (location)
                        {
                            case LocationType.Outside:
                            case LocationType.MovingOut:
                                edgeType = EdgeType.Top;
                                location = LocationType.MovingIn;
                                break;
                            case LocationType.Inside:
                            case LocationType.MovingIn:
                                edgeType = EdgeType.Bottom;
                                location = LocationType.Inside;
                                break;
                        }
                    }
                }
            }
        }

        return location != LocationType.Outside;
    }

    private class ContainsContext
    {
        public EdgeType EdgeType { get; set; }
        public LocationType LocationType { get; set; }
    }

    private enum LocationType
    {
        Outside,
        MovingIn,
        Inside,
        MovingOut
    }

    private enum EdgeType
    {
        None,
        Top,
        Bottom
    }
}
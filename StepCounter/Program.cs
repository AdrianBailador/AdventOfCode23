using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

class Day21
{
    static int width = 0;
    static int height = 0;
    static char[,] map;

    static async Task Main(string[] args)
    {
        List<string> lines = (await File.ReadAllLinesAsync("C:\\Users\\Adrián Bailador\\Desktop\\Day21.txt")).ToList();

        Point startPosition = null;

        // Build map
        width = lines[0].Length;
        height = lines.Count;

        int y = 0;
        map = new char[width, height];
        foreach (string line in lines)
        {
            for (int x = 0; x < line.Length; x++)
            {
                map[x, y] = line[x];
                if (map[x, y] == 'S')
                {
                    startPosition = new Point(x, y);
                    map[x, y] = '.';
                }
            }
            y++;
        }

        HashSet<Point> toExplore = new HashSet<Point>(new[] { startPosition });

        // Part 1
        long startTime = Environment.TickCount;

        for (int i = 0; i < 64; i++)
        {
            toExplore = ExploreAdjacentPoints(toExplore);
        }

        long resultPart1 = toExplore.Count;
        Console.WriteLine($"Result part 1: {resultPart1} in {Environment.TickCount - startTime}ms");

        // Part 2
        startTime = Environment.TickCount;

        long count = 26501365;
        long cycles = count / width;
        long reminder = count % width;

        toExplore = new HashSet<Point>(new[] { startPosition });

        List<Point> regressionPoints = new List<Point>();

        int steps = 0;
        for (int i = 0; i < 3; i++)
        {
            while (steps < i * width + reminder)
            {
                toExplore = ExploreAdjacentPointsParallel(toExplore);
                steps++;
            }
            regressionPoints.Add(new Point(i, toExplore.Count));
        }

        long resultPart2 = CalculateQuadraticCurve(cycles, regressionPoints);

        Console.WriteLine($"Result part 2: {resultPart2} in {Environment.TickCount - startTime}ms");
    }

    private static HashSet<Point> ExploreAdjacentPoints(HashSet<Point> points)
    {
        return points.SelectMany(p => new[] { p.GetDown(), p.GetLeft(), p.GetUp(), p.GetRight() })
                     .Where(p => IsValid(map, p))
                     .ToHashSet();
    }

    private static HashSet<Point> ExploreAdjacentPointsParallel(HashSet<Point> points)
    {
        return points.AsParallel()
                     .SelectMany(p => new[] { p.GetDown(), p.GetLeft(), p.GetUp(), p.GetRight() })
                     .Where(p => IsValid2(map, p))
                     .ToHashSet();
    }

    private static long CalculateQuadraticCurve(long x, List<Point> regressionPoints)
    {
        Func<long, long> g = x =>
        {
            double x1 = regressionPoints[0].x;
            double y1 = regressionPoints[0].y;
            double x2 = regressionPoints[1].x;
            double y2 = regressionPoints[1].y;
            double x3 = regressionPoints[2].x;
            double y3 = regressionPoints[2].y;
            return (long)(((x - x2) * (x - x3)) / ((x1 - x2) * (x1 - x3)) * y1 +
                    ((x - x1) * (x - x3)) / ((x2 - x1) * (x2 - x3)) * y2 +
                    ((x - x1) * (x - x2)) / ((x3 - x1) * (x3 - x2)) * y3);
        };

        return g(x);
    }

    private static bool IsValid2(char[,] map, Point p)
    {
        int x = ((p.x % width) + width) % width;
        int y = ((p.y % height) + height) % height;
        return map[x, y] != '#';
    }

    private static bool IsValid(char[,] map, Point p)
    {
        return p.x >= 0 && p.x < width && p.y >= 0 && p.y < height && map[p.x, p.y] != '#';
    }

    private class Point
    {
        public int x, y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Point GetUp() { return new Point(x, y - 1); }
        public Point GetLeft() { return new Point(x - 1, y); }
        public Point GetRight() { return new Point(x + 1, y); }
        public Point GetDown() { return new Point(x, y + 1); }

        public override int GetHashCode()
        {
            int prime = 31;
            int result = 1;
            result = prime * result + x;
            result = prime * result + y;
            return result;
        }

        public override bool Equals(object other)
        {
            return (x == ((Point)other).x) && (y == ((Point)other).y);
        }
    }
}

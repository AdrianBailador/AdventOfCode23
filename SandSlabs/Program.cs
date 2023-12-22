using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        var bricks = LoadBricksFromFile(@"C:\Users\Adrián Bailador\Desktop\Day22.txt");

        // Sort bricks by Z-coordinate
        bricks = bricks.OrderBy(b => b.Z1).ToList();

        // Connect bricks based on support relationships
        ConnectBricks(bricks);

        // Part 1: Count bricks that are not the only supporter for all bricks placed above them
        Console.WriteLine($"Part 1: {bricks.Count(b => b.Above.All(a => a.Supporters.Count > 1))}");

        // Part 2: Count bricks that, when removed, cause all the bricks above them to fall
        Console.WriteLine($"Part 2: {CountBricksForPart2(bricks)}");
    }

    static List<Brick> LoadBricksFromFile(string filePath)
    {
        var input = File.ReadAllLines(filePath);
        var bricks = new List<Brick>();

        foreach (var line in input)
        {
            var coords = line.Split('~', ',').Select(int.Parse).ToArray();
            var brick = new Brick(coords[0], coords[1], coords[2], coords[3], coords[4], coords[5]);
            bricks.Add(brick);
            Debug.Assert(brick.X1 <= brick.X2 && brick.Y1 <= brick.Y2 && brick.Z1 <= brick.Z2);
        }

        return bricks;
    }

    static void ConnectBricks(List<Brick> bricks)
    {
        // Bricks start falling downwards. We have ordered them by Z1, so we can just iterate over them.
        for (var i = 0; i < bricks.Count; i++)
        {
            var brick = bricks[i];
            var z = brick.Z1;

            while (z > 0)
            {
                var supportingBricks = bricks.Where(b =>
                        b.Z2 == z - 1 &&
                        brick.Xs.Intersect(b.Xs).Any() &&
                        brick.Ys.Intersect(b.Ys).Any())
                    .ToList();

                if (supportingBricks.Count > 0 || z == 1)
                {
                    foreach (var b in supportingBricks)
                    {
                        b.Above.Add(brick);
                        brick.Supporters.Add(b);
                    }
                    bricks[i] = brick;
                    break;
                }

                z--;
                brick = brick with { Z1 = z, Z2 = z + brick.Zs.Length - 1 };
            }
        }
    }

    static int CountBricksForPart2(List<Brick> bricks)
    {
        var part2 = 0;

        foreach (var brick in bricks)
        {
            var queue = new Queue<Brick>();
            queue.Enqueue(brick);
            var disintegrated = new HashSet<Brick>();

            while (queue.TryDequeue(out var currentBrick))
            {
                disintegrated.Add(currentBrick);

                // Count all bricks that are supported by the current brick and have all their supporters
                // disintegrated, and enqueue them to be processed.
                foreach (var above in currentBrick.Above.Where(above => above.Supporters.All(disintegrated.Contains)))
                {
                    part2++;
                    queue.Enqueue(above);
                }
            }
        }

        return part2;
    }

    internal record Brick(int X1, int Y1, int Z1, int X2, int Y2, int Z2)
    {
        public List<Brick> Above { get; set; } = new();
        public List<Brick> Supporters { get; set; } = new();

        public int[] Xs => Enumerable.Range(X1, X2 - X1 + 1).ToArray();
        public int[] Ys => Enumerable.Range(Y1, Y2 - Y1 + 1).ToArray();
        public int[] Zs => Enumerable.Range(Z1, Z2 - Z1 + 1).ToArray();
    }
}

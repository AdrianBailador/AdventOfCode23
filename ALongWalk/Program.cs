using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

class Program
{
    static void Main()
    {
        string filePath = "C:\\Users\\Adrián Bailador\\Desktop\\Day23.txt";

        var input = File.ReadAllLines(filePath);

        var start = new Position(0, 1);
        var end = new Position(input.Length - 1, input[0].Length - 2);

        var stopwatch = new Stopwatch();

        stopwatch.Start();
        var graph = BuildGraph(input);
        var part1Result = FindLongestPath(graph, new HashSet<Position>(), start, end);
        stopwatch.Stop();

        Console.WriteLine($"Part 1: {part1Result}");
        Console.WriteLine($"Time for Part 1: {stopwatch.ElapsedMilliseconds} ms");

        stopwatch.Reset();

        stopwatch.Start();
        graph = BuildGraph(input, true);
        var part2Result = FindLongestPath(graph, new HashSet<Position>(), start, end);
        stopwatch.Stop();

        Console.WriteLine($"Part 2: {part2Result}");
        Console.WriteLine($"Time for Part 2: {stopwatch.ElapsedMilliseconds} ms");
    }
    static Dictionary<Position, HashSet<Position>> BuildGraph(string[] input, bool part2 = false)
    {
        var graph = new Dictionary<Position, HashSet<Position>>();

        for (var row = 0; row < input.Length; row++)
        {
            var line = input[row];

            for (var col = 0; col < line.Length; col++)
            {
                if (line[col] == '#')
                {
                    continue;
                }

                var pos = new Position(row, col);
                graph[pos] = new HashSet<Position>();

                if (!part2)
                {
                    switch (line[col])
                    {
                        case '>':
                            graph[pos].Add(pos.Move(Direction.Right));
                            continue;
                        case 'v':
                            graph[pos].Add(pos.Move(Direction.Down));
                            continue;
                        case '<':
                            graph[pos].Add(pos.Move(Direction.Left));
                            continue;
                    }
                }

                if (row > 0 && input[row - 1][col] != '#')
                {
                    graph[pos].Add(pos.Move(Direction.Up));
                }

                if (row < input.Length - 1 && input[row + 1][col] != '#')
                {
                    graph[pos].Add(pos.Move(Direction.Down));
                }

                if (col > 0 && input[row][col - 1] != '#')
                {
                    graph[pos].Add(pos.Move(Direction.Left));
                }

                if (col < line.Length - 1 && input[row][col + 1] != '#')
                {
                    graph[pos].Add(pos.Move(Direction.Right));
                }
            }
        }

        return graph;
    }

    static int FindLongestPath(Dictionary<Position, HashSet<Position>> graph, HashSet<Position> visited, Position current, Position end)
    {
        var result = 0;

        if (current == end)
        {
            return visited.Count;
        }

        foreach (var neighbor in graph[current])
        {
            if (!visited.Add(neighbor))
            {
                continue;
            }

            var length = FindLongestPath(graph, visited, neighbor, end);
            result = Math.Max(result, length);
            visited.Remove(neighbor);
        }

        return result;
    }

    internal record Direction(int Row, int Col)
    {
        public static Direction Up = new(-1, 0);
        public static Direction Down = new(1, 0);
        public static Direction Left = new(0, -1);
        public static Direction Right = new(0, 1);
    }

    internal record Position(int Row, int Col)
    {
        public Position Move(Direction dir)
        {
            return new Position(Row + dir.Row, Col + dir.Col);
        }
    }
}

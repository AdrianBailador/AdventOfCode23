using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var inputFilePath = @"C:\\Users\\Adrián Bailador\\Desktop\\Day16.txt";
        var input = File.ReadAllLines(inputFilePath).Select(l => l.ToCharArray()).ToArray();

        // Part 1
        var timer = Stopwatch.StartNew();
        var part1Result = CountTiles(input, 0, -1, Directions.Right);
        Console.WriteLine($"Part 1: {part1Result}, Elapsed time: {timer.ElapsedMilliseconds}ms");

        // Part 2
        timer.Restart();
        var tasks = new List<Task<int>>();
        tasks.AddRange(GetTasksForRowDirections(input));
        tasks.AddRange(GetTasksForColumnDirections(input));

        await Task.WhenAll(tasks);

        var part2Result = tasks.Max(t => t.Result);
        Console.WriteLine($"Part 2: {part2Result}, Elapsed time: {timer.ElapsedMilliseconds}ms");
    }

    static IEnumerable<Task<int>> GetTasksForRowDirections(char[][] input)
    {
        for (var row = 0; row < input.Length; row++)
        {
            var currentRow = row;
            yield return Task.Run(() => CountTiles(input, currentRow, -1, Directions.Right));
            yield return Task.Run(() => CountTiles(input, currentRow, input[currentRow].Length, Directions.Left));
        }
    }

    static IEnumerable<Task<int>> GetTasksForColumnDirections(char[][] input)
    {
        for (var col = 0; col < input[0].Length; col++)
        {
            var currentCol = col;
            yield return Task.Run(() => CountTiles(input, -1, currentCol, Directions.Down));
            yield return Task.Run(() => CountTiles(input, input.Length, currentCol, Directions.Up));
        }
    }

    static int CountTiles(char[][] map, int startRow, int startColumn, Directions startDirection)
    {
        var tiles = new Dictionary<(int row, int col), Directions>
        {
            [(startRow, startColumn)] = Directions.None // Add the entry point which is outside the map
        };

        var beams = new Queue<(int Row, int Column, Directions Direction)>();
        beams.Enqueue((startRow, startColumn, startDirection));

        while (beams.TryDequeue(out var beam))
        {
            // Check if the tile has been visited in the same direction
            if (tiles.TryGetValue((beam.Row, beam.Column), out var tileDirections) &&
                tileDirections.HasFlag(beam.Direction))
            {
                continue; // Skip if already visited in the same direction
            }

            // Mark the tile as visited
            tiles[(beam.Row, beam.Column)] = tileDirections | beam.Direction;

            // Determine the next position based on the current direction
            var (row, col) = beam.Direction switch
            {
                Directions.Up => (beam.Row - 1, beam.Column),
                Directions.Down => (beam.Row + 1, beam.Column),
                Directions.Left => (beam.Row, beam.Column - 1),
                Directions.Right => (beam.Row, beam.Column + 1),
                _ => throw new Exception("Invalid direction")
            };

            // Skip if the next position is outside the map
            if (row < 0 || row >= map.Length || col < 0 || col >= map[row].Length)
            {
                continue;
            }

            // Update the beam's position
            beam = beam with { Row = row, Column = col };

            // Process the current tile based on its content
            switch (map[row][col])
            {
                case '/':
                    // Change direction based on the tile's content
                    beams.Enqueue(beam with
                    {
                        Direction = beam.Direction switch
                        {
                            Directions.Up => Directions.Right,
                            Directions.Down => Directions.Left,
                            Directions.Left => Directions.Down,
                            Directions.Right => Directions.Up,
                            _ => throw new Exception("Invalid direction")
                        }
                    });
                    break;
                case '\\':
                    // Change direction based on the tile's content
                    beams.Enqueue(beam with
                    {
                        Direction = beam.Direction switch
                        {
                            Directions.Up => Directions.Left,
                            Directions.Down => Directions.Right,
                            Directions.Left => Directions.Up,
                            Directions.Right => Directions.Down,
                            _ => throw new Exception("Invalid direction")
                        }
                    });
                    break;
                case '-' when beam.Direction is Directions.Up or Directions.Down:
                    // Continue straight and add branches
                    beams.Enqueue(beam with { Direction = Directions.Left });
                    beams.Enqueue(beam with { Direction = Directions.Right });
                    break;
                case '|' when beam.Direction is Directions.Left or Directions.Right:
                    // Continue straight and add branches
                    beams.Enqueue(beam with { Direction = Directions.Up });
                    beams.Enqueue(beam with { Direction = Directions.Down });
                    break;
                default: // '.', '-' or '|' in the same direction
                         // Continue straight
                    beams.Enqueue(beam);
                    break;
            }
        }

        return tiles.Count - 1; // Exclude the entry point
    }
}

[Flags]
internal enum Directions
{
    None = 0,
    Up = 1,
    Down = 2,
    Left = 4,
    Right = 8
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        var filePath = "C:\\Users\\Adrián Bailador\\Desktop\\Day14.txt";
        var lines = File.ReadAllLines(filePath);

        // Inicializa la variable input
        input = lines.Select(s => s.ToCharArray()).ToArray();

        TiltNorth();

        var part1 = 0;

        for (var score = input.Length; score > 0; score--)
        {
            part1 += input[^score].Count(c => c == 'O') * score;
        }

        Console.WriteLine($"Part 1: {part1}");

        var cache = new Dictionary<string, int>();
        var cycle = 1;

        while (cycle <= 1_000_000_000)
        {
            TiltNorth();
            TiltWest();
            TiltSouth();
            TiltEast();

            var current = string.Join(string.Empty, input.SelectMany(c => c));

            if (cache.TryGetValue(current, out var cached))
            {
                var remaining = 1_000_000_000 - cycle - 1;
                var loop = cycle - cached;

                var loopRemaining = remaining % loop;
                cycle = 1_000_000_000 - loopRemaining - 1;
            }

            cache[current] = cycle++;
        }

        var part2 = 0;

        for (var score = input.Length; score > 0; score--)
        {
            part2 += input[^score].Count(c => c == 'O') * score;
        }

        Console.WriteLine($"Part 2: {part2}");
    }

    static char[][] input;

    static void TiltNorth()
    {
        for (var row = 1; row < input.Length; row++)
        {
            for (var col = 0; col < input[row].Length; col++)
            {
                var c = input[row][col];

                if (c != 'O')
                {
                    continue;
                }

                var previous = 1;
                while (input[row - previous][col] == '.')
                {
                    input[row - previous][col] = 'O';
                    input[row - previous + 1][col] = '.';
                    previous++;

                    if (row - previous < 0)
                    {
                        break;
                    }
                }
            }
        }
    }

    static void TiltSouth()
    {
        for (var row = input.Length - 2; row >= 0; row--)
        {
            for (var col = 0; col < input[row].Length; col++)
            {
                var c = input[row][col];

                if (c != 'O')
                {
                    continue;
                }

                var previous = 1;
                while (input[row + previous][col] == '.')
                {
                    input[row + previous][col] = 'O';
                    input[row + previous - 1][col] = '.';
                    previous++;

                    if (row + previous >= input.Length)
                    {
                        break;
                    }
                }
            }
        }
    }

    static void TiltWest()
    {
        for (var row = 0; row < input.Length; row++)
        {
            for (var col = 1; col < input[row].Length; col++)
            {
                var c = input[row][col];

                if (c != 'O')
                {
                    continue;
                }

                var previous = 1;
                while (input[row][col - previous] == '.')
                {
                    input[row][col - previous] = 'O';
                    input[row][col - previous + 1] = '.';
                    previous++;

                    if (col - previous < 0)
                    {
                        break;
                    }
                }
            }
        }
    }

    static void TiltEast()
    {
        for (var row = 0; row < input.Length; row++)
        {
            for (var col = input[row].Length - 2; col >= 0; col--)
            {
                var c = input[row][col];

                if (c != 'O')
                {
                    continue;
                }

                var previous = 1;
                while (input[row][col + previous] == '.')
                {
                    input[row][col + previous] = 'O';
                    input[row][col + previous - 1] = '.';
                    previous++;

                    if (col + previous >= input[row].Length)
                    {
                        break;
                    }
                }
            }
        }
    }
}

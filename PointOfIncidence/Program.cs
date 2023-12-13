using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        string filePath = "C:\\Users\\Adrián Bailador\\Desktop\\Day13.txt";
        var input = File.ReadAllText(filePath).Split("\r\n\r\n").Select(p => p.Split("\r\n")).ToList();

        var part1Rows = new List<int>();
        var part1Cols = new List<int>();

        for (var i = 0; i < input.Count; i++)
        {
            var (rows, cols) = CheckPattern(input, part1Rows, part1Cols, i);
            part1Rows.Add(rows);
            part1Cols.Add(cols);
        }

        Console.WriteLine($"Part 1: {part1Rows.Sum() + part1Cols.Sum()}");

        var part2Rows = 0;
        var part2Cols = 0;

        for (var i = 0; i < input.Count; i++)
        {
            var (rows, cols) = CheckPattern(input, part1Rows, part1Cols, i, 1);

            part2Rows += rows;
            part2Cols += cols;
        }

        Console.WriteLine($"Part 2: {part2Rows + part2Cols}");
    }

    static (int Rows, int Columns) CheckPattern(List<string[]> input, List<int> part1Rows, List<int> part1Cols, int patternIndex, int offset = 0)
    {
        var pattern = input[patternIndex];
        var rows = 0;
        var cols = 0;

        for (var i = 1; i < pattern.Length; i++) // Check rows.
        {
            var rowOffset = offset;
            var isMirrorRow = true;

            for (var j = 1; j <= pattern.Length - i; j++)
            {
                if (j > i || j + i > pattern.Length) break; // Outside of pattern bounds.

                var row1 = pattern[i + j - 1];
                var row2 = pattern[i - j];

                // Count the number of differences between the two rows
                var diff = row1.Select((row, rowIndex) => row == row2[rowIndex] ? 0 : 1).Sum();

                // Check if the row is a mirror of the adjacent row or the difference is within the offset.
                isMirrorRow &= diff == rowOffset || diff == 0;

                if (isMirrorRow && diff == rowOffset)
                {
                    rowOffset = 0; // Offset already used, so set it to 0.
                }
            }

            if (isMirrorRow && rowOffset == 0 && (offset == 0 || part1Rows[patternIndex] != i))
            {
                rows += i * 100;
                break;
            }
        }

        for (var i = 1; i < pattern[0].Length; i++) // Check columns.
        {
            var columnOffset = offset;
            var isMirrorColumn = true;

            for (var j = 1; j <= pattern[0].Length - i; j++)
            {
                if (j > i || j + i > pattern[0].Length) break; // Outside of pattern bounds.

                var col1 = pattern.Select(p => p[i + j - 1]).ToArray();
                var col2 = pattern.Select(p => p[i - j]).ToArray();

                // Count the number of differences between the two columns
                var diff = col1.Select((col, colIndex) => col == col2[colIndex] ? 0 : 1).Sum();

                // Check if the column is a mirror of the adjacent column or the difference is within the offset.
                isMirrorColumn &= diff == columnOffset || diff == 0;

                if (isMirrorColumn && diff == columnOffset)
                {
                    columnOffset = 0; // Offset already used, so set it to 0.
                }
            }

            if (isMirrorColumn && columnOffset == 0 &&
                (offset == 0 || part1Cols[patternIndex] != i))
            {
                cols += i;
                break;
            }
        }

        return (rows, cols);
    }
}

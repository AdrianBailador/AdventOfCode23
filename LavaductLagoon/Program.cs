using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main()
    {
        var input = File.ReadAllLines(@"C:\Users\Adrián Bailador\Desktop\Day18.txt");

        var polygon1 = new List<(long Row, long Col)>();
        (long Row, long Col) currentPosition1 = (0, 0);
        var circumference1 = 0.0;

        var polygon2 = new List<(long Row, long Col)>();
        (long Row, long Col) currentPosition2 = (0, 0);
        var circumference2 = 0.0;

        foreach (var line in input)
        {
            polygon1.Add(currentPosition1);
            var move = line.Split(' ');

            var length1 = int.Parse(move[1]);
            currentPosition1 = move[0] switch
            {
                "R" => (currentPosition1.Row, currentPosition1.Col + length1),
                "D" => (currentPosition1.Row + length1, currentPosition1.Col),
                "L" => (currentPosition1.Row, currentPosition1.Col - length1),
                "U" => (currentPosition1.Row - length1, currentPosition1.Col),
                _ => throw new Exception("Unknown direction")
            };

            circumference1 += length1;

            polygon2.Add(currentPosition2);
            var hex = move[2].TrimStart('(').TrimEnd(')');
            var length2 = long.Parse(hex[1..^1], System.Globalization.NumberStyles.HexNumber);

            currentPosition2 = hex.Last() switch
            {
                '0' => (currentPosition2.Row, currentPosition2.Col + length2), // R
                '1' => (currentPosition2.Row + length2, currentPosition2.Col), // D
                '2' => (currentPosition2.Row, currentPosition2.Col - length2), // L
                '3' => (currentPosition2.Row - length2, currentPosition2.Col), // U
                _ => throw new Exception("Unknown direction")
            };

            circumference2 += length2;
        }

        Console.WriteLine($"Part 1: {Area(polygon1) + circumference1 / 2 + 1}");
        Console.WriteLine($"Part 2: {Area(polygon2) + circumference2 / 2 + 1}");
        return;
    }

    // Shoelace formula, found it on the internet:
    // https://rosettacode.org/wiki/Shoelace_formula_for_polygonal_area#C#
    static double Area(List<(long Row, long Col)> polygon)
    {
        var n = polygon.Count;
        var result = 0.0;
        for (var i = 0; i < n - 1; i++)
        {
            result += polygon[i].Row * polygon[i + 1].Col - polygon[i + 1].Row * polygon[i].Col;
        }

        result = Math.Abs(result + polygon[n - 1].Row * polygon[0].Col - polygon[0].Row * polygon[n - 1].Col) / 2.0;
        return result;
    }
}

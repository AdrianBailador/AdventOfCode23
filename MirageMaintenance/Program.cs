using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

class Program
{
    static long GetNextInSequence(long[] values)
    {
        if (values.All(v => v == 0))
            return 0;

        var decompositions = new long[values.Length - 1];

        for (var i = 0; i < values.Length - 1; i++)
            decompositions[i] = values[i + 1] - values[i];

        var delta = GetNextInSequence(decompositions);

        return values[values.Length - 1] + delta;
    }

    static void Main()
    {
        try
        {
            // Obtener la ruta completa del archivo
            string filePath = @"C:\Users\Adrián Bailador\Desktop\Day9.txt";

            // Leer líneas del archivo
            var fileContents = File.ReadAllLines(filePath);

            // Parte 1
            var partOneResult = fileContents
                .Select(l => l.ParseLongs().ToArray())
                .Select(GetNextInSequence)
                .Sum();
            Console.WriteLine($"Part 1 Result: {partOneResult}");

            // Parte 2
            var partTwoResult = fileContents
                .Select(l => l.ParseLongs().Reverse().ToArray())
                .Select(GetNextInSequence)
                .Sum();
            Console.WriteLine($"Part 2 Result: {partTwoResult}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocurrió un error: {ex.Message}");
        }
    }
}

public static class Extensions
{
    public static IEnumerable<long> ParseLongs(this string input)
    {
        foreach (var match in Regex.Matches(input, @"-?\d+"))
        {
            if (long.TryParse(match.ToString(), out var value))
                yield return value;
        }
    }
}

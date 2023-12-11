using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

class Program
{
    static long PathLength(char[] moves, Dictionary<string, (string Left, string Right)> nodes, string node, string ending)
    {
        long count = 0;
        long curr = 0;
        while (!node.EndsWith(ending))
        {
            node = moves[curr] == 'L' ? nodes[node].Left : nodes[node].Right;
            curr = (curr + 1) % moves.Length;
            ++count;
        }

        return count;
    }

    static long GCD(long a, long b)
    {
        long remainder;

        while (b != 0)
        {
            remainder = a % b;
            a = b;
            b = remainder;
        }

        return a;
    }

    static long LCM(long a, long b) => (a * b) / GCD(a, b);

    static void Main()
    {
        // Ruta del archivo
        string filePath = @"C:\Users\Adrián Bailador\Desktop\Day8.txt";

        try
        {
            // Leer líneas del archivo
            var lines = File.ReadAllLines(filePath);

            // Obtener movimientos y nodos
            var moves = lines[0].ToCharArray();
            var nodes = lines[2..].Select(line => Regex.Match(line, @"(\w+) = \((\w+), (\w+)\)"))
                .Select(m => (m.Groups[1].Value, (m.Groups[2].Value, m.Groups[3].Value)))
                .ToDictionary(e => e.Item1, e => e.Item2);

            // Calcular y mostrar resultado para P1
            Console.WriteLine($"P1: {PathLength(moves, nodes, "AAA", "ZZZ")}");


            // Calcular y mostrar resultado para P2
            var p2 = nodes.Keys.Where(k => k[2] == 'A')
                .Select(p => PathLength(moves, nodes, p, "Z"))
                .Aggregate(LCM);
            Console.WriteLine($"P2: {p2}");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"El archivo {filePath} no se encontró.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocurrió un error: {ex.Message}");
        }
    }
}

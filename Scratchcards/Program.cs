using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        var lines = File.ReadAllLines(@"C:\Users\Adrián Bailador\Desktop\Day4.txt");
        int sum = 0;

        foreach (var line in lines)
        {
            var parts = line.Split('|');
            var winning = parts[0].Split(':')[1].Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).Select(int.Parse).ToList();
            var yours = parts[1].Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).Select(int.Parse).ToList();

            int wins = yours.Count(x => winning.Contains(x));

            if (wins > 0)
            {
                sum += 1 << (wins - 1);
            }
        }

        Console.WriteLine("Part 1: " + sum);

        var counts = new int[lines.Length];
        for (int i = 0; i < counts.Length; i++)
        {
            counts[i] = 1;
        }

        for (int index = 0; index < lines.Length; index++)
        {
            var currentCardCount = counts[index];

            var parts = lines[index].Split('|');
            var winning = new HashSet<int>(parts[0].Split(':')[1].Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).Select(int.Parse));
            var yours = new HashSet<int>(parts[1].Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).Select(int.Parse));

            int wins = yours.Count(x => winning.Contains(x));

            for (int i = 0; i < wins; i++)
            {
                if (index + i + 1 < counts.Length)
                {
                    counts[index + i + 1] += currentCardCount;
                }
            }
        }

        Console.WriteLine("Part 2: " + counts.Sum());
    }
}

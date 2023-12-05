using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        var input = File.ReadAllLines("C:/Users/Adrián Bailador/Desktop/Day5.txt");

        // Start a timer to measure execution time
        var watch = Stopwatch.StartNew();

        // Get the initial seeds
        var seeds = input[0].Split(' ').Skip(1).Select(x => long.Parse(x)).ToList();

        // List to store seed transformations
        var maps = new List<List<(long from, long to, long adjustment)>>();

        // Temporary list to store currently processed seed transformations
        List<(long from, long to, long adjustment)>? currMap = null;

        // Process input lines to obtain seed transformations
        foreach (var line in input.Skip(2))
        {
            if (line.EndsWith(':'))
            {
                // Start a new seed transformation
                currMap = new List<(long from, long to, long adjustment)>();
                continue;
            }
            else if (line.Length == 0 && currMap != null)
            {
                // Finish the current seed transformation and add it to the main list
                maps.Add(currMap!);
                currMap = null;
                continue;
            }

            // Get information from the current line and add it to the current seed transformation
            var nums = line.Split(' ').Select(x => long.Parse(x)).ToArray();
            currMap!.Add((nums[1], nums[1] + nums[2] - 1, nums[0] - nums[1]));
        }

        // Add the last seed transformation if it exists
        if (currMap != null)
            maps.Add(currMap);

        // Part 1: Apply transformations to seeds and find the minimum value
        var result1 = long.MaxValue;
        foreach (var seed in seeds)
        {
            var value = seed;
            foreach (var map in maps)
            {
                foreach (var item in map)
                {
                    if (value >= item.from && value <= item.to)
                    {
                        value += item.adjustment;
                        break;
                    }
                }
            }
            result1 = Math.Min(result1, value);
        }

        // Part 2: Apply inverse transformations to seeds and find the minimum value
        var ranges = new List<(long from, long to)>();
        for (int i = 0; i < seeds.Count; i += 2)
            ranges.Add((from: seeds[i], to: seeds[i] + seeds[i + 1] - 1));

        foreach (var map in maps)
        {
            // Sort transformations by the source start
            var orderedMap = map.OrderBy(x => x.from).ToList();
            var newRanges = new List<(long from, long to)>();

            foreach (var r in ranges)
            {
                var range = r;
                foreach (var mapping in orderedMap)
                {
                    if (range.from < mapping.from)
                    {
                        // Add the part before the transformation
                        newRanges.Add((range.from, Math.Min(range.to, mapping.from - 1)));
                        range.from = mapping.from;
                        if (range.from >= range.to)
                            break;
                    }

                    if (range.from <= mapping.to)
                    {
                        // Apply the transformation
                        newRanges.Add((range.from + mapping.adjustment, Math.Min(range.to, mapping.to) + mapping.adjustment));
                        range.from = mapping.to + 1;
                        if (range.from >= range.to)
                            break;
                    }
                }
                if (range.from < range.to)
                    newRanges.Add(range);
            }
            ranges = newRanges;
        }

        // Find the minimum value after all transformations
        var result2 = ranges.Min(r => r.from);

        // Stop the timer and display results and execution time
        watch.Stop();
        Console.WriteLine($"Result1 = {result1}");
        Console.WriteLine($"Result2 = {result2}");
        Console.WriteLine($"Took = {watch.ElapsedMilliseconds}ms");
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

class Program
{
    public class Schematic
    {
        public int? Val { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public int Line { get; set; }
        public string Type { get; set; }
    }

    static void Main()
    {
        var pattern = new Regex(@"(\d+|[^\.\d\n])");
        var lines = File.ReadLines(@"C:\Users\Adrián Bailador\Desktop\Day3.txt").Select((line, index) => new { Line = line, Index = index });

        var schematic = lines.SelectMany(x => pattern.Matches(x.Line).Cast<Match>().Select(m => ParseMatch(m, x.Index))).ToList();

        // Part 1
        var part1 = schematic.Where(s => s.Type == "num" && FindAdjacent(schematic, s, new[] { "gear", "part" }).Any()).Sum(s => s.Val);
        Console.WriteLine("Day 3 (Part 1): " + part1);

        // Part 2
        var part2 = schematic.Where(s => s.Type == "gear").Sum(s => GearVal(FindAdjacent(schematic, s, new[] { "num" })));
        Console.WriteLine("Day 3 (Part 2): " + part2);
    }

    static Schematic ParseMatch(Match match, int line)
    {
        var val = match.Groups[1].Value.All(char.IsDigit) ? int.Parse(match.Groups[1].Value) : (int?)null;
        var type = match.Groups[1].Value.All(char.IsDigit) ? "num" : match.Groups[1].Value == "*" ? "gear" : "part";
        return new Schematic { Val = val, Start = match.Index, End = match.Index + match.Length - 1, Line = line, Type = type };
    }

    static List<Schematic> FindAdjacent(List<Schematic> schematic, Schematic row, string[] types)
    {
        return schematic.Where(s => types.Contains(s.Type) && Math.Abs(s.Line - row.Line) <= 1 && s.Start <= row.End + 1 && s.End >= row.Start - 1).ToList();
    }

    static int GearVal(List<Schematic> adjacent)
    {
        return adjacent.Count == 2 ? adjacent[0].Val.Value * adjacent[1].Val.Value : 0;
    }
}

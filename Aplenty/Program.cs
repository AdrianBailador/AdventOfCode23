using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    private static Dictionary<string, Workflow> workflows;
    private static List<Dictionary<string, int>> parts;

    static void Main()
    {
        var input = File.ReadAllText(@"C:\Users\Adrián Bailador\Desktop\Day19.txt").Split("\r\n\r\n");

        workflows = input[0].Split("\r\n").Select(workflow => new Workflow(workflow)).ToDictionary(w => w.Name);

        var messages = input[1].Split("\r\n");
        parts = new List<Dictionary<string, int>>();

        foreach (var message in messages)
        {
            var part = new Dictionary<string, int>();
            var values = message[1..^1].Split(',').Select(r => r.Split('='));

            foreach (var xmas in values)
            {
                part[xmas[0]] = int.Parse(xmas[1]);
            }

            parts.Add(part);
        }

        var part1 = 0;
        foreach (var part in parts)
        {
            var workflow = workflows["in"];
            var processing = true;

            while (processing)
            {
                foreach (var rule in workflow.Rules)
                {
                    var isValid = rule.Comparison == default;

                    if (!isValid)
                    {
                        var value = part[rule.Xmas];
                        isValid = rule.Comparison == '>' && value > rule.Value ||
                                  rule.Comparison == '<' && value < rule.Value;
                    }

                    if (isValid)
                    {
                        switch (rule.Destination)
                        {
                            case "A":
                                part1 += part.Sum(p => p.Value);
                                processing = false;
                                break;
                            case "R":
                                processing = false;
                                break;
                            default:
                                workflow = workflows[rule.Destination];
                                break;
                        }

                        break;
                    }
                }
            }
        }

        Console.WriteLine($"Part 1: {part1}");

        var candidates = new Dictionary<string, (int Min, int Max)>
        {
            ["x"] = (1, 4000),
            ["m"] = (1, 4000),
            ["a"] = (1, 4000),
            ["s"] = (1, 4000)
        };

        Console.WriteLine($"Part 2: {ProcessRanges("in", candidates)}");
    }

    static long ProcessRanges(string position, Dictionary<string, (int Min, int Max)> ranges)
    {
        switch (position)
        {
            case "A":
                return ranges.Values
                    .Aggregate<(int Min, int Max), long>(1, (current, range) => current * (range.Max - range.Min + 1));
            case "R":
                return 0;
        }

        long result = 0;
        var workflow = workflows[position];

        foreach (var rule in workflow.Rules)
        {
            var (min, max) = string.IsNullOrEmpty(rule.Xmas) ? (0, 0) : ranges[rule.Xmas];

            switch (rule.Comparison)
            {
                case '<':
                    if (max < rule.Value)
                    {
                        result += ProcessRanges(rule.Destination, ranges);
                        return result;
                    }

                    if (min < rule.Value)
                    {
                        var newRanges = new Dictionary<string, (int Min, int Max)>(ranges)
                        {
                            [rule.Xmas] = (min, rule.Value - 1)
                        };
                        result += ProcessRanges(rule.Destination, newRanges);

                        ranges[rule.Xmas] = (rule.Value, max);
                    }
                    break;
                case '>':
                    if (min > rule.Value)
                    {
                        result += ProcessRanges(rule.Destination, ranges);
                        return result;
                    }

                    if (max > rule.Value)
                    {
                        var newRanges = new Dictionary<string, (int Min, int Max)>(ranges)
                        {
                            [rule.Xmas] = (rule.Value + 1, max)
                        };
                        result += ProcessRanges(rule.Destination, newRanges);

                        ranges[rule.Xmas] = (min, rule.Value);
                    }
                    break;
                default:
                    result += ProcessRanges(rule.Destination, ranges);
                    break;
            }
        }

        return result;
    }

    internal struct Workflow
    {
        public Workflow(string pattern)
        {
            Name = pattern.Split('{')[0];
            var rules = pattern.Split('{')[1].TrimEnd('}').Split(',').ToList();
            Rules = rules.Select(rule => new Rule(rule)).ToList();
        }

        public string Name { get; set; }
        public List<Rule> Rules { get; set; }
    }

    internal struct Rule
    {
        public Rule(string rule)
        {
            var splitRule = rule.Split('<', '>');

            if (splitRule.Length == 2)
            {
                var right = splitRule[1].Split(':');

                Xmas = splitRule[0];
                Value = int.Parse(right[0]);
                Destination = right[1];
                Comparison = rule[1];
            }
            else
            {
                Destination = rule;
            }
        }

        public string Xmas { get; set; }
        public char Comparison { get; set; }
        public int Value { get; set; }
        public string Destination { get; set; }
    }
}

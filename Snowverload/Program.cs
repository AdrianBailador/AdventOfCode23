using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Days
{
    class Graph
    {
        private readonly int _verticesCount;
        private readonly Random _random;
        private readonly List<(string from, string to)> _edges;

        public Graph(List<(string from, string to)> edges, Random random)
        {
            _verticesCount = edges.SelectMany(x => new List<string> { x.from, x.to }).Distinct().Count();
            _edges = edges;
            _random = random;
        }

        public (int minCut, int vertices1Count, int vertices2Count) Cut()
        {
            var contractedEdges = _edges;
            var contractedVerticesCount = _verticesCount;
            var contracted = new Dictionary<string, List<string>>();

            while (contractedVerticesCount > 2)
            {
                var edgeToContract = contractedEdges[_random.Next(0, contractedEdges.Count)];

                if (contracted.ContainsKey(edgeToContract.from))
                {
                    contracted[edgeToContract.from].Add(edgeToContract.to);
                }
                else
                {
                    contracted.Add(edgeToContract.from, new List<string> { edgeToContract.to });
                }

                if (contracted.ContainsKey(edgeToContract.to))
                {
                    contracted[edgeToContract.from].AddRange(contracted[edgeToContract.to]);
                    contracted.Remove(edgeToContract.to);
                }

                var newEdges = new List<(string from, string to)>();
                foreach (var edge in contractedEdges)
                {
                    if (edge.to == edgeToContract.to)
                    {
                        newEdges.Add((edge.from, edgeToContract.from));
                    }
                    else if (edge.from == edgeToContract.to)
                    {
                        newEdges.Add((edgeToContract.from, edge.to));
                    }
                    else
                    {
                        newEdges.Add(edge);
                    }
                }

                contractedEdges = newEdges.Where(x => x.from != x.to).ToList();
                contractedVerticesCount--;
            }

            var counts = contracted.Select(x => x.Value.Count + 1).ToList();
            return (contractedEdges.Count(), counts.First(), counts.Last());
        }
    }

    public class Day25 : ISolution
    {
        public string PartOne(IEnumerable<string> input)
        {
            var edges = from row in input
                        select row.Replace(":", "").Split(" ") into split
                        from to in split.Skip(1)
                        let fr = split[0]
                        select (fr, to);

            var graph = new Graph(edges.ToList(), new Random());

            var minCut = int.MaxValue;
            int count1 = 0;
            int count2 = 0;
            while (minCut != 3)
            {
                (minCut, count1, count2) = graph.Cut();
            }

            return (count1 * count2).ToString();
        }

        public string PartTwo(IEnumerable<string> input)
        {
            return "Completed!";
        }

        public int Day => 25;
    }

    public interface ISolution
    {
        string PartOne(IEnumerable<string> input);

        string PartTwo(IEnumerable<string> input);

        int Day { get; }

        string[] Input()
        {
            var filePath = @"C:\Users\Adrián Bailador\Desktop\Day25.txt";
            return File.ReadAllLines(filePath);
        }
    }

    static class Program
    {
        static async Task Main(string[] args)
        {
            await RunSolutionForDay(25);
        }

        static async Task RunSolutionForDay(int? day)
        {
            var solutions = GetSolutionsToRun(day);

            Console.WriteLine($"Advent Of Code");

            foreach (var solution in solutions)
            {
                Console.WriteLine($"Day {solution.Day}");

                RunSolutionPart(() => solution.PartOne(solution.Input()), 1);
                RunSolutionPart(() => solution.PartTwo(solution.Input()), 2);
            }
        }

        static IEnumerable<ISolution> GetSolutionsToRun(int? day)
        {
            List<ISolution> solutions = new();
            if (day.HasValue)
            {
                var sol = Solutions().SingleOrDefault(x => x?.Day == day);
                if (sol != null)
                {
                    solutions.Add(sol);
                }
            }
            else
            {
                solutions.AddRange(Solutions()!);
            }

            return solutions;
        }

        static void RunSolutionPart(Func<string> solutionFunc, int part)
        {
            try
            {
                var timer = new Stopwatch();
                timer.Start();
                var answer = solutionFunc();
                timer.Stop();

                PrintSolution(part, answer, timer.ElapsedMilliseconds);
            }
            catch (NotImplementedException)
            {
                PrintError(part);
            }
        }

        static void PrintSolution(int part, string answer, long timeInMillis)
        {
            Console.Write("  \u2714");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($" - Part {part}: {answer} ");
            Console.ResetColor();

            var icon = timeInMillis > 50 ? "\u231B " : "";

            var timeColor = timeInMillis > 50
                ? timeInMillis > 1000
                    ? ConsoleColor.Red
                    : ConsoleColor.Yellow
                : ConsoleColor.Green;

            Console.Write($"[{icon}{timeInMillis}ms] {Environment.NewLine}");
            Console.ForegroundColor = timeColor;
            Console.ResetColor();
        }

        static void PrintError(int part)
        {
            Console.Write("  \u2718");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($" - Part {part} has not been solved{Environment.NewLine}");
            Console.ResetColor();
        }

        static IEnumerable<ISolution?> Solutions()
        {
            var type = typeof(ISolution);

            return Assembly.GetExecutingAssembly().DefinedTypes
                .Where(x => x.ImplementedInterfaces.Contains(type))
                .Select(impl => (ISolution)Activator.CreateInstance(impl)!)
                .Where(x => x.Day != 0)
                .OrderBy(sol => sol?.Day);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        var file = ReadLines(@"C:\Users\Adrián Bailador\Desktop\Day11.txt");
        // Part 1
        var resultPart1 = Part1Solver(file);
        Console.WriteLine($"Parte 1: {resultPart1}");

        // Part 2
        var resultPart2 = Part2Solver(file);
        Console.WriteLine($"Parte 2: {resultPart2}");
    }

    static string[] ReadLines(string filePath)
    {
        return System.IO.File.ReadAllLines(filePath);
    }

    static string Part1Solver(string[] lines)
    {
        var dic = new Dictionary<int, int[]>();
        var startIndex = 0;
        var matrix = new List<List<char>>();

        foreach (var line in lines)
        {
            matrix.Add(line.ToCharArray().ToList());
        }

        for (int y = 0; y < matrix.Count; y++)
        {
            if (matrix[y].All(x => x == '.'))
            {
                matrix.Insert(y, new string('.', matrix[y].Count).ToList());
                y++;
            }
        }

        for (int x = 0; x < matrix[0].Count; x++)
        {
            var yList = new List<char>();
            for (int y = 0; y < matrix.Count; y++)
            {
                yList.Add(matrix[y][x]);
            }

            if (yList.All(y => y == '.'))
            {
                for (int y1 = 0; y1 < matrix.Count; y1++)
                {
                    matrix[y1].Insert(x, '.');
                }
                x++;
            }
        }

        for (int y = 0; y < matrix.Count; y++)
        {
            for (int x = 0; x < matrix[y].Count; x++)
            {
                if (matrix[y][x] == '#')
                {
                    dic.Add(startIndex, new int[] { x, y });
                    startIndex++;
                }
            }
        }

        var result = 0;

        for (int i = 0; i < dic.Count; i++)
        {
            for (int j = i + 1; j < dic.Count; j++)
            {
                result += Math.Abs(dic[i][0] - dic[j][0]) + Math.Abs(dic[i][1] - dic[j][1]);
            }
        }

        return result.ToString();
    }

    static string Part2Solver(string[] lines)
    {
        var dic = new Dictionary<int, int[]>();
        var startIndex = 0;
        var matrix = new List<List<char>>();

        foreach (var line in lines)
        {
            matrix.Add(line.ToCharArray().ToList());
        }

        for (int y = 0; y < matrix.Count; y++)
        {
            for (int x = 0; x < matrix[y].Count; x++)
            {
                if (matrix[y][x] == '#')
                {
                    dic.Add(startIndex, new int[] { x, y });
                    startIndex++;
                }
            }
        }

        long result = 0;

        for (int i = 0; i < dic.Count; i++)
        {
            for (int j = i + 1; j < dic.Count; j++)
            {
                if (dic[i][0] < dic[j][0])
                {
                    for (int kx = dic[i][0] + 1; kx <= dic[j][0]; kx++)
                    {
                        var yList = new List<char>();

                        for (int y = 0; y < matrix.Count; y++)
                        {
                            yList.Add(matrix[y][kx]);
                        }

                        if (yList.All(y => y == '.'))
                        {
                            result += 1000000;
                        }
                        else
                        {
                            result++;
                        }
                    }
                }
                else
                {
                    for (int kx = dic[i][0] - 1; kx >= dic[j][0]; kx--)
                    {
                        var yList = new List<char>();

                        for (int y = 0; y < matrix.Count; y++)
                        {
                            yList.Add(matrix[y][kx]);
                        }

                        if (yList.All(y => y == '.'))
                        {
                            result += 1000000;
                        }
                        else
                        {
                            result++;
                        }
                    }
                }

                for (int ky = dic[i][1] + 1; ky <= dic[j][1]; ky++)
                {
                    if (matrix[ky].All(x => x != '#'))
                    {
                        result += 1000000;
                    }
                    else
                    {
                        result++;
                    }
                }
            }
        }

        return result.ToString();
    }
}

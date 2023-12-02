using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

class Trebuchet
{
    static void Main()
    {
        string content = File.ReadAllText("C:\\Users\\Adrián Bailador\\Desktop\\Day1.txt");
        Console.WriteLine("Day 1! task 1");
        Console.WriteLine(TaskOne(content));
        Console.WriteLine("Day 1! task 2");
        Console.WriteLine(TaskTwo(content));
    }

    static string TaskOne(string content)
    {
        var lines = content.Split('\n');
        int totalNumber = 0;

        foreach (var line in lines)
        {
            var numberCharacters = line.Where(char.IsDigit).ToList();
            if (numberCharacters.Count > 1)
            {
                var stringNumber = numberCharacters.First().ToString() + numberCharacters.Last().ToString();
                totalNumber += int.Parse(stringNumber);
            }
        }

        return totalNumber.ToString();
    }

    static string TaskTwo(string content)
    {
        var lines = content.Split('\n');
        int totalNumber = 0;
        var numbersAsText = new[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
        var numbersList = new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        foreach (var line in lines)
        {
            var firstNumber = FindFirst(numbersAsText, numbersList, line);
            var lastNumber = FindLast(numbersAsText, numbersList, line);
            var stringNumber = $"{firstNumber}{lastNumber}";
            totalNumber += int.Parse(stringNumber);
        }

        return totalNumber.ToString();
    }

    static int FindFirst(string[] numbersAsText, string[] numbersList, string line)
    {
        int lowestIteration = line.Length;
        string currentUsedValue = "";

        foreach (var currentNumber in numbersAsText.Concat(numbersList))
        {
            var firstIndex = line.IndexOf(currentNumber);
            if (firstIndex != -1 && firstIndex <= lowestIteration)
            {
                lowestIteration = firstIndex;
                currentUsedValue = currentNumber;
            }
        }

        return ConvertToNumber(currentUsedValue);
    }

    static int FindLast(string[] numbersAsText, string[] numbersList, string line)
    {
        int highestIteration = 0;
        string currentUsedValue = "";

        foreach (var currentNumber in numbersAsText.Concat(numbersList))
        {
            var lastIndex = line.LastIndexOf(currentNumber);
            if (lastIndex != -1 && lastIndex >= highestIteration)
            {
                highestIteration = lastIndex;
                currentUsedValue = currentNumber;
            }
        }

        return ConvertToNumber(currentUsedValue);
    }

    static int ConvertToNumber(string number)
    {
        return number switch
        {
            "one" => 1,
            "two" => 2,
            "three" => 3,
            "four" => 4,
            "five" => 5,
            "six" => 6,
            "seven" => 7,
            "eight" => 8,
            "nine" => 9,
            "1" => 1,
            "2" => 2,
            "3" => 3,
            "4" => 4,
            "5" => 5,
            "6" => 6,
            "7" => 7,
            "8" => 8,
            "9" => 9,
            _ => throw new Exception($"ERROR {number}")
        };
    }
}

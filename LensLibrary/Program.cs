using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

internal class Lens
{
    public string Label { get; set; } = string.Empty;
    public long FocalStrength { get; set; }
}

internal class Box : List<Lens>
{
    public byte Id { get; set; }

    public long FocusingPower
    {
        get
        {
            long result = 0;
            int idx = 1;
            foreach (Lens l in this)
            {
                result += idx++ * l.FocalStrength;
            }
            return result * (((long)Id) + 1);
        }
    }
}

internal class Program
{
    public static readonly char[] OperationChars = { '=', '-' };

    private static void Main(string[] args)
    {
        string path = @"C:\\Users\\Adrián Bailador\\Desktop\\Day15.txt";
        // Part 1
        Console.WriteLine("Part 1: " + File.ReadAllText(path).Split(',').Sum(CalculateValue));

        // Part 2
        Dictionary<byte, Box> boxes = new Dictionary<byte, Box>();
        foreach (string instruction in File.ReadAllText(path).Split(','))
        {
            byte boxNr = 0;
            int indexOfOperation = instruction.IndexOfAny(OperationChars);
            foreach (char c in instruction.Take(indexOfOperation))
            {
                boxNr += (byte)c;
                boxNr *= 17;
            }
            Box box = boxes.TryGetValue(boxNr, out Box? receivedBox) ? receivedBox : boxes[boxNr] = new Box() { Id = boxNr };
            Lens? lens;
            int lensIdx = -1;
            switch (instruction[indexOfOperation])
            {
                case '=':
                    lens = box.FirstOrDefault(l => MemoryExtensions.Equals(instruction.AsSpan(0, indexOfOperation), l.Label, StringComparison.Ordinal));
                    if (lens != null)
                    {
                        lens.FocalStrength = long.Parse(instruction.AsSpan(indexOfOperation + 1));
                    }
                    else
                    {
                        box.Add(new Lens() { Label = string.Join("", instruction.Take(indexOfOperation)), FocalStrength = long.Parse(instruction.AsSpan(indexOfOperation + 1)) });
                    }
                    break;
                case '-':
                    lensIdx = box.FindIndex(l => MemoryExtensions.Equals(instruction.AsSpan(0, indexOfOperation), l.Label, StringComparison.Ordinal));
                    if (lensIdx != -1)
                    {
                        box.RemoveAt(lensIdx);
                    }
                    break;
            }
        }
        Console.WriteLine("Part 2: " + boxes.Values.Sum(b => b.FocusingPower));
    }

    static int CalculateValue(string arg)
    {
        byte result = 0;
        foreach (char c in arg)
        {
            result += (byte)c;
            result *= 17;
        }
        return result;
    }
}

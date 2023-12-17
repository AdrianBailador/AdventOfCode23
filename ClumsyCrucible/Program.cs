using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var map = File.ReadAllLines(@"C:\Users\Adrián Bailador\Desktop\Day17.txt")
              .Select(s => s.Select(c => int.Parse(c.ToString())).ToArray())
              .ToArray();

var queue = new PriorityQueue<Path, int>();
var visited = new HashSet<string>();

queue.Enqueue(new Path(new Position(0, 0), Direction.Right, 0), 0);

var totalHeat = 0;

while (queue.Count > 0)
{
    var path = queue.Dequeue();

    if (path.Position.Row == map.Length - 1 && path.Position.Col == map[0].Length - 1)
    {
        totalHeat = path.Heat;
        break;
    }

    if (path.Distance < 3)
    {
        TryMove(path, path.Direction);
    }

    TryMove(path, path.Direction.TurnLeft());
    TryMove(path, path.Direction.TurnRight());
}

Console.WriteLine($"Part 1: {totalHeat}");

queue.Clear();
queue.Enqueue(new Path(new Position(0, 0), Direction.Right, 0), 0);
queue.Enqueue(new Path(new Position(0, 0), Direction.Down, 0), 0);
totalHeat = 0;
visited.Clear();

while (queue.Count > 0)
{
    var path = queue.Dequeue();
    if (path.Position.Row == map.Length - 1 && path.Position.Col == map[0].Length - 1)
    {
        totalHeat = path.Heat;
        break;
    }

    if (path.Distance < 10)
    {
        TryMove(path, path.Direction);
    }

    if (path.Distance >= 4)
    {
        TryMove(path, path.Direction.TurnLeft());
        TryMove(path, path.Direction.TurnRight());
    }
}

Console.WriteLine($"Part 2: {totalHeat}");
return;

void TryMove(Path path, Direction direction)
{
    var candidate = new Path(path.Position.Move(direction), direction, direction == path.Direction ? path.Distance + 1 : 1);

    if (candidate.Position.Row < 0 || candidate.Position.Row >= map.Length ||
        candidate.Position.Col < 0 || candidate.Position.Col >= map[0].Length)
    {
        return;
    }

    var key = $"{candidate.Position.Row},{candidate.Position.Col},{candidate.Direction.Row},{candidate.Direction.Col},{candidate.Distance}";
    if (visited.Contains(key))
    {
        return;
    }

    visited.Add(key);

    candidate.Heat = path.Heat + map[candidate.Position.Row][candidate.Position.Col];
    queue.Enqueue(candidate, candidate.Heat);
}

internal class Path
{
    public readonly Position Position;
    public readonly Direction Direction;
    public readonly int Distance;
    public int Heat { get; set; }

    public Path(Position position, Direction direction, int distance)
    {
        Position = position;
        Direction = direction;
        Distance = distance;
    }
}

internal class Direction
{
    public readonly int Row;
    public readonly int Col;

    public Direction(int row, int col)
    {
        Row = row;
        Col = col;
    }

    public Direction TurnLeft()
    {
        return new Direction(-Col, Row);
    }

    public Direction TurnRight()
    {
        return new Direction(Col, -Row);
    }

    public static Direction Up = new(-1, 0);
    public static Direction Down = new(1, 0);
    public static Direction Left = new(0, -1);
    public static Direction Right = new(0, 1);
}

internal class Position
{
    public readonly int Row;
    public readonly int Col;

    public Position(int row, int col)
    {
        Row = row;
        Col = col;
    }

    public Position Move(Direction dir)
    {
        return new Position(Row + dir.Row, Col + dir.Col);
    }
}

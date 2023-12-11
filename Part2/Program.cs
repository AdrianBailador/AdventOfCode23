using System;
using System.IO;

class Program
{
    static void Main()
    {
        // Ruta del archivo
        string filePath = @"C:\Users\Adrián Bailador\Desktop\Day10.txt";

        // Leer todas las líneas del archivo
        string[] lines = File.ReadAllLines(filePath);

        var answer = 0;
        var sketch = new Pipe[lines[0].Length, lines.Length];
        Pipe? startingPoint = null;
        for (var y = 0; y < lines.Length; y++)
        {
            for (var x = 0; x < lines[0].Length; x++)
            {
                var pipe = new Pipe(lines[y][x], x, y);
                sketch[x, y] = pipe;
                if (pipe.symbol.Equals('S'))
                    startingPoint = pipe;
            }
        }

        if (startingPoint == null)
            throw new Exception("No starting point found");

        // iteratively remove incorrectly connected pipes until we end up with the main loop only
        var incorrectPipesFound = false;
        do
        {
            incorrectPipesFound = false;

            // find incorrectly connected pipes (but don't remove them yet as to not influence connection count on adjacent pipes)
            for (var y = 0; y < sketch.GetLength(1); y++)
            {
                for (var x = 0; x < sketch.GetLength(0); x++)
                {
                    if (sketch[x, y].symbol.Equals('S') || sketch[x, y].symbol.Equals('.'))
                        continue;

                    var connections = 0;
                    if (y > 0 && sketch[x, y].north && sketch[x, y - 1].south)
                        connections++;
                    if (x > 0 && sketch[x, y].west && sketch[x - 1, y].east)
                        connections++;
                    if (y < sketch.GetLength(1) - 1 && sketch[x, y].south && sketch[x, y + 1].north)
                        connections++;
                    if (x < sketch.GetLength(0) - 1 && sketch[x, y].east && sketch[x + 1, y].west)
                        connections++;

                    if (connections != 2)
                    {
                        sketch[x, y].ScheduleToRemove();
                        incorrectPipesFound = true;
                    }
                }
            }
            // remove all unconnected pipes marked as scheduled to remove
            for (var y = 0; y < sketch.GetLength(1); y++)
                for (var x = 0; x < sketch.GetLength(0); x++)
                    sketch[x, y].CleanUp();

        } while (incorrectPipesFound);

        // now we can start following the paths
        var paths = new Position[] {
            new Position(startingPoint.x, startingPoint.y),
            new Position(startingPoint.x, startingPoint.y)
        };

        // start paths in a different direction each
        var firstPathFound = false;
        if (startingPoint.y > 0 && sketch[startingPoint.x, startingPoint.y - 1].south)
        {
            paths[0].y--;
            paths[0].direction = Direction.North;
            firstPathFound = true;
        }
        if (startingPoint.x < sketch.GetLength(0) - 1 && sketch[startingPoint.x + 1, startingPoint.y].west)
        {
            var index = 0;
            if (firstPathFound)
                index++;
            else
                firstPathFound = true;
            paths[index].x++;
            paths[index].direction = Direction.East;
        }
        if (startingPoint.y < sketch.GetLength(1) - 1 && sketch[startingPoint.x, startingPoint.y + 1].north)
        {
            var index = 0;
            if (firstPathFound)
                index++;
            paths[index].y++;
            paths[index].direction = Direction.South;
        }
        if (startingPoint.x > 0 && sketch[startingPoint.x - 1, startingPoint.y].east)
        {
            paths[1].x--;
            paths[1].direction = Direction.West;
        }

        // follow each path until we're on the same position
        // this loop steps in an 'open' direction and closes it right away so it can't walk back in the wrong direction
        // also we should check if we don't walk towards the starting point again, as both paths should move away from that
        while (true)
        {
            for (var i = 0; i < paths.Length; i++)
            {
                // a bit tedious.. but let's mark the inside of the loop while walking along the path
                // either going clockwise for path one or counterclockwise for path two
                switch (sketch[paths[i].x, paths[i].y].symbol)
                {
                    case 'F':
                        if ((i == 1 && paths[i].direction == Direction.North) ||
                            (i == 0 && paths[i].direction == Direction.West))
                        {
                            sketch[paths[i].x, paths[i].y].northInside = true;
                            sketch[paths[i].x, paths[i].y].westInside = true;
                        }
                        break;
                    case '-':
                        switch (paths[i].direction)
                        {
                            case Direction.North:
                                throw new Exception("Unexpected direction");
                            case Direction.East:
                                if (i == 0)
                                    sketch[paths[i].x, paths[i].y].southInside = true;
                                else
                                    sketch[paths[i].x, paths[i].y].northInside = true;
                                break;
                            case Direction.South:
                                throw new Exception("Unexpected direction");
                            case Direction.West:
                                if (i == 0)
                                    sketch[paths[i].x, paths[i].y].northInside = true;
                                else
                                    sketch[paths[i].x, paths[i].y].southInside = true;
                                break;

                        }
                        break;
                    case '7':
                        if ((i == 0 && paths[i].direction == Direction.North) ||
                            (i == 1 && paths[i].direction == Direction.East))
                        {
                            sketch[paths[i].x, paths[i].y].northInside = true;
                            sketch[paths[i].x, paths[i].y].eastInside = true;
                        }
                        break;
                    case '|':
                        switch (paths[i].direction)
                        {
                            case Direction.North:
                                if (i == 0)
                                    sketch[paths[i].x, paths[i].y].eastInside = true;
                                else
                                    sketch[paths[i].x, paths[i].y].westInside = true;
                                break;
                            case Direction.East:
                                throw new Exception("Unexpected direction");
                            case Direction.South:
                                if (i == 0)
                                    sketch[paths[i].x, paths[i].y].westInside = true;
                                else
                                    sketch[paths[i].x, paths[i].y].eastInside = true;
                                break;
                            case Direction.West:
                                throw new Exception("Unexpected direction");

                        }
                        break;
                    case 'J':
                        if ((i == 0 && paths[i].direction == Direction.East) ||
                            (i == 1 && paths[i].direction == Direction.South))
                        {
                            sketch[paths[i].x, paths[i].y].eastInside = true;
                            sketch[paths[i].x, paths[i].y].southInside = true;
                        }
                        break;
                    case 'L':
                        if ((i == 0 && paths[i].direction == Direction.South) ||
                            (i == 1 && paths[i].direction == Direction.West))
                        {
                            sketch[paths[i].x, paths[i].y].southInside = true;
                            sketch[paths[i].x, paths[i].y].westInside = true;
                        }
                        break;
                }

                // advance along the path
                if (sketch[paths[i].x, paths[i].y].north && !sketch[paths[i].x, paths[i].y - 1].symbol.Equals('S'))
                {
                    paths[i].y--;
                    paths[i].direction = Direction.North;
                    sketch[paths[i].x, paths[i].y].south = false;
                }
                else if (sketch[paths[i].x, paths[i].y].east && !sketch[paths[i].x + 1, paths[i].y].symbol.Equals('S'))
                {
                    paths[i].x++;
                    paths[i].direction = Direction.East;
                    sketch[paths[i].x, paths[i].y].west = false;
                }
                else if (sketch[paths[i].x, paths[i].y].south && !sketch[paths[i].x, paths[i].y + 1].symbol.Equals('S'))
                {
                    paths[i].y++;
                    paths[i].direction = Direction.South;
                    sketch[paths[i].x, paths[i].y].north = false;
                }
                else if (sketch[paths[i].x, paths[i].y].west && !sketch[paths[i].x - 1, paths[i].y].symbol.Equals('S'))
                {
                    paths[i].x--;
                    paths[i].direction = Direction.West;
                    sketch[paths[i].x, paths[i].y].east = false;
                }
            }

            if (paths[0].x == paths[1].x && paths[0].y == paths[1].y)
                break;
        }

        // check all empty tiles for being inside the loop by looping in each direction until
        // we either hit an inside boundary, or any other boundary (outside or edge of sketch)
        for (var y = 0; y < sketch.GetLength(1); y++)
        {
            for (var x = 0; x < sketch.GetLength(0); x++)
            {
                if (sketch[x, y].symbol.Equals('.'))
                {
                    var leftBoundary = false;
                    for (var x1 = x; x1 >= 0; x1--)
                    {
                        if (!sketch[x1, y].symbol.Equals('.'))
                        {
                            if (sketch[x1, y].eastInside || sketch[x1, y].symbol.Equals('S'))
                                leftBoundary = true;
                            break;
                        }
                    }
                    var rightBoundary = false;
                    for (var x1 = x; x1 < sketch.GetLength(0); x1++)
                    {
                        if (!sketch[x1, y].symbol.Equals('.'))
                        {
                            if (sketch[x1, y].westInside || sketch[x1, y].symbol.Equals('S'))
                                rightBoundary = true;
                            break;
                        }
                    }
                    var upperBoundary = false;
                    for (var y1 = y; y1 >= 0; y1--)
                    {
                        if (!sketch[x, y1].symbol.Equals('.'))
                        {
                            if (sketch[x, y1].southInside || sketch[x, y1].symbol.Equals('S'))
                                upperBoundary = true;
                            break;
                        }
                    }
                    var lowerBoundary = false;
                    for (var y1 = y; y1 < sketch.GetLength(1); y1++)
                    {
                        if (!sketch[x, y1].symbol.Equals('.'))
                        {
                            if (sketch[x, y1].northInside || sketch[x, y1].symbol.Equals('S'))
                                lowerBoundary = true;
                            break;
                        }
                    }
                    if (leftBoundary && rightBoundary && upperBoundary && lowerBoundary)
                        sketch[x, y].inside = true;
                }
            }
        }

        // count tiles that are marked to be on the inside of the loop
        for (var y = 0; y < sketch.GetLength(1); y++)
        {
            for (var x = 0; x < sketch.GetLength(0); x++)
            {
                if (sketch[x, y].inside)
                    answer++;
            }
        }

        Console.WriteLine(answer);
    }
}

// Asegúrate de tener las siguientes clases definidas

class Position
{
    public int x;
    public int y;

    public Direction direction;

    public Position(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

enum Direction
{
    North,
    East,
    South,
    West
}

class Pipe
{
    public char symbol;
    public int x;
    public int y;

    // connecting directions
    public bool north;
    public bool east;
    public bool south;
    public bool west;

    public bool scheduleToRemove;

    // inside sides of this pipe relative to the paths direction
    public bool northInside;
    public bool eastInside;
    public bool southInside;
    public bool westInside;

    public bool inside;

    public Pipe(char symbol, int x, int y)
    {
        this.symbol = symbol;
        this.x = x;
        this.y = y;
        SetConnections();
    }

    public void ScheduleToRemove()
    {
        scheduleToRemove = true;
    }

    public void CleanUp()
    {
        if (scheduleToRemove)
        {
            symbol = '.';
            north = false;
            east = false;
            south = false;
            west = false;
        }
    }

    private void SetConnections()
    {
        switch (symbol)
        {
            case 'S':
                north = true;
                east = true;
                south = true;
                west = true;
                break;
            case 'F':
                north = false;
                east = true;
                south = true;
                west = false;
                break;
            case '-':
                north = false;
                east = true;
                south = false;
                west = true;
                break;
            case '7':
                north = false;
                east = false;
                south = true;
                west = true;
                break;
            case '|':
                north = true;
                east = false;
                south = true;
                west = false;
                break;
            case 'J':
                north = true;
                east = false;
                south = false;
                west = true;
                break;
            case 'L':
                north = true;
                east = true;
                south = false;
                west = false;
                break;
        }
    }
}
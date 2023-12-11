using System;
using System.IO;
using System.Linq;

class Program
{
    static void Main()
    {
        try
        {
            // Obtener la ruta completa del archivo
            string filePath = @"C:\Users\Adrián Bailador\Desktop\Day10.txt";

            // Leer líneas del archivo
            var lines = File.ReadAllLines(filePath);

            // Inicializar variables
            var answer = 1;
            var sketch = new Pipe[lines[0].Length, lines.Length];
            Pipe? startingPoint = null;

            // Construir el esquema de tuberías y encontrar el punto de inicio
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

            // Iterativamente eliminar tuberías incorrectamente conectadas
            var incorrectPipesFound = false;
            do
            {
                incorrectPipesFound = false;

                // Encontrar tuberías incorrectamente conectadas
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

                // Eliminar tuberías marcadas para eliminación
                for (var y = 0; y < sketch.GetLength(1); y++)
                    for (var x = 0; x < sketch.GetLength(0); x++)
                        sketch[x, y].CleanUp();

            } while (incorrectPipesFound);

            // Iniciar los caminos en direcciones diferentes
            var paths = new Position[]
            {
                new Position(startingPoint.x, startingPoint.y),
                new Position(startingPoint.x, startingPoint.y)
            };

            // Encontrar la primera dirección válida
            var firstPathFound = false;
            if (startingPoint.y > 0 && sketch[startingPoint.x, startingPoint.y - 1].south)
            {
                paths[0].y--;
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
            }
            if (startingPoint.y < sketch.GetLength(1) - 1 && sketch[startingPoint.x, startingPoint.y + 1].north)
            {
                var index = 0;
                if (firstPathFound)
                    index++;
                paths[index].y++;
            }
            if (startingPoint.x > 0 && sketch[startingPoint.x - 1, startingPoint.y].east)
            {
                paths[1].x--;
            }

            // Seguir cada camino hasta que estén en la misma posición
            while (true)
            {
                for (var i = 0; i < paths.Length; i++)
                {
                    if (sketch[paths[i].x, paths[i].y].north && !sketch[paths[i].x, paths[i].y - 1].symbol.Equals('S'))
                    {
                        paths[i].y--;
                        sketch[paths[i].x, paths[i].y].south = false;
                    }
                    else if (sketch[paths[i].x, paths[i].y].east && !sketch[paths[i].x + 1, paths[i].y].symbol.Equals('S'))
                    {
                        paths[i].x++;
                        sketch[paths[i].x, paths[i].y].west = false;
                    }
                    else if (sketch[paths[i].x, paths[i].y].south && !sketch[paths[i].x, paths[i].y + 1].symbol.Equals('S'))
                    {
                        paths[i].y++;
                        sketch[paths[i].x, paths[i].y].north = false;
                    }
                    else if (sketch[paths[i].x, paths[i].y].west && !sketch[paths[i].x - 1, paths[i].y].symbol.Equals('S'))
                    {
                        paths[i].x--;
                        sketch[paths[i].x, paths[i].y].east = false;
                    }
                }

                answer++;

                if (paths[0].x == paths[1].x && paths[0].y == paths[1].y)
                    break;
            }

            // Mostrar el resultado
            Console.WriteLine(answer);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocurrió un error: {ex.Message}");
        }
    }
}

class Position
{
    public int x;
    public int y;

    public Position(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

class Pipe
{
    public char symbol;
    public int x;
    public int y;

    public bool north;
    public bool east;
    public bool south;
    public bool west;

    public bool scheduleToRemove;

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




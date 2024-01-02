using System;
using System.IO;
using System.Text.RegularExpressions;

namespace AdventOfCode.Y2023.Day24
{
    record Vec2(decimal x0, decimal x1);
    record Vec3(decimal x0, decimal x1, decimal x2);
    record Particle2(Vec2 pos, Vec2 vel);
    record Particle3(Vec3 pos, Vec3 vel);


    class Solution : Solver
    {
        public object PartOne(string input)
        {
            var particles = Project(ParseParticles(input), v => (v.x0, v.x1));

            var inRange = (decimal d) => 2e14m <= d && d <= 4e14m;

            var inFuture = (Particle2 p, Vec2 pos) =>
                Math.Sign(pos.x0 - p.pos.x0) == Math.Sign(p.vel.x0);

            var res = 0;
            for (var i = 0; i < particles.Length; i++)
            {
                for (var j = i + 1; j < particles.Length; j++)
                {
                    var pos = Intersection(particles[i], particles[j]);
                    if (pos != null &&
                        inRange(pos.x0) &&
                        inRange(pos.x1) &&
                        inFuture(particles[i], pos) &&
                        inFuture(particles[j], pos))
                    {
                        res++;
                    }
                }
            }
            return res;
        }

        public object PartTwo(string input)
        {
            var particles = ParseParticles(input);
            var stoneXY = Solve2D(Project(particles, vec => (vec.x0, vec.x1)));
            var stoneXZ = Solve2D(Project(particles, vec => (vec.x0, vec.x2)));
            return Math.Round(stoneXY.x0 + stoneXY.x1 + stoneXZ.x1);
        }

        Vec2 Solve2D(Particle2[] particles)
        {
            var translateV = (Particle2 p, Vec2 vel) =>
                new Particle2(p.pos, new Vec2(p.vel.x0 - vel.x0, p.vel.x1 - vel.x1));

            var s = 500;
            for (var v1 = -s; v1 < s; v1++)
            {
                for (var v2 = -s; v2 < s; v2++)
                {
                    var vel = new Vec2(v1, v2);
                    var stone = Intersection(
                        translateV(particles[0], vel),
                        translateV(particles[1], vel)
                    );

                    if (particles.All(p => Hits(translateV(p, vel), stone)))
                    {
                        return stone;
                    }
                }
            }
            throw new Exception("No solution found.");
        }

        bool Hits(Particle2 p, Vec2 pos)
        {
            var d = (pos.x0 - p.pos.x0) * p.vel.x1 - (pos.x1 - p.pos.x1) * p.vel.x0;
            return Math.Abs(d) < (decimal)0.0001;
        }

        Vec2 Intersection(Particle2 p1, Particle2 p2)
        {
            var determinant = p1.vel.x0 * p2.vel.x1 - p1.vel.x1 * p2.vel.x0;
            if (determinant == 0)
            {
                return null;
            }

            var b0 = p1.vel.x0 * p1.pos.x1 - p1.vel.x1 * p1.pos.x0;
            var b1 = p2.vel.x0 * p2.pos.x1 - p2.vel.x1 * p2.pos.x0;

            return new(
                 (p2.vel.x0 * b0 - p1.vel.x0 * b1) / determinant,
                 (p2.vel.x1 * b0 - p1.vel.x1 * b1) / determinant
             );
        }

        Particle3[] ParseParticles(string input) => [..
            from line in input.Split('\n')
            let v = ParseNum(line)
            select new Particle3(new(v[0], v[1], v[2]), new(v[3], v[4], v[5]))
        ];

        decimal[] ParseNum(string l) => [..
            from m in Regex.Matches(l, @"-?\d+") select decimal.Parse(m.Value)
        ];

        Particle2[] Project(Particle3[] ps, Func<Vec3, (decimal, decimal)> proj) => [..
            from p in ps
            select new Particle2(
                new Vec2(proj(p.pos).Item1, proj(p.pos).Item2),
                new Vec2(proj(p.vel).Item1, proj(p.vel).Item2)
            )
        ];
    }

    class Solver
    {
        // Define any common functionality if needed.
    }

    class Program
    {
        static void Main()
        {
            try
            {
                string filePath = @"C:\Users\Adrián Bailador\Desktop\Day24.txt";
                string fileContent = File.ReadAllText(filePath);

                Solution solver = new Solution();
                var resultPartOne = solver.PartOne(fileContent);
                var resultPartTwo = solver.PartTwo(fileContent);

                Console.WriteLine($"Part One Result: {resultPartOne}");
                Console.WriteLine($"Part Two Result: {resultPartTwo}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}

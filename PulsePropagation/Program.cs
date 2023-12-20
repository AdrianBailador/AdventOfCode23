using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    private static Dictionary<string, IModule> modules;

    static void Main()
    {
        var input = File.ReadAllLines(@"C:\Users\Adrián Bailador\Desktop\Day20.txt");

        // Initialize the dictionary of modules
        modules = new Dictionary<string, IModule>();

        // Process input lines and create corresponding modules
        foreach (var line in input)
        {
            var split = line.Split(" -> ");
            var name = split[0];
            var targets = split[1].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

            if (name == "broadcaster")
            {
                modules[name] = new Broadcaster(name, targets);
            }
            else if (name.StartsWith('%'))
            {
                modules[name[1..]] = new FlipFlop(name[1..], targets);
            }
            else if (name.StartsWith('&'))
            {
                modules[name[1..]] = new Conjunction(name[1..], targets);
            }
        }

        // Set inputs for Conjunction-type modules
        foreach (var module in modules.Values)
        {
            if (module is Conjunction conjunction)
            {
                var inputs = modules.Values.Where(m => m.Targets.ContainsKey(module.Name)).ToList();
                foreach (var inputModule in inputs)
                {
                    conjunction.Inputs[inputModule.Name] = PulseType.Low;
                }
            }
        }

        var lowPulses = 0;
        var highPulses = 0;
        var queue = new Queue<(IModule Module, PulseType PulseType)>();
        var currentPulse = PulseType.Low;
        var count = 0;

        // Find the module that sends pulses to "rx"
        var rxSource = modules.Values.First(m => m.Targets.ContainsKey("rx"));
        // Find modules that send pulses to the Conjunction that sends pulses to "rx"
        var rxSourceInputs = modules.Values.Where(m => m.Targets.ContainsKey(rxSource.Name)).ToList();
        var rxSourceInputCounts = new Dictionary<string, (int Count, bool Done)>();

        // Initialize counters for each input module
        foreach (var rxSourceModule in rxSourceInputs)
        {
            rxSourceInputCounts[rxSourceModule.Name] = (0, false);
        }

        while (true)
        {
            // Print the result for Part 1 after 1000 iterations
            if (count == 1000)
            {
                Console.WriteLine($"Part 1: {lowPulses * highPulses}");
            }

            count++;

            queue.Enqueue((modules["broadcaster"], currentPulse));

            if (currentPulse == PulseType.Low)
            {
                lowPulses++;
            }
            else
            {
                highPulses++;
            }

            // Check if all input modules have sent a high pulse to the Conjunction that sends pulses to "rx"
            if (rxSourceInputCounts.All(rxs => rxs.Value.Done))
            {
                // Calculate the Least Common Multiple (LCM) of counts to determine when all modules have sent high pulses
                Console.WriteLine($"Part 2: {Lcm(rxSourceInputCounts.Select(rxs => rxs.Value.Count).ToArray())}");
                break;
            }

            while (queue.TryDequeue(out var current))
            {
                // Process the current module and update pulses for target modules
                current.Module.Process(current.PulseType);

                foreach (var target in current.Module.Targets.Keys)
                {
                    var targetPulse = current.Module.Targets[target];

                    // Check if the target is "rx" and update corresponding counters
                    if (target == "rx")
                    {
                        if (current.Module is Conjunction conjunction)
                        {
                            var highInputs = conjunction.Inputs.Where(conjunctionInput => conjunctionInput.Value == PulseType.High);
                            foreach (var conjunctionInput in highInputs)
                            {
                                rxSourceInputCounts[conjunctionInput.Key] = (count, true);
                            }
                        }
                    }

                    // Update pulse counters
                    switch (targetPulse)
                    {
                        case PulseType.Low:
                            lowPulses++;
                            break;
                        case PulseType.High:
                            highPulses++;
                            break;
                        case PulseType.None:
                        default:
                            break;
                    }

                    // Enqueue the target module for processing its pulses if needed
                    if (targetPulse != PulseType.None)
                    {
                        if (!modules.ContainsKey(target))
                        {
                            continue;
                        }

                        if (modules[target] is Conjunction conjunction)
                        {
                            conjunction.Inputs[current.Module.Name] = targetPulse;
                        }

                        queue.Enqueue((modules[target], targetPulse));
                    }

                    // Change the current pulse if the queue is empty
                    if (queue.Count == 0)
                    {
                        currentPulse = current.PulseType == PulseType.Low ? PulseType.High : PulseType.Low;
                    }
                }
            }
        }
    }

    // Calculate the Least Common Multiple (LCM) of a collection of numbers
    static long Lcm(IEnumerable<int> numbers) => numbers.Select(Convert.ToInt64).Aggregate((a, b) => a * b / Gcd(a, b));

    // Calculate the Greatest Common Divisor (GCD) of two numbers
    static long Gcd(long a, long b) => b == 0 ? a : Gcd(b, a % b);

    internal interface IModule
    {
        string Name { get; set; }
        Dictionary<string, PulseType> Targets { get; set; }
        void Process(PulseType pulse);
    }

    internal class Broadcaster : IModule
    {
        public Broadcaster(string name, List<string> targets)
        {
            Name = name;

            foreach (var target in targets)
            {
                Targets[target] = PulseType.Low;
            }
        }

        public string Name { get; set; }
        public Dictionary<string, PulseType> Targets { get; set; } = new();

        public virtual void Process(PulseType pulse)
        {
            foreach (var name in Targets.Keys)
            {
                Targets[name] = pulse;
            }
        }
    }

    internal class FlipFlop : Broadcaster
    {
        public bool PoweredOn { get; set; }

        public FlipFlop(string name, List<string> targets) : base(name, targets)
        {
        }

        public override void Process(PulseType pulse)
        {
            var output = PulseType.None;

            if (pulse == PulseType.Low)
            {
                output = PoweredOn ? PulseType.Low : PulseType.High;
                PoweredOn = !PoweredOn;
            }

            foreach (var targetName in Targets.Keys)
            {
                Targets[targetName] = output;
            }
        }
    }

    internal class Conjunction : Broadcaster
    {
        public Dictionary<string, PulseType> Inputs { get; set; } = new();

        public Conjunction(string name, List<string> targets) : base(name, targets)
        {
        }

        public override void Process(PulseType pulse)
        {
            foreach (var name in Targets.Keys)
            {
                Targets[name] = Inputs.Values.All(x => x == PulseType.High) ? PulseType.Low : PulseType.High;
            }
        }
    }

    internal enum PulseType
    {
        None,
        Low,
        High
    }
}

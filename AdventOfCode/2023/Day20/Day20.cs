using AdventOfCode.Shared;

namespace AdventOfCode._2023.Day20
{
    public class Day20 : Day
    {
        public Day20() : base(2023, 20, "Day20/input_2023_20.txt", "949764474", "", true)
        {

        }

        private Dictionary<string, Module> _modules;
        public override void Initialise()
        {
            _modules = InputLines
                .Select(l => new Module(l))
                .ToDictionary(m => m.Name, m => m);

            foreach (var module in _modules.Values)
            {
                foreach (var destination in module.Destinations)
                {
                    if (_modules.ContainsKey(destination))
                    {
                        var destinationModule = _modules[destination];
                        destinationModule.RegisterInput(module.Name);
                    }
                }
            }
        }

        public override string Part1()
        {
            var result = PushButton(1000);
            return result.ToString();
        }

        public override string Part2()
        {
            return string.Empty;
        }

        private long PushButton(int pushCount)
        {
            long lowPulseCount = 0;
            long highPulseCount = 0;

            while (pushCount > 0)
            {
                pushCount -= 1;

                var pulseQueue = new Queue<Pulse>();
                pulseQueue.Enqueue(new Pulse
                {
                    Source = "button",
                    Destination = "broadcaster",
                    Type = PulseType.Low
                });

                while (pulseQueue.Any())
                {
                    var currentPulse = pulseQueue.Dequeue();
                    // TraceLine($"{currentPulse.Source} -{currentPulse.Type}-> {currentPulse.Destination}");
                    if (currentPulse.Type == PulseType.Low)
                    {
                        lowPulseCount += 1;
                    }
                    if (currentPulse.Type == PulseType.High)
                    {
                        highPulseCount += 1;
                    }

                    if (_modules.ContainsKey(currentPulse.Destination))
                    {
                        var module = _modules[currentPulse.Destination];
                        var resultingPulses = module.ProcessPulse(currentPulse);

                        foreach (var pulse in resultingPulses)
                        {
                            pulseQueue.Enqueue(pulse);
                        }
                    }
                }
            }

            return lowPulseCount * highPulseCount;
        }

        private class Module
        {
            public string Name { get; set; }
            public ModuleType Type { get; set; }
            public List<string> Destinations;

            public Module(string description)
            {
                var split = description.Split(" -> ");
                Destinations = split[1].Split(',', StringSplitOptions.TrimEntries).ToList();

                var name = split[0];
                if (name.StartsWith('%'))
                {
                    Name = name.Substring(1);
                    Type = ModuleType.FlipFlop;
                }
                else if (name.StartsWith('&'))
                {
                    Name = name.Substring(1);
                    Type = ModuleType.Conjunction;
                }
                else
                {
                    Name = name;
                    Type = ModuleType.Broadcast;
                }
            }

            public List<Pulse> ProcessPulse(Pulse pulse)
            {
                switch (Type)
                {
                    case ModuleType.Broadcast: return ProcessPulseBroadcast(pulse);
                    case ModuleType.FlipFlop: return ProcessPulseFlipFlop(pulse);
                    case ModuleType.Conjunction: return ProcessPulseConjunction(pulse);
                }

                throw new Exception("Unexpected Module Type");
            }

            private bool flipFlopOn = false;
            private List<Pulse> ProcessPulseFlipFlop(Pulse pulse)
            {
                if (pulse.Type == PulseType.High)
                {
                    return new List<Pulse>();
                }

                if (flipFlopOn)
                {
                    flipFlopOn = false;

                    return SendPulse(PulseType.Low);
                }

                flipFlopOn = true;

                return SendPulse(PulseType.High);
            }

            public void RegisterInput(string name)
            {
                _lastSignal.Add(name, PulseType.Low);
            }

            private Dictionary<string, PulseType> _lastSignal = new Dictionary<string, PulseType>();
            private List<Pulse> ProcessPulseConjunction(Pulse pulse)
            {
                _lastSignal[pulse.Source] = pulse.Type;

                if (_lastSignal.Values.All(p => p == PulseType.High))
                {
                    return SendPulse(PulseType.Low);
                }

                return SendPulse(PulseType.High);
            }

            private List<Pulse> ProcessPulseBroadcast(Pulse pulse)
            {
                return SendPulse(pulse.Type);
            }

            private List<Pulse> SendPulse(PulseType pulseType)
            {
                return Destinations
                    .Select(d => new Pulse
                    {
                        Source = Name,
                        Destination = d,
                        Type = pulseType
                    })
                    .ToList();
            }
        }

        private class Pulse
        {
            public string Source { get; set; }
            public string Destination { get; set; }
            public PulseType Type { get; set; }
        }

        private enum PulseType
        {
            Low,
            High
        }

        private enum ModuleType
        {
            Broadcast,
            FlipFlop,
            Conjunction
        }
    }
}

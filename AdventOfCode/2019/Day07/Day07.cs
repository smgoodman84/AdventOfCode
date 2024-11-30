using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdventOfCode._2019.Intcode;
using AdventOfCode.Shared;

namespace AdventOfCode._2019.Day07;

public class Day07 : Day
{
    public Day07() : base(2019, 7, "Day07/input_2019_07.txt", "116680", "89603079")
    {

    }

    public override string Part1()
    {
        return GetMaximumThrustSignal().ToString();
    }

    public override string Part2()
    {
        return GetMaximumFeedbackThrustSignal().GetAwaiter().GetResult().ToString();
    }

    public long GetMaximumThrustSignal(List<long> inputs = null)
    {
        if (inputs == null)
        {
            return GetMaximumThrustSignal(new List<long>());
        }

        if (inputs.Count == 5)
        {
            return GetThrustSignal(inputs.ToArray());
        }

        long max = 0;
        foreach (var i in Enumerable.Range(0, 5).Where(x => !inputs.Contains(x)))
        {
            var nextInput = inputs.ToList();
            nextInput.Add(i);

            var result = GetMaximumThrustSignal(nextInput);
            if (result > max)
            {
                max = result;
            }
        }

        return max;
    }

    private long GetThrustSignal(params long[] phaseSettingInputs)
    {
        var amplifiers = Enumerable.Range(1, 5)
            .Select(x => IntcodeMachine.Load(InputLines))
            .ToArray();

        var phaseSettings = new PreparedInput(phaseSettingInputs);

        IInput input = new PreparedInput(0);
        IOPipe pipe = null;
        foreach (var amplifier in amplifiers)
        {
            var amplifierInputs = new CombinedInput(phaseSettings, input);

            amplifier.SetInput(amplifierInputs);

            pipe = new IOPipe();
            amplifier.SetOutput(pipe);

            input = pipe;
        }

        foreach (var amplifier in amplifiers)
        {
            amplifier.Execute().Wait();
        }

        var result = pipe.ReadInput().Result;

        return result;
    }

    public async Task<long> GetMaximumFeedbackThrustSignal(List<long> inputs = null)
    {
        if (inputs == null)
        {
            return await GetMaximumFeedbackThrustSignal(new List<long>());
        }

        if (inputs.Count == 5)
        {
            return await GetFeedbackThrustSignal(inputs.ToArray());
        }

        long max = 0;
        foreach (var i in Enumerable.Range(5, 5).Where(x => !inputs.Contains(x)))
        {
            var nextInput = inputs.ToList();
            nextInput.Add(i);

            var result = await GetMaximumFeedbackThrustSignal(nextInput);
            if (result > max)
            {
                max = result;
            }
        }

        return max;
    }

    private async Task<long> GetFeedbackThrustSignal(params long[] phaseSettingInputs)
    {
        var amplifiers = phaseSettingInputs
            .Select(x => IntcodeMachine.Load(InputLines))
            .ToArray();

        var phaseSettings = new PreparedInput(phaseSettingInputs);

        var pipes = phaseSettingInputs
            .Select(InitialisedPipe)
            .ToArray();

        pipes[0].Output(0);

        var pipeIndex = 0;
        foreach (var amplifier in amplifiers)
        {
            amplifier.SetInput(pipes[pipeIndex]);
            pipeIndex += 1;
            if (pipeIndex >= phaseSettingInputs.Length)
            {
                pipeIndex = 0;
            }
            amplifier.SetOutput(pipes[pipeIndex]);
        }

        var tasks = amplifiers.Select(a => a.Execute());
        await Task.WhenAll(tasks);

        var result = await pipes[0].ReadInput();

        return result;
    }

    private static IOPipe InitialisedPipe(long initialisedValue)
    {
        var pipe = new IOPipe();
        pipe.Output(initialisedValue);
        return pipe;
    }
}
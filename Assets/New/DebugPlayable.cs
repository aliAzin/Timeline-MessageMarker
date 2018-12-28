using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class DebugPlayable : IEnumerable<(DebugPlayable Node, float Weight)>
{
    public Playable Playable { get; set; }
    public PlayableOutput PlayableOutput { get; set; }

    public static explicit operator DebugPlayable(Playable value) => new DebugPlayable
    {
        Playable = value,
    };

    public static explicit operator DebugPlayable(PlayableGraph value) => new DebugPlayable
    {
        Playable = value.GetRootPlayable(0),
        PlayableOutput = value.GetOutput(0),
    };

    public static explicit operator DebugPlayable(PlayableOutput value) => new DebugPlayable
    {
        PlayableOutput = value,
        Playable = value.GetSourcePlayable()
    };

    public Type Type => Playable.GetPlayableType();
    public int InputCount => Playable.GetInputCount();
    public int OutputCount => Playable.GetOutputCount();
    public bool CanChangeInputs => Playable.CanChangeInputs();
    public bool CanSetWeights => Playable.CanSetWeights();
    public bool CanDestroy => Playable.CanDestroy();
    public PlayState PlayState => Playable.GetPlayState();
    public bool IsValid => Playable.IsValid();
    public Type OutputType => PlayableOutput.GetPlayableOutputType();

    public float OutputWeight
    {
        get => PlayableOutput.GetWeight();
        set => PlayableOutput.SetWeight(value);
    }

    public double Speed
    {
        get => Playable.GetSpeed();
        set => Playable.SetSpeed(value);
    }

    public double Time
    {
        get => Playable.GetTime();
        set => Playable.SetTime(value);
    }

    public double Duration
    {
        get => Playable.GetDuration();
        set => Playable.SetDuration(value);
    }

    public bool IsDone
    {
        get => Playable.IsDone();
        set => Playable.SetDone(value);
    }

    public List<(DebugPlayable Node, float Weight)> Inputs => this.ToList();

    private IEnumerable<(DebugPlayable Node, float Weight)> EnumeratePlayableInputs()
    {
        for (var port = 0; port < InputCount; ++port)
            if (Playable.IsValid())
                yield return (
                    Node: (DebugPlayable)Playable.GetInput(port),
                    Weight: Playable.GetInputWeight(port));
    }

    public List<DebugPlayable> Outputs => EnumeratePlayableOutputs().Select(p => (DebugPlayable)p).ToList();

    public IEnumerable<Playable> EnumeratePlayableOutputs()
    {
        var outputs = Playable.GetOutputCount();
        for (int i = 0; i < outputs; i++)
        {
            var output = Playable.GetOutput(i);
            yield return output;
        }
    }

    public IEnumerator<(DebugPlayable Node, float Weight)> GetEnumerator() => EnumeratePlayableInputs().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString() => $"{Type}: Inputs={InputCount}";
}

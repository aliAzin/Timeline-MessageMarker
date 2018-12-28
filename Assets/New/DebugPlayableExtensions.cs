using System.Collections.Generic;
using UnityEngine.Playables;

public static class DebugPlayableExtensions
{
    public static IEnumerable<Playable> EnumerateOutputs<T>(this T sourcePlayable) where T : struct, IPlayable
    {
        var outputs = sourcePlayable.GetOutputCount();
        for (int i = 0; i < outputs; i++)
        {
            var output = sourcePlayable.GetOutput(i);
            yield return output;
        }
    }

    public static IEnumerable<(Playable Node, float Weight)> EnumerateWeightedInputs<T>(this T sourcePlayable)
        where T : struct, IPlayable
    {
        var inputCount = sourcePlayable.GetInputCount();
        for (var port = 0; port < inputCount; ++port)
            if (sourcePlayable.IsValid())
                yield return (
                    Node: sourcePlayable.GetInput(port),
                    Weight: sourcePlayable.GetInputWeight(port));
    }

    public static IEnumerable<Playable> EnumerateInputs<T>(this T sourcePlayable) where T : struct, IPlayable
    {
        var inputCount = sourcePlayable.GetInputCount();
        for (var port = 0; port < inputCount; ++port)
            if (sourcePlayable.IsValid())
                yield return sourcePlayable.GetInput(port);
    }

    public static IEnumerable<TInput> EnumerateBehaviors<TInput>(this Playable sourcePlayable)
        where TInput : class, IPlayableBehaviour, new()
    {
        var inputCount = sourcePlayable.GetInputCount();
        for (var port = 0; port < inputCount; ++port)
        {
            if (sourcePlayable.IsValid())
            {
                var input = sourcePlayable.GetInput(port);
                if (input.GetPlayableType() == typeof(TInput))
                {
                    yield return ((ScriptPlayable<TInput>) input).GetBehaviour();
                }
            }
        }
    }

    public static TInput GetBehaviorAtIndex<TInput>(this Playable sourcePlayable, int index) where TInput : class, IPlayableBehaviour, new()
    {
        return ((ScriptPlayable<TInput>) sourcePlayable.GetInput(index)).GetBehaviour();
    }

}
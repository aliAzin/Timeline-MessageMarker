using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[ExecuteInEditMode]
public class TimelineEventRouter : MonoBehaviour
{
    private PlayableDirector _director;
    private bool _isAttached;
    private int _outputHash;
    private readonly INotificationReceiver _receiver = SomeEventReceiver.Instance;

    private void Awake()
    {
        _director = GetComponent<PlayableDirector>();

        Attach();

        if (_director.playOnAwake) // played event has already fired.
        {
            DirectorOnPlayed(_director);
        }
    }

    private void OnEnable()
    {
        if (!Application.isPlaying)
        {
            _isAttached = false;
        }
        Attach();
    }

    private void OnDisable()
    {
        Detach();
    }

    private void Attach()
    {
        if (!_isAttached && _director != null)
        {
            _director.played += DirectorOnPlayed;
            _isAttached = true;
        }
    }

    private void Detach()
    {
        if (!_isAttached && _director != null)
        {
            _director.played -= DirectorOnPlayed;

            if (_director.playableGraph.IsValid())
            {
                DetachReceiverFromOutput(_director.playableGraph.GetOutput(0), _receiver);
            }
        }
        _isAttached = false;
    }

    private void DirectorOnPlayed(PlayableDirector director)
    {
        if (!isActiveAndEnabled || _director == null || !_director.playableGraph.IsValid())
        {
            Detach();
            return;
        }

        var output = _director.playableGraph.GetOutput(0);
        var outputHash = output.GetHandle().GetHashCode();
        if (outputHash != _outputHash)
        {
            ApplyTrackNotificationsToMarkerTrack(_director, output);
            AttachReceiverToOutput(output, _receiver);
            _outputHash = outputHash;
        }
    }

    private void ApplyTrackNotificationsToMarkerTrack(PlayableDirector director, PlayableOutput output)
    {
        var timeline = director.playableAsset as TimelineAsset;
        var sourcePlayable = output.GetSourcePlayable();

        if (timeline != null)
        {
            var markerTrackBehavior = GetMarkerTrackBehavior(timeline, sourcePlayable);

            foreach (var track in timeline.GetOutputTracks().Where(t => t != timeline.markerTrack && !t.isSubTrack))
            {
                RegisterTimelineMarkers(director, output, track.GetMarkers(), markerTrackBehavior);
            }
        }
    }

    private static void AttachReceiverToOutput(PlayableOutput output, INotificationReceiver receiver)
    {
        if (!output.GetNotificationReceivers().Contains(receiver))
        {
            output.AddNotificationReceiver(receiver);
        }
    }

    private static void DetachReceiverFromOutput(PlayableOutput output, INotificationReceiver receiver)
    {
        if (output.GetNotificationReceivers().Contains(receiver))
        {
            output.RemoveNotificationReceiver(receiver);
        }
    }

    private static TimeNotificationBehaviour GetMarkerTrackBehavior(TimelineAsset timeline, Playable sourcePlayable)
    {
        TimeNotificationBehaviour result = default;
        for (var j = 0; j < timeline.outputTrackCount; j++)
        {
            var track = timeline.GetOutputTrack(j);
            if (track == timeline.markerTrack && !track.isSubTrack)
            {
                result = ((ScriptPlayable<TimeNotificationBehaviour>) sourcePlayable.GetInput(j)).GetBehaviour();
                break;
            }
        }

        return result;
    }

    public static void RegisterTimelineMarkers(PlayableDirector director, PlayableOutput output, IEnumerable<IMarker> markers, TimeNotificationBehaviour notificationBehavior)
    {
        foreach (var m in markers)
        {
            var notif = m as INotification;
            if (notif == null)
                continue;

            var time = (DiscreteTime) m.time;
            var tlDuration = (DiscreteTime) director.playableAsset.duration;
            if (time >= tlDuration && time <= tlDuration.OneTickAfter() && tlDuration != 0)
            {
                time = tlDuration.OneTickBefore();
            }

            if (m is INotificationOptionProvider notificationOptionProvider)
            {
                notificationBehavior.AddNotification((double) time, notif, notificationOptionProvider.flags);
            }
            else
            {
                notificationBehavior.AddNotification((double) time, notif);
            }
        }
    }
}
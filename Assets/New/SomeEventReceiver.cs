using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class SomeEventReceiver : INotificationReceiver
{
    private SomeEventReceiver() { }

    public static SomeEventReceiver Instance = new SomeEventReceiver();

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        Debug.Log($"External Received Notification! Playable:{origin.GetPlayableType()} Time:{origin.GetTime():N2} Type:{notification.GetType().Name} Id:{notification.id} Context={context}");
    }
}
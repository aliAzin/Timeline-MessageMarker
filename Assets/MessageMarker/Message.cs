using System;
using System.ComponentModel;
using System.Net;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

public enum ParameterType
{
    Int,
    Float,
    String,
    Object,
    None
}

[ExecuteInEditMode]
[Serializable, DisplayName("Message Marker")]
public class Message : Marker, INotification, INotificationOptionProvider
{
    public string method;
    public bool retroactive;
    public bool emitOnce;
    
    public ParameterType parameterType;
    public int Int;
    public string String;
    public float Float;
    public ExposedReference<Object> Object;

    PropertyName INotification.id => new PropertyName(method);

    public NotificationFlags flags =>
        (retroactive ? NotificationFlags.Retroactive : default) | 
        (emitOnce ? NotificationFlags.TriggerOnce : default) | 
        NotificationFlags.TriggerInEditMode;

}

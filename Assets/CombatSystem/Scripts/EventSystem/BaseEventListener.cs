using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEventListener : MonoBehaviour
{
    [Tooltip("Event to register with.")]
    public GameEvent Event;

    private void OnEnable()
    {
        if (Event == null)
            return;

        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        if (Event == null)
            return;

        Event.UnregisterListener(this);
    }

    public abstract void OnEventRaised();
}

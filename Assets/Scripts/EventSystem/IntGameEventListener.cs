using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class IntGameEventListener : MonoBehaviour
{
    [Tooltip("Event to register with.")]
    public IntGameEvent Event;

    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent<int> Response;

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

    public void OnEventRaised(int v)
    {
        Response?.Invoke(v);
    }
}

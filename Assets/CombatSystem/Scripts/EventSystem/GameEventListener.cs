using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : BaseEventListener
{
    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent Response;

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

    public override void OnEventRaised()
    {
        Response.Invoke();
    }
}

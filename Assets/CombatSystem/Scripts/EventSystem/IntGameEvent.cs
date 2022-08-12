using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Int Game Event", menuName = "Game Events/Int Game Event")]
public class IntGameEvent : ScriptableObject
{

    [SerializeField]
#if UNITY_EDITOR
    [Help("Please, submit any further explanations on where this event will be used and why for organization purposes.")]
#endif
    [TextArea(10, 20)]
    string NOTE;

    /// <summary>
    /// The list of listeners that this event will notify if it is raised.
    /// </summary>
    private readonly List<IntGameEventListener> eventListeners = new List<IntGameEventListener>();

    public void Raise(int value)
    {
        for (int i = eventListeners.Count - 1; i >= 0; i--)
            eventListeners[i].OnEventRaised(value);
    }

    public void RegisterListener(IntGameEventListener listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

    public void UnregisterListener(IntGameEventListener listener)
    {
        if (eventListeners.Contains(listener))
            eventListeners.Remove(listener);
    }
}

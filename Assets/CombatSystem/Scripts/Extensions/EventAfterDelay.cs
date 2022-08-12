using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Will call an event after a set delay. Good for testing purposes but can be used in game too.
/// </summary>
public class EventAfterDelay : MonoBehaviour
{
    [SerializeField] float delay;
    public UnityEvent events; 
    private void Start()
    {
        StartCoroutine(RaiseEvent());
    }

    private IEnumerator RaiseEvent()
    {
        yield return new WaitForSeconds(delay);
        events?.Invoke();
    }
}

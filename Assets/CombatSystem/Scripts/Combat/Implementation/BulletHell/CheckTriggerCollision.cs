using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class CheckTriggerCollision : MonoBehaviour
{
    public string checkTag;
    public UnityEvent TriggerEnterEvent;
    public UnityEvent TriggerExitEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(checkTag))
            return;

        TriggerEnterEvent?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag(checkTag))
            return;

        TriggerExitEvent?.Invoke();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all ACTions options
/// </summary>
public abstract class ACTModule : BaseEventListener
{
    public override void OnEventRaised() => ApplyACT();
    public abstract void ApplyACT();
}

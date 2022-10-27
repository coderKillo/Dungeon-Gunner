using System;
using UnityEngine;

public class DestroyedEvent : MonoBehaviour
{
    public Action<DestroyedEvent> OnDestroyed;

    public void CallDestroyedEvent()
    {
        OnDestroyed?.Invoke(this);
    }
}
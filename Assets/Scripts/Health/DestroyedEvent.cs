using System;
using UnityEngine;

public class DestroyedEvent : MonoBehaviour
{
    public Action<DestroyedEvent, DestroyedEventArgs> OnDestroyed;

    public void CallDestroyedEvent(int points = 0)
    {
        OnDestroyed?.Invoke(this, new DestroyedEventArgs()
        {
            points = points
        });
    }
}

public class DestroyedEventArgs : EventArgs
{
    public int points;
}

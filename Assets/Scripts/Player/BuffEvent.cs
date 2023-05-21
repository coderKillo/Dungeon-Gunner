using System;
using UnityEngine;

public class BuffEvent : MonoBehaviour
{
    public Action<BuffEvent, BuffEventArgs> OnAddBuff;

    public void CallAddBuff(Sprite icon, Color color, float duration)
    {
        OnAddBuff?.Invoke(this, new BuffEventArgs()
        {
            icon = icon,
            color = color,
            duration = duration,
        });
    }
}

public class BuffEventArgs : EventArgs
{
    public Sprite icon;
    public Color color;
    public float duration;
}
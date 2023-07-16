using System;
using UnityEngine;

public class BuffEvent : MonoBehaviour
{
    public Action<BuffEvent, AddBuffEventArgs> OnAddBuff;

    public void CallAddBuff(Sprite icon, CardPowerUp type, Color color, float duration)
    {
        OnAddBuff?.Invoke(this, new AddBuffEventArgs()
        {
            icon = icon,
            type = type,
            color = color,
            duration = duration,
        });
    }

    public Action<BuffEvent, RefreshBuffEventArgs> OnRefreshBuff;

    public void CallRefreshBuff(CardPowerUp type, float duration)
    {
        OnRefreshBuff?.Invoke(this, new RefreshBuffEventArgs()
        {
            type = type,
            duration = duration,
        });
    }
}

public class AddBuffEventArgs : EventArgs
{
    public Sprite icon;
    public CardPowerUp type;
    public Color color;
    public float duration;
}

public class RefreshBuffEventArgs : EventArgs
{
    public CardPowerUp type;
    public float duration;
}
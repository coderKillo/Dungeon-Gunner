using System;
using UnityEngine;

[DisallowMultipleComponent]
public class HealthEvent : MonoBehaviour
{
    public Action<HealthEvent, HealthEventArgs> OnHealthChanged;

    public void CallHealthEventChanged(float healthPercent, int healthAmount, int damageAmount, int healAmount)
    {
        OnHealthChanged?.Invoke(this, new HealthEventArgs()
        {
            healthPercent = healthPercent,
            healthAmount = healthAmount,
            damageAmount = damageAmount,
            healAmount = healAmount
        });
    }
}

public class HealthEventArgs : EventArgs
{
    public float healthPercent;
    public int healthAmount;
    public int damageAmount;
    public int healAmount;
}
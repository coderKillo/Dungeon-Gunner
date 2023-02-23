using System;
using UnityEngine;

[DisallowMultipleComponent]
public class HealthEvent : MonoBehaviour
{
    public Action<HealthEvent, HealthEventArgs> OnHealthChanged;

    public void CallHealthEventChanged(float healthPercent, int healthAmount, int damageAmount, int healAmount, float shieldPercent, int shieldAmount, bool isCrit = false)
    {
        OnHealthChanged?.Invoke(this, new HealthEventArgs()
        {
            healthPercent = healthPercent,
            healthAmount = healthAmount,
            damageAmount = damageAmount,
            healAmount = healAmount,
            shieldPercent = shieldPercent,
            shieldAmount = shieldAmount,
            isCrit = isCrit
        });
    }
}

public class HealthEventArgs : EventArgs
{
    public float healthPercent;
    public int healthAmount;
    public int damageAmount;
    public int healAmount;
    public float shieldPercent;
    public int shieldAmount;
    public bool isCrit;
}
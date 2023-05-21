using System;
using UnityEngine;

[DisallowMultipleComponent]
public class HealthEvent : MonoBehaviour
{
    public Action<HealthEvent, HealthEventArgs> OnHealthChanged;

    public void CallHealthEventChanged(float healthPercent, int healthAmount, int damageAmount, int healAmount, int totalHealth, float shieldPercent, int shieldAmount, int totalShield, bool isCrit = false)
    {
        OnHealthChanged?.Invoke(this, new HealthEventArgs()
        {
            healthPercent = healthPercent,
            healthAmount = healthAmount,
            damageAmount = damageAmount,
            healAmount = healAmount,
            totalHealth = totalHealth,
            shieldPercent = shieldPercent,
            shieldAmount = shieldAmount,
            totalShield = totalShield,
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
    public int totalHealth;
    public float shieldPercent;
    public int shieldAmount;
    public int totalShield;
    public bool isCrit;
}
using System;
using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    private HealthEvent healthEvent;
    [HideInInspector] public bool isDamageable = true;

    private int currentHealth;

    private int startingHealth;
    public int StartingHealth
    {
        set
        {
            startingHealth = value;
            currentHealth = value;
        }
        get
        {
            return startingHealth;
        }
    }

    private void Awake()
    {
        healthEvent = GetComponent<HealthEvent>();
    }

    private void Start()
    {
        CallHealthEvent(0);
    }

    public void TakeDamage(int damageAmount)
    {
        if (!isDamageable)
        {
            return;
        }

        currentHealth -= damageAmount;

        CallHealthEvent(damageAmount);
    }

    private void CallHealthEvent(int damageAmount)
    {
        healthEvent.CallHealthEventChanged(((float)currentHealth / (float)startingHealth), currentHealth, damageAmount);
    }
}

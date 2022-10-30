using System;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    [HorizontalGroup("Split", 0.5f)]
    [Button("+10", ButtonSizes.Large), GUIColor(0, 1, 0)]
    private void Heal()
    {
        TakeDamage(-10);
    }

    [HorizontalGroup("Split", 0.5f)]
    [Button("-10", ButtonSizes.Large), GUIColor(1, 0, 0)]
    private void Damage()
    {
        TakeDamage(10);
    }


    private HealthEvent healthEvent;
    private Player player;
    private Enemy enemy;

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

        player = GetComponent<Player>();
        enemy = GetComponent<Enemy>();
    }

    public void TakeDamage(int damageAmount)
    {
        if (!IsDamageable())
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

    private bool IsDamageable()
    {
        if (!isDamageable)
        {
            return false;
        }

        if (player && player.IsRolling())
        {
            return false;
        }

        return true;
    }
}

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
        Heal(10);
    }

    [HorizontalGroup("Split", 0.5f)]
    [Button("-10", ButtonSizes.Large), GUIColor(1, 0, 0)]
    private void Damage()
    {
        TakeDamage(10, false);
    }

    [Button("+10", ButtonSizes.Large), GUIColor(0, 0, 1)]
    private void Armor()
    {
        AddArmor(10);
    }

    private HealthEvent healthEvent;
    private Player player;
    private Enemy enemy;

    [ShowInInspector] public bool isDamageable = true;


    [ShowInInspector] private int currentHealth;
    [ShowInInspector] private int currentArmor;
    [ShowInInspector] private int maxArmor = 300;

    [ShowInInspector] private int startingHealth;
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
        CallHealthEvent(0, 0, false);

        player = GetComponent<Player>();
        enemy = GetComponent<Enemy>();
    }

    public void TakeDamage(int damageAmount, bool isCrit)
    {
        if (damageAmount <= 0)
        {
            return;
        }

        if (!IsDamageable())
        {
            return;
        }

        if (currentArmor > damageAmount)
        {
            currentArmor -= damageAmount;
            damageAmount = 0;
        }
        else if (currentArmor > 0)
        {
            damageAmount -= currentArmor;
            currentArmor = 0;
        }

        currentHealth -= damageAmount;

        CallHealthEvent(damageAmount, 0, isCrit);
    }

    public void Heal(int amount)
    {
        var healthMissing = startingHealth - currentHealth;
        var capedHealAmount = Math.Clamp(amount, 0, healthMissing);

        currentHealth += capedHealAmount;

        CallHealthEvent(0, capedHealAmount, false);
    }

    public void IncreaseMaxHealth(int amount)
    {
        startingHealth += amount;
        currentHealth += amount;

        CallHealthEvent(0, 0, false);
    }

    public void AddArmor(int amount)
    {
        currentArmor += amount;
        currentArmor = Math.Clamp(currentArmor, 0, maxArmor);

        CallHealthEvent(0, 0, false);
    }

    private void CallHealthEvent(int damageAmount, int healAmount, bool isCrit)
    {
        healthEvent.CallHealthEventChanged(((float)currentHealth / (float)startingHealth), currentHealth, damageAmount, healAmount, startingHealth, ((float)currentArmor / (float)maxArmor), currentArmor, maxArmor, isCrit);
    }

    private bool IsDamageable()
    {
        if (!isDamageable)
        {
            return false;
        }

        if (player && player.IsDashing())
        {
            return false;
        }

        return true;
    }
}

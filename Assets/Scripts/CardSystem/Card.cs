using System.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card
{
    public CardSO details;
    public Guid id;
    public int level = 1;
    public float value = 1.0f;

    public Guid weaponId;

    public void Select(Player player)
    {
        Weapon weapon;
        switch (details.action)
        {
            case CardAction.AddWeapon:
                weapon = player.GetWeapon(weaponId);

                weapon.damageFactor = 1 + (details.weaponDamageFactorPerLevel * (level - 1));

                player.setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);

                if (weapon.weaponDetails.ammoCapacity > 0)
                {
                    value = (float)weapon.totalAmmo / (float)weapon.weaponDetails.ammoCapacity;
                }

                break;

            case CardAction.Heal:
            case CardAction.Shield:
            case CardAction.Ammo:
                break;


            default:
                break;
        }
    }

    public void Deselect(Player player)
    {
        switch (details.action)
        {
            case CardAction.AddWeapon:
                player.setActiveWeaponEvent.CallSetActiveWeaponEvent(null);
                break;


            default:
                break;
        }

    }

    public void Activate(Player player)
    {
        switch (details.action)
        {
            case CardAction.Heal:
                var healValue = details.healAmount * level;

                player.health.Heal(healValue);

                value = 0f;

                break;


            case CardAction.Shield:
                var shieldValue = details.shieldAmount * level;

                player.health.AddArmor(shieldValue);

                value = 0f;

                break;


            case CardAction.PowerUp:
                ActivatePowerUp(details.powerUpType, details.powerUpColor, player);

                break;


            case CardAction.Ammo:
                foreach (var weapon in player.GetAllWeapons())
                {
                    var increaseValue = Mathf.RoundToInt(weapon.weaponDetails.ammoCapacity * (details.ammoAmount * level / 100f));
                    weapon.totalAmmo = Mathf.Clamp(weapon.totalAmmo + increaseValue, 0, weapon.weaponDetails.ammoCapacity);
                }

                value = 0f;

                break;


            case CardAction.AddWeapon:
                var currentWeapon = player.activeWeapon.CurrentWeapon;
                if (currentWeapon.weaponDetails.ammoCapacity > 0)
                {
                    value = (float)currentWeapon.totalAmmo / (float)currentWeapon.weaponDetails.ammoCapacity;
                }
                break;


            default:
                break;
        }

    }

    private void ActivatePowerUp(CardPowerUp powerUpType, Color powerUpColor, Player player)
    {
        var powerUp = CardPowerUpManager.GetPowerUp(powerUpType);
        powerUp.Initialize(details, level);

        value -= 1f / details.stacks;

        player.playerPowerUp.AddPowerUp(powerUpType, powerUp);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card
{
    public CardSO details;
    public int id;
    public int level = 1;

    public void Action(Player player)
    {
        switch (details.action)
        {
            case CardAction.Heal:
                var healValue = details.healAmount * level;

                player.health.Heal(healValue);

                break;


            case CardAction.Shield:
                var shieldValue = details.shieldAmount * level;

                player.health.AddArmor(shieldValue);

                break;


            case CardAction.PowerUp:
                player.playerPowerUp.StartPowerUp(SpeedPowerUp(), Color.green);
                break;


            case CardAction.Ammo:
                var currentWeapon = player.activeWeapon.CurrentWeapon;
                var increaseValue = Mathf.RoundToInt(currentWeapon.weaponDetails.ammoCapacity * (details.ammoAmount * level / 100f));

                currentWeapon.totalAmmo = Mathf.Clamp(currentWeapon.totalAmmo + increaseValue, 0, currentWeapon.weaponDetails.ammoCapacity);

                player.weaponReloadedEvent.CallWeaponReloadedEvent(currentWeapon);

                break;


            case CardAction.AddWeapon:
                var weapon = player.AddWeaponToPlayer(details.weapon);

                weapon.damageFactor += (0.2f * level);

                break;


            default:
                break;
        }

    }

    static private IEnumerator SpeedPowerUp()
    {
        // TODO: implement powerups and outsource to own class
        yield return new WaitForSeconds(5f);
    }
}


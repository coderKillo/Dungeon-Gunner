using System.Diagnostics;
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
                ActivatePowerUp(details.powerUpType, details.powerUpColor, player);
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

    private void ActivatePowerUp(CardPowerUp powerUpType, Color powerUpColor, Player player)
    {
        switch (powerUpType)
        {
            case CardPowerUp.Crit:
                player.playerPowerUp.StartPowerUp(CritPowerUp(player), powerUpColor);
                break;
            case CardPowerUp.Speed:
                player.playerPowerUp.StartPowerUp(SpeedPowerUp(player), powerUpColor);
                break;
            case CardPowerUp.MultiShot:
                player.playerPowerUp.StartPowerUp(MultiShotPowerUp(player), powerUpColor);
                break;
            case CardPowerUp.Reflect:
                player.playerPowerUp.StartPowerUp(ReflectPowerUp(player), powerUpColor);
                break;
            case CardPowerUp.BlackHole:
                break;
            case CardPowerUp.FireBall:
                break;
            case CardPowerUp.LightningShot:
                player.playerPowerUp.StartPowerUp(LightningShotPowerUp(player), powerUpColor);
                break;
            case CardPowerUp.LightningDash:
                player.playerPowerUp.StartPowerUp(LightningDashPowerUp(player), powerUpColor);
                break;

            default:
                break;
        }
    }

    private IEnumerator CritPowerUp(Player player)
    {
        var critChance = (details.powerUpAbility + (details.powerUpScaleAbility * level)) * 100f;
        var duration = details.powerUpDuration + (details.powerUpScaleDuration * level);

        var weaponCritChanceFactor = player.fireWeapon.WeaponCritChanceFactor;
        player.fireWeapon.WeaponCritChanceFactor = critChance;

        yield return new WaitForSeconds(duration);

        player.fireWeapon.WeaponCritChanceFactor = weaponCritChanceFactor;
    }

    private IEnumerator SpeedPowerUp(Player player)
    {
        var moveSpeedFactor = details.powerUpAbility + (details.powerUpScaleAbility * level);
        var duration = details.powerUpDuration + (details.powerUpScaleDuration * level);

        var moveSpeed = player.playerControl.MoveSpeed;
        player.playerControl.MoveSpeed = moveSpeedFactor * moveSpeed;

        yield return new WaitForSeconds(duration);

        player.playerControl.MoveSpeed = moveSpeed;
    }

    private IEnumerator MultiShotPowerUp(Player player)
    {
        var multiShot = Mathf.RoundToInt(details.powerUpAbility + (details.powerUpScaleAbility * level));
        var duration = details.powerUpDuration + (details.powerUpScaleDuration * level);

        var weaponCritChanceFactor = player.fireWeapon.WeaponCritChanceFactor;
        player.fireWeapon.MultiShot = multiShot;

        yield return new WaitForSeconds(duration);

        player.fireWeapon.MultiShot = 1;
    }

    private IEnumerator ReflectPowerUp(Player player)
    {
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator BlackHolePowerUp(Player player)
    {
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator FireBallPowerUp(Player player)
    {
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator LightningShotPowerUp(Player player)
    {
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator LightningDashPowerUp(Player player)
    {
        yield return new WaitForSeconds(1f);
    }
}


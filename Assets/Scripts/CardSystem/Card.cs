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

    public void Select(Player player)
    {
        switch (details.action)
        {
            case CardAction.PowerUp:
                switch (details.powerUpType)
                {
                    case CardPowerUp.BlackHole:
                        var radius = details.powerUpAbility + (details.powerUpScaleAbility * level);
                        details.powerUpSpell.ammo.range = Mathf.RoundToInt(radius);
                        player.AddWeaponToPlayer(details.powerUpSpell);
                        break;
                    case CardPowerUp.FireBall:
                        var damage = details.powerUpAbility + (details.powerUpScaleAbility * level);
                        details.powerUpSpell.ammo.damage = Mathf.RoundToInt(damage);
                        player.AddWeaponToPlayer(details.powerUpSpell);
                        break;

                    default:
                        break;
                }
                break;

            case CardAction.AddWeapon:
                var weapon = player.GetWeapon(details.weapon);

                weapon.damageFactor = (details.weaponDamageFactorPerLevel * level);

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
            case CardAction.PowerUp:
                switch (details.powerUpType)
                {
                    case CardPowerUp.BlackHole:
                        player.RemoveWeaponFromPlayer(details.powerUpSpell);
                        break;
                    case CardPowerUp.FireBall:
                        player.RemoveWeaponFromPlayer(details.powerUpSpell);
                        break;

                    default:
                        break;
                }
                break;

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

                value = 0f;

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
        if (details.powerUpDuration > 0f)
        {
            player.buffEvent.CallAddBuff(details.icon, details.powerUpColor, PowerUpDuration());
        }

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

    private float PowerUpDuration()
    {
        return details.powerUpDuration + (details.powerUpScaleDuration * level);
    }

    private IEnumerator CritPowerUp(Player player)
    {
        var critChance = (details.powerUpAbility + (details.powerUpScaleAbility * level)) * 100f;

        var weaponCritChanceFactor = player.fireWeapon.WeaponCritChanceFactor;
        player.fireWeapon.WeaponCritChanceFactor = critChance;

        yield return new WaitForSeconds(PowerUpDuration());

        player.fireWeapon.WeaponCritChanceFactor = weaponCritChanceFactor;
    }

    private IEnumerator SpeedPowerUp(Player player)
    {
        var moveSpeedFactor = details.powerUpAbility + (details.powerUpScaleAbility * level);

        var moveSpeed = player.playerControl.MoveSpeed;
        player.playerControl.MoveSpeed = moveSpeedFactor * moveSpeed;

        yield return new WaitForSeconds(PowerUpDuration());

        player.playerControl.MoveSpeed = moveSpeed;
    }

    private IEnumerator MultiShotPowerUp(Player player)
    {
        var multiShot = Mathf.RoundToInt(details.powerUpAbility + (details.powerUpScaleAbility * level));

        var weaponCritChanceFactor = player.fireWeapon.WeaponCritChanceFactor;
        player.fireWeapon.MultiShot = multiShot;

        yield return new WaitForSeconds(PowerUpDuration());

        player.fireWeapon.MultiShot = 1;
    }

    private IEnumerator ReflectPowerUp(Player player)
    {
        player.playerReflectAmmo.Enable();

        yield return new WaitForSeconds(PowerUpDuration());

        player.playerReflectAmmo.Disable();
    }

    private IEnumerator LightningShotPowerUp(Player player)
    {
        var damage = details.powerUpAbility + (details.powerUpScaleAbility * level);

        player.fireWeapon.OnHitEffect = details.OnHitEffect;
        player.fireWeapon.OnHitDamage = Mathf.RoundToInt(damage);

        yield return new WaitForSeconds(PowerUpDuration());

        player.fireWeapon.OnHitEffect = null;
    }

    private IEnumerator LightningDashPowerUp(Player player)
    {
        var damage = details.powerUpAbility + (details.powerUpScaleAbility * level);

        player.playerDash.Effect = GameResources.Instance.dashLightningEffect;
        player.playerDash.Damage = Mathf.RoundToInt(damage);

        yield return new WaitForSeconds(PowerUpDuration());

        player.playerDash.Effect = GameResources.Instance.dashSmokeEffect;
        player.playerDash.Damage = Mathf.RoundToInt(0);
    }
}


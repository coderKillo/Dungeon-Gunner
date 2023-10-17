using UnityEngine;

public enum CardPowerUp
{
    Crit,
    Speed,
    MultiShot,
    Reflect,
    LightningShot,
    ExplosiveShot,
    FrostShot,
    VampiricShot,
    Berserk,
    LightningDash
}

public abstract class ICardPowerUp
{
    protected int _level = 0;
    public int Level { get { return _level; } }
    protected CardSO _details;
    public CardSO Details { get { return _details; } }

    public void Initialize(CardSO details, int level)
    {
        _level = level;
        _details = details;
    }

    public float Duration()
    {
        return _details.powerUpDuration + (_details.powerUpScaleDuration * _level);
    }

    public abstract void Activate(Player player);
    public abstract void Deactivate(Player player);
}

public class CardPowerUpManager
{
    static public ICardPowerUp GetPowerUp(CardPowerUp type)
    {
        switch (type)
        {
            case CardPowerUp.Crit:
                return new CritPowerUp();
            case CardPowerUp.Speed:
                return new SpeedPowerUp();
            case CardPowerUp.MultiShot:
                return new MultiShotPowerUp();
            case CardPowerUp.Reflect:
                return new ReflectPowerUp();
            case CardPowerUp.LightningDash:
                return new LightningDashPowerUp();
            case CardPowerUp.Berserk:
                return new BerserkPowerUp();
            case CardPowerUp.LightningShot:
            case CardPowerUp.VampiricShot:
            case CardPowerUp.FrostShot:
            case CardPowerUp.ExplosiveShot:
                return new ShotPowerUp();
            default:
                Debug.LogWarning($"Get unknown power up: {type}");
                return null;
        }
    }
}


public class CritPowerUp : ICardPowerUp
{
    private float _weaponCritChanceFactor = 0f;

    public override void Activate(Player player)
    {
        var critChance = (_details.powerUpAbility + (_details.powerUpScaleAbility * _level)) * 100f;

        _weaponCritChanceFactor = player.fireWeapon.WeaponCritChanceFactor;
        player.fireWeapon.WeaponCritChanceFactor = critChance;
    }

    public override void Deactivate(Player player)
    {
        player.fireWeapon.WeaponCritChanceFactor = _weaponCritChanceFactor;
    }
}

public class SpeedPowerUp : ICardPowerUp
{
    private float _moveSpeed = 0f;

    public override void Activate(Player player)
    {
        _moveSpeed = player.playerControl.MoveSpeed;

        var moveSpeedFactor = _details.powerUpAbility + (_details.powerUpScaleAbility * _level);

        player.playerControl.MoveSpeed = moveSpeedFactor * _moveSpeed;
        player.evasion.evadeIncomingAttacks = true;
    }

    public override void Deactivate(Player player)
    {
        player.playerControl.MoveSpeed = _moveSpeed;
        player.evasion.evadeIncomingAttacks = false;
    }
}

public class MultiShotPowerUp : ICardPowerUp
{
    public override void Activate(Player player)
    {
        var multiShot = Mathf.RoundToInt(_details.powerUpAbility + (_details.powerUpScaleAbility * _level));

        var weaponCritChanceFactor = player.fireWeapon.WeaponCritChanceFactor;
        player.fireWeapon.MultiShot = multiShot;
    }

    public override void Deactivate(Player player)
    {
        player.fireWeapon.MultiShot = 1;
    }
}

public class ReflectPowerUp : ICardPowerUp
{
    public override void Activate(Player player)
    {
        player.playerReflectAmmo.gameObject.SetActive(true);
    }

    public override void Deactivate(Player player)
    {
        player.playerReflectAmmo.gameObject.SetActive(false);
    }
}

public class ShotPowerUp : ICardPowerUp
{
    public override void Activate(Player player)
    {
        var damage = _details.powerUpAbility + (_details.powerUpScaleAbility * _level);
        var onHit = new OnHit()
        {
            effect = _details.OnHitEffect,
            radius = _details.onHitRadius,
            damage = Mathf.RoundToInt(damage)
        };

        player.fireWeapon.AddOnHitEffect(_details.title, onHit);
    }

    public override void Deactivate(Player player)
    {
        player.fireWeapon.RemoveOnHitEffect(_details.title);
    }
}

public class LightningDashPowerUp : ICardPowerUp
{
    public override void Activate(Player player)
    {
        var damage = _details.powerUpAbility + (_details.powerUpScaleAbility * _level);

        player.playerDash.Effect = GameResources.Instance.dashLightningEffect;
        player.playerDash.Damage = Mathf.RoundToInt(damage);
    }

    public override void Deactivate(Player player)
    {
        player.playerDash.Effect = GameResources.Instance.dashSmokeEffect;
        player.playerDash.Damage = Mathf.RoundToInt(0);
    }
}

public class BerserkPowerUp : ICardPowerUp
{
    private float _damageFactor = 0f;
    private float _attackSpeed = 0f;

    public override void Activate(Player player)
    {
        _damageFactor = player.fireWeapon.WeaponDamageFactor;
        _attackSpeed = player.fireWeapon.WeaponAttackSpeedFactor;

        var power = _details.powerUpAbility + (_details.powerUpScaleAbility * _level);

        player.fireWeapon.WeaponDamageFactor += power;
        player.fireWeapon.WeaponAttackSpeedFactor *= Mathf.Clamp(1 - power, 0f, 2f);
    }

    public override void Deactivate(Player player)
    {
        player.fireWeapon.WeaponDamageFactor = _damageFactor;
        player.fireWeapon.WeaponAttackSpeedFactor = _attackSpeed;
    }
}

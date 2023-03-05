using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Card_", menuName = "Scriptable Object/Card System/Card")]
public class CardSO : ScriptableObject
{
    public string title;
    public CardRarity rarity;
    public Sprite icon;
    public Sprite iconMini;

    [TextArea]
    public string description;

    [Space(10)]
    [Header("Action")]
    [EnumToggleButtons]
    public CardAction action;

    [ShowIf("action", CardAction.Heal)]
    public int healAmount;

    [ShowIf("action", CardAction.Shield)]
    public int shieldAmount;

    [ShowIf("action", CardAction.Ammo)]
    public float ammoAmount;

    [ShowIf("action", CardAction.AddWeapon)]
    public WeaponDetailsSO weapon;

    [ShowIf("action", CardAction.AddWeapon)]
    public float weaponDamageFactorPerLevel;

    [ShowIf("action", CardAction.PowerUp)]
    public CardPowerUp powerUpType;

    [ShowIf("action", CardAction.PowerUp)]
    public Color powerUpColor;

    [ShowIf("action", CardAction.PowerUp)]
    public float powerUpDuration;

    [ShowIf("action", CardAction.PowerUp)]
    public float powerUpScaleDuration;

    [ShowIf("action", CardAction.PowerUp)]
    public float powerUpAbility;

    [ShowIf("action", CardAction.PowerUp)]
    public float powerUpScaleAbility;
}

public enum CardAction
{
    Heal,
    Shield,
    Ammo,
    AddWeapon,
    PowerUp
}

public enum CardPowerUp
{
    Crit,
    Speed,
    MultiShot,
    Reflect,
    BlackHole,
    FireBall,
    LightningShot,
    LightningDash
}

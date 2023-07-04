using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Card_", menuName = "Scriptable Object/Card System/Card")]
public class CardSO : ScriptableObject
{
    public string title;
    public CardRarity rarity;
    public Sprite icon;
    public Vector2 iconSize = new Vector2(48f, 48f);
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
    public float powerUpAbility;

    [ShowIf("action", CardAction.PowerUp)]
    public float powerUpScaleAbility;

    [ShowIf("action", CardAction.PowerUp)]
    public bool isBuff;

    [ShowIf("isBuff")]
    public Color powerUpColor;

    [ShowIf("isBuff")]
    public float powerUpDuration;

    [ShowIf("isBuff")]
    public float powerUpScaleDuration;

    [ShowIf("isBuff")]
    public bool isOnHit;

    [ShowIf("isOnHit")]
    public GameObject OnHitEffect;

    [ShowIf("isOnHit")]
    public float onHitRadius;

    [ShowIf("action", CardAction.PowerUp)]
    public bool isSpell;

    [ShowIf("isSpell")]
    public WeaponDetailsSO powerUpSpell;

    public bool IsKnown()
    {
        return PlayerPrefs.HasKey(PrefKeys.CardKey(this));
    }

    public void MakeKnown()
    {
        PlayerPrefs.SetInt(PrefKeys.CardKey(this), 1);
    }

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

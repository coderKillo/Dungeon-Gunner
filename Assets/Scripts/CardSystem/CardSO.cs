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

    [ShowIf("action", CardAction.PowerUp)]
    public UnityEvent powerUpEvent;
}

public enum CardAction
{
    Heal,
    Shield,
    Ammo,
    AddWeapon,
    PowerUp
}

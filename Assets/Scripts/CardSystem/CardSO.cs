using UnityEngine;
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
    public float healAmount;

    [ShowIf("action", CardAction.Ammo)]
    public float ammoAmount;

    [ShowIf("action", CardAction.AddWeapon)]
    public WeaponDetailsSO weapon;

}

public enum CardAction
{
    Heal,
    Ammo,
    AddWeapon
}

using UnityEngine;

[CreateAssetMenu(fileName = "Card_", menuName = "Scriptable Object/Card System/Card")]
public class CardSO : ScriptableObject
{
    public string title;
    public CardRarity rarity;
    public Sprite sprite;

    [TextArea]
    public string description;
}

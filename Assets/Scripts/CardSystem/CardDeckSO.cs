using UnityEngine;

[CreateAssetMenu(fileName = "CardDeck_", menuName = "Scriptable Object/Card System/Card Deck")]
public class CardDeckSO : ScriptableObject
{
    public CardSO[] deck;
}

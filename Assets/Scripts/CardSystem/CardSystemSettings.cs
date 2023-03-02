using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

public class CardSystemSettings : SerializedMonoBehaviour
{
    [Space(10)]
    [Header("Rarity Color")]
    [SerializeField] private Dictionary<CardRarity, Color> _rarityColorLookup;

    [Space(10)]
    [Header("Rarity Ratio")]
    [SerializeField] private Dictionary<CardRarity, int> _rarityRatio;

    [Space(10)]
    [Header("Card Level")]
    [SerializeField] private Dictionary<DungeonLevelSO, LevelRange> _cardLevelByDungeonLevel;

    public Color GetColor(CardRarity rarity)
    {
        return _rarityColorLookup.GetValueOrDefault(rarity, Color.white);
    }

    public Card PickRandomCard(Card[] cards)
    {
        var rarityListLookup = new List<CardRarity>();

        foreach (var item in _rarityRatio)
        {
            rarityListLookup.AddRange(Enumerable.Repeat(item.Key, item.Value));
        }

        var selectedRarity = rarityListLookup[Random.Range(0, rarityListLookup.Count)];
        var selectedRarityCards = new List<Card>();

        foreach (var card in cards)
        {
            if (card.details.rarity != selectedRarity)
            {
                continue;
            }

            selectedRarityCards.Add(card);
        }

        if (selectedRarityCards.Count <= 0)
        {
            return cards[Random.Range(0, cards.Length)];
        }

        return selectedRarityCards[Random.Range(0, selectedRarityCards.Count)];
    }

    public int GetCardLevelByDungeonLevel(DungeonLevelSO level)
    {
        if (!_cardLevelByDungeonLevel.ContainsKey(level))
        {
            return Random.Range(1, 3);
        }

        return Random.Range(_cardLevelByDungeonLevel[level].level.x, _cardLevelByDungeonLevel[level].level.y);
    }
}

public struct LevelRange
{
    [MinMaxSlider(1, 5)]
    public Vector2Int level;
}

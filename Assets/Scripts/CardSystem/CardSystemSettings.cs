using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

public class CardSystemSettings : SerializedMonoBehaviour
{
    [Space(10)]
    [Header("Rarity Ratio")]
    [SerializeField] private Dictionary<CardRarity, int> _rarityRatio;

    static public Color GetColor(CardRarity rarity)
    {
        return GameResources.Instance.rarityColorLookup.GetValueOrDefault(rarity, Color.gray);
    }

    public CardSO PickRandomCard(CardSO[] cards)
    {
        var rarityListLookup = new List<CardRarity>();

        foreach (var item in _rarityRatio)
        {
            rarityListLookup.AddRange(Enumerable.Repeat(item.Key, item.Value));
        }

        var selectedRarity = rarityListLookup[Random.Range(0, rarityListLookup.Count)];

        return PickRandomCardWithSpecificRarity(cards, selectedRarity);
    }

    public CardSO PickRandomCardWithSpecificRarity(CardSO[] cards, CardRarity rarity)
    {
        var selectedRarityCards = new List<CardSO>();

        foreach (var card in cards)
        {
            if (card.rarity != rarity)
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
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CardDeck : MonoBehaviour
{
    [SerializeField] private CardDeckSO playerDeck;

    private Dictionary<CardRarity, List<CardSO>> _deckDirectory = new Dictionary<CardRarity, List<CardSO>>();

    private void Awake()
    {
        foreach (var card in playerDeck.deck)
        {
            if (!_deckDirectory.ContainsKey(card.rarity))
            {
                _deckDirectory.Add(card.rarity, new List<CardSO>());
            }
            _deckDirectory[card.rarity].Add(card);
        }
    }

    public CardSO PickCard(CardRarity rarity)
    {
        Debug.Assert(_deckDirectory.ContainsKey(rarity), $"deck does not contain rarity: {rarity}");

        var cards = _deckDirectory[rarity];

        if (cards.Count <= 0)
        {
            FillDirectory(rarity);
            cards = _deckDirectory[rarity];
        }

        var index = Random.Range(0, cards.Count);
        var card = cards[index];

        cards.RemoveAt(index);

        return card;
    }

    private void FillDirectory(CardRarity rarity)
    {
        foreach (var card in playerDeck.deck)
        {
            if (card.rarity != rarity) continue;

            _deckDirectory[rarity].Add(card);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CardDraw : MonoBehaviour
{
    private List<Card> _draw = new List<Card>();

    [HideInInspector] public Action<Card[]> OnDraw;
    [HideInInspector] public Action<Card> OnCardSelected;

    private CardSystemSettings _cardSystemSettings;
    private CardRarity _priority = CardRarity.Common;
    public CardRarity Priority { set { _priority = value; } }

    private void Awake()
    {
        _cardSystemSettings = GetComponent<CardSystemSettings>();
    }

    public void Draw(CardSO[] cards)
    {
        _draw.Clear();

        for (int i = 0; i < Settings.cardDrawSize; i++)
        {
            var card = new Card();
            card.id = Guid.NewGuid();
            card.level = _cardSystemSettings.GetCardLevelByDungeonLevel(GameManager.Instance.CurrentLevel);
            card.details = GetUniqueDetails(cards);

            _draw.Add(card);
        }

        if (!ContainsRarity(_priority))
        {
            _draw[UnityEngine.Random.Range(0, Settings.cardDrawSize)].details = _cardSystemSettings.PickRandomCardWithSpecificRarity(cards, _priority);
        }

        OnDraw?.Invoke(_draw.ToArray());
    }

    private CardSO GetUniqueDetails(CardSO[] cards)
    {
        var details = _cardSystemSettings.PickRandomCard(cards);

        int maxRetry = 10;
        int j = 0;
        while (ContainsDetails(details) && j < maxRetry)
        {
            details = _cardSystemSettings.PickRandomCardWithSpecificRarity(cards, details.rarity);
            j++;
        }

        return details;
    }

    private bool ContainsRarity(CardRarity rarity)
    {
        return (_draw.Find((x) => x.details.rarity == rarity)) != null;
    }

    private bool ContainsDetails(CardSO details)
    {
        return (_draw.Find((x) => x.details == details)) != null;
    }

    public void CardSelected(int id)
    {
        OnCardSelected?.Invoke(_draw[id]);

        _draw.Clear();
    }

}

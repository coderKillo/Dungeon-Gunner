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

    private void Awake()
    {
        _cardSystemSettings = GetComponent<CardSystemSettings>();
    }

    public void Draw(Card[] cards)
    {
        _draw.Clear();

        for (int i = 0; i < Settings.cardDrawSize; i++)
        {
            var card = _cardSystemSettings.PickRandomCard(cards);
            card.level = _cardSystemSettings.GetCardLevelByDungeonLevel(GameManager.Instance.CurrentLevel);

            _draw.Add(card);
        }

        OnDraw?.Invoke(_draw.ToArray());
    }

    public void CardSelected(int id)
    {
        OnCardSelected?.Invoke(_draw[id]);

        _draw.Clear();
    }

}

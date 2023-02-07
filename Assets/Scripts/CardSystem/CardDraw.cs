using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CardDraw : MonoBehaviour
{
    private List<Card> _draw = new List<Card>();

    [HideInInspector] public Action<Card[]> OnDraw;
    [HideInInspector] public Action<Card> OnCardSelected;

    public void Draw(Card[] cards)
    {
        _draw.Clear();

        for (int i = 0; i < Settings.cardDrawSize; i++)
        {
            _draw.Add(PickRandomCard(cards));
        }

        OnDraw?.Invoke(_draw.ToArray());
    }

    public void CardSelected(int id)
    {
        OnCardSelected?.Invoke(_draw[id]);

        _draw.Clear();
    }

    private Card PickRandomCard(Card[] cards)
    {
        return cards[UnityEngine.Random.Range(0, cards.Length - 1)];
    }

}

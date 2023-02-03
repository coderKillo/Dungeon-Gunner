using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CardDraw : MonoBehaviour
{
    private List<CardSO> _draw = new List<CardSO>();

    [HideInInspector] public Action<CardSO[]> OnDraw;
    [HideInInspector] public Action<CardSO> OnCardSelected;

    public void Draw(CardSO[] cards)
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

    private CardSO PickRandomCard(CardSO[] cards)
    {
        return cards[UnityEngine.Random.Range(0, cards.Length - 1)];
    }

}

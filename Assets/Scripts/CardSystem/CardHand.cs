using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CardHand : MonoBehaviour
{
    private List<Card> _hand = new List<Card>();

    [HideInInspector] public Action<Card> OnCardAdd;
    [HideInInspector] public Action<Card> OnCardRemove;
    [HideInInspector] public Action<Card> OnCardSelected;
    [HideInInspector] public Action<Boolean> OnShow;

    public void Add(Card card)
    {
        _hand.Add(card);

        OnCardAdd?.Invoke(card);
    }

    public void Remove(Card card)
    {
        _hand.Remove(card);

        OnCardRemove?.Invoke(card);
    }

    public void CardSelected(int id)
    {
        OnCardSelected?.Invoke(_hand[id]);
    }

    public void Show(bool show)
    {
        OnShow?.Invoke(show);
    }

    public bool Full() { return _hand.Count > Settings.cardHandSize; }
}

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CardHand : MonoBehaviour
{
    private List<CardSO> _hand = new List<CardSO>();

    [HideInInspector] public Action<CardSO> OnCardAdd;
    [HideInInspector] public Action<CardSO> OnCardRemove;
    [HideInInspector] public Action<CardSO> OnCardSelected;
    [HideInInspector] public Action<Boolean> OnShow;

    public void Add(CardSO card)
    {
        _hand.Add(card);

        OnCardAdd?.Invoke(card);
    }

    public void Remove(CardSO card)
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

    public bool Full() { return _hand.Count >= Settings.cardHandSize; }
}

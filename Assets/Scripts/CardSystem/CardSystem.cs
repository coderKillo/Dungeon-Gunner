using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Sirenix.OdinInspector;

// TODO: add state machine
public class CardSystem : SingletonAbstract<CardSystem>
{
    [SerializeField] private CardDeckSO playerDeck;

    private List<CardSO> _deck = new List<CardSO>();

    private CardHand _cardHand;
    private CardDraw _cardDraw;

    protected override void Awake()
    {
        base.Awake();

        CreateDeck();

        _cardDraw = GetComponent<CardDraw>();
        _cardHand = GetComponent<CardHand>();

        _cardDraw.OnCardSelected += CardDraw_OnCardSelected;
        _cardHand.OnCardSelected += CardHand_OnCardSelected;
    }

    private void CardHand_OnCardSelected(CardSO card)
    {
        _cardHand.Remove(card);
    }

    private void CardDraw_OnCardSelected(CardSO card)
    {
        _cardHand.Add(card);
    }

    [Button("Draw Card")]
    public void Draw()
    {
        _cardDraw.Draw(_deck.ToArray());
    }

    [Button("Show")]
    public void Show()
    {
        _cardHand.Show(true);
    }

    [Button("Hide")]
    public void Hide()
    {
        _cardHand.Show(false);
    }

    private void CreateDeck()
    {
        foreach (var card in playerDeck.deck)
        {
            _deck.Add(card);
        }
    }
}

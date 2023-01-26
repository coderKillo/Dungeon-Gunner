using System.Net.Mime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawUI : MonoBehaviour
{
    [SerializeField] private GameObject _cardPrefab;

    private Animator _animator;
    private List<Card> _cards;

    private void Awake()
    {
        _cards = new List<Card>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        CardSystem.OnDraw += Draw;
    }

    public void Clear()
    {
        foreach (var card in _cards)
        {
            Destroy(card.gameObject);
        }

        _cards.Clear();
    }

    public void Draw(CardSO[] cards)
    {
        Clear();

        for (int i = 0; i < cards.Length; i++)
        {
            var cardObject = GameObject.Instantiate(_cardPrefab, Vector3.zero, Quaternion.identity, transform);

            var card = cardObject.GetComponent<Card>();
            card.Instantiate(cards[i]);
            card.Flip.ShowBack();

            var cardEvent = cardObject.GetComponent<CardEvent>();
            cardEvent.Id = i;
            cardEvent.OnEvent += OnCardEvent;

            _cards.Add(card);
        }

        _animator.Play("Start");
    }

    private void OnCardEvent(CardEvent arg1, CardEventArgs args2)
    {
        switch (args2.cardEventType)
        {
            case CardEventType.Click:
                CardClicked(args2.id);
                break;

            default:
                break;
        }
    }

    private void CardClicked(int id)
    {
        if (!_cards[id].Flip.IsFlipped)
        {
            _cards[id].Flip.ShowFront();
        }
        else
        {
            CardSystem.Instance.DrawSelectCard(id);
            Clear();
        }
    }
}

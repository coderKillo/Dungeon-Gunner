using System.Net.Mime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawUI : MonoBehaviour
{
    [SerializeField] private GameObject _cardPrefab;

    private CardSelector[] _cards;

    private void Awake()
    {
        _cards = new CardSelector[0];
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
    }

    public void Draw(CardSO[] cards)
    {
        Clear();

        _cards = new CardSelector[cards.Length];

        for (int i = 0; i < cards.Length; i++)
        {
            var cardObject = GameObject.Instantiate(_cardPrefab, Vector3.zero, Quaternion.identity, transform);

            var card = cardObject.GetComponent<Card>();
            card.Instantiate(cards[i]);

            var selector = cardObject.GetComponent<CardSelector>();
            selector.Id = i;
            selector.OnClick += CardSelected;

            _cards[i] = selector;
        }

    }

    private void CardSelected(int id)
    {
        _cards[id].Selected = !_cards[id].Selected;
    }
}

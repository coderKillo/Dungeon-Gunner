using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private GameObject _cardMiniPrefab;

    private List<Card> _cards;

    private void Awake()
    {
        _cards = new List<Card>();
    }

    void Start()
    {
        CardSystem.OnHandChanged += OnHandChanged;
    }

    public void Clear()
    {
        foreach (var card in _cards)
        {
            Destroy(card.gameObject);
        }

        _cards.Clear();
    }

    private void OnHandChanged(CardSO[] cards)
    {
        Clear();

        for (int i = 0; i < cards.Length; i++)
        {
            var cardObject = GameObject.Instantiate(_cardMiniPrefab, Vector3.zero, Quaternion.identity, transform);

            var card = cardObject.GetComponent<Card>();
            card.Instantiate(cards[i]);

            var cardEvent = cardObject.GetComponent<CardEvent>();
            cardEvent.Id = i;
            cardEvent.OnEvent += OnCardEvent;

            _cards.Add(card);
        }
    }

    private void OnCardEvent(CardEvent arg1, CardEventArgs arg2)
    {
        // TODO
    }
}

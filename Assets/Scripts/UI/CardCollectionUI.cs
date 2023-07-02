using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCollectionUI : MonoBehaviour
{
    [SerializeField] private CardDeckSO _cardDeck;
    [SerializeField] private Transform _scrollViewContent;
    [SerializeField] private GameObject _cardPrefab;

    void Start()
    {
        foreach (var card in _cardDeck.deck)
        {
            var cardObject = GameObject.Instantiate(_cardPrefab, Vector3.zero, Quaternion.identity, _scrollViewContent);
            var cardUI = cardObject.GetComponent<CardUI>();
            cardUI.Initialize(card, Guid.NewGuid(), 1, 1f);

            if (PlayerPrefs.HasKey(PrefKeys.CardKey(card)))
            {
                cardUI.GetComponent<CardFlip>().ShowFront();
            }
            else
            {
                cardUI.GetComponent<CardFlip>().ShowBack();
            }
        }
    }
}

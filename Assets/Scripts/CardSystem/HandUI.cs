using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class HandUI : MonoBehaviour
{
    [Space(10)]
    [Header("Reference")]
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private GameObject _cardMiniPrefab;
    [SerializeField] private Transform _handPreview;
    [SerializeField] private Transform _handGroup;

    private List<Card> _cards;
    private Card _cardPreview;

    private void Awake()
    {
        _cards = new List<Card>();

        var cardObject = GameObject.Instantiate(_cardPrefab, Vector3.zero, Quaternion.identity, _handPreview);
        _cardPreview = cardObject.GetComponent<Card>();
        cardObject.SetActive(false);
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
            var cardObject = GameObject.Instantiate(_cardMiniPrefab, Vector3.zero, Quaternion.identity, _handGroup);

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
        switch (arg2.cardEventType)
        {
            case CardEventType.PointerEnter:
                ShowPreviewCard(arg2.id);
                break;

            case CardEventType.PointerExit:
                HidePreviewCard();
                break;

            default:
                break;
        }
    }

    private void ShowPreviewCard(int id)
    {
        _cards[id].PointerEnterFeedback.PlayFeedbacks();

        _cardPreview.Instantiate(_cards[id].Details);
        _cardPreview.gameObject.SetActive(true);
    }

    private void HidePreviewCard()
    {
        _cardPreview.gameObject.SetActive(false);
    }
}

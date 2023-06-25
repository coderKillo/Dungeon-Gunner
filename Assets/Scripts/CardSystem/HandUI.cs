using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using DG.Tweening;

public class HandUI : MonoBehaviour
{
    [Space(10)]
    [Header("Reference")]
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private Transform _handGroup;
    [SerializeField] private CardHand _cardHand;
    [SerializeField] private CardSystemSettings _cardSystemSettings;

    private List<CardUI> _cards;

    private void Awake()
    {
        _cards = new List<CardUI>();
    }

    private void OnEnable()
    {
        _cardHand.OnCardUpdate += OnCardUpdate;
    }

    private void OnDisable()
    {
        _cardHand.OnCardUpdate -= OnCardUpdate;
    }

    void Start()
    {
        _handGroup.gameObject.SetActive(true);
    }

    private void OnCardUpdate(Card[] cards)
    {
        if (cards.Length < _cards.Count)
        {
            var removedCard = _cards[CardHand.ACTIVE_CARD_INDEX];
            removedCard.DestroyFeedback.PlayFeedbacks();
            _cards.Remove(removedCard);
        }

        for (int i = 0; i < _cards.Count; i++)
        {
            Destroy(_cards[i].gameObject);
        }

        _cards.Clear();

        foreach (var card in cards)
        {
            Add(card);
        }

        _cards[CardHand.ACTIVE_CARD_INDEX].ActivateFeedback.Initialization();
        _cards[CardHand.ACTIVE_CARD_INDEX].ActivateFeedback.PlayFeedbacks();
    }

    private void Add(Card card)
    {
        var cardObject = GameObject.Instantiate(_cardPrefab, Vector3.zero, Quaternion.identity, _handGroup);
        cardObject.transform.SetSiblingIndex(0);

        var cardUI = cardObject.GetComponent<CardUI>();

        cardUI.title.text = card.details.title;
        cardUI.icon.sprite = card.details.icon;
        cardUI.background.color = CardSystemSettings.GetColor(card.details.rarity);
        cardUI.id = card.id;
        cardUI.details = card.details;
        cardUI.setLevel(card.level);
        cardUI.setValue(card.value);
        cardUI.setDescription();

        _cards.Add(cardUI);
    }
}

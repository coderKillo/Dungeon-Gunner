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
            _cards[i].gameObject.SetActive(false);
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
        var cardUI = GetCardUIFromPool();
        cardUI.transform.SetSiblingIndex(0);
        cardUI.Initialize(card.details, card.id, card.level, card.value);
        cardUI.gameObject.SetActive(true);

        _cards.Add(cardUI);
    }


    #region POOL FUNCTIONS
    private CardUI GetCardUIFromPool()
    {
        for (int i = 0; i < _handGroup.childCount; i++)
        {
            var child = _handGroup.GetChild(i);
            if (!child.gameObject.activeInHierarchy)
            {
                return child.GetComponent<CardUI>();
            }
        }

        var cardObject = GameObject.Instantiate(_cardPrefab, Vector3.zero, Quaternion.identity, _handGroup);
        cardObject.SetActive(false);
        return cardObject.GetComponent<CardUI>();
    }
    #endregion
}

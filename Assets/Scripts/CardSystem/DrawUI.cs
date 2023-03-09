using System.Net.Mime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawUI : MonoBehaviour
{
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private CardDraw _cardDraw;
    [SerializeField] private CardSystemSettings _cardSystemSettings;

    private Animator _animator;
    private List<CardUI> _cards;

    private void Awake()
    {
        _cards = new List<CardUI>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        _cardDraw.OnDraw += Draw;
    }

    public void Clear()
    {
        foreach (var card in _cards)
        {
            card.DestroyFeedback.PlayFeedbacks();
        }

        _cards.Clear();
    }

    public void Draw(Card[] cards)
    {
        Clear();

        for (int i = 0; i < cards.Length; i++)
        {
            var cardObject = GameObject.Instantiate(_cardPrefab, Vector3.zero, Quaternion.identity, transform);

            var card = cardObject.GetComponent<CardUI>();
            card.title.text = cards[i].details.title;
            card.icon.sprite = cards[i].details.icon;
            card.background.color = _cardSystemSettings.GetColor(cards[i].details.rarity);
            card.id = cards[i].id;
            card.details = cards[i].details;
            card.setLevel(cards[i].level);
            card.setDescription();

            cardObject.GetComponent<CardFlip>().ShowBack();

            var cardEvent = cardObject.GetComponent<CardEvent>();
            cardEvent.Id = card.id;
            cardEvent.OnEvent += OnCardEvent;

            _cards.Add(card);
        }

        _animator.Play("Start");
    }

    private void OnCardEvent(CardEvent arg1, CardEventArgs args2)
    {
        var index = _cards.FindIndex((x) => x.id == args2.id);
        if (index < 0)
        {
            return;
        }

        switch (args2.cardEventType)
        {
            case CardEventType.ClickLeft:
                CardClicked(index);
                break;

            default:
                break;
        }
    }

    private void CardClicked(int index)
    {
        var cardFlip = _cards[index].gameObject.GetComponent<CardFlip>();

        if (!cardFlip.IsFlipped)
        {
            cardFlip.ShowFront();
        }
        else
        {
            _cards[index].SelectedFeedback.PlayFeedbacks();
            _cardDraw.CardSelected(index);

            foreach (var card in _cards)
            {
                card.setSelectable(false);
            }

            _cards.RemoveAt(index);
            _animator.Play("End");
        }
    }
}

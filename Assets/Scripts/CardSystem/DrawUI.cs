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
    [SerializeField] private CardRarityColor _rarityColor;

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

            var card = cardObject.GetComponent<CardUI>();
            card.title.text = cards[i].title;
            card.description.text = cards[i].description;
            card.icon.sprite = cards[i].icon;
            card.background.color = _rarityColor.GetColor(cards[i].rarity);

            cardObject.GetComponent<CardFlip>().ShowBack();

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
        var cardFlip = _cards[id].gameObject.GetComponent<CardFlip>();

        if (!cardFlip.IsFlipped)
        {
            cardFlip.ShowFront();
        }
        else
        {
            _cardDraw.CardSelected(id);
            Clear();
        }
    }
}

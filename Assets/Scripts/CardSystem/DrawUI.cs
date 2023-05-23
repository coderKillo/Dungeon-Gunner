using System.Net.Mime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawUI : MonoBehaviour
{
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private RectTransform _handBackground;
    [SerializeField] private RectTransform _handFullWarning;
    [SerializeField] private CardDraw _cardDraw;
    [SerializeField] private CardSystemSettings _cardSystemSettings;
    [SerializeField] private float _cardExitDuration = 1f;

    private Animator _animator;
    private List<CardUI> _cards;
    private CardDraw.State _currentState = CardDraw.State.Idle;

    private void Awake()
    {
        _cards = new List<CardUI>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        _cardDraw.OnCardChange += OnCardChange;
        _cardDraw.OnStateChange += OnStateChange;

        _handBackground.gameObject.SetActive(false);
        _handFullWarning.gameObject.SetActive(false);
    }

    public void Clear()
    {
        foreach (var card in _cards)
        {
            Destroy(card.gameObject);
        }

        _cards.Clear();
    }

    private void OnStateChange(CardDraw.State state)
    {
        Debug.Log(state);
        switch (state)
        {
            case CardDraw.State.Idle:
                CloseDraw();
                break;

            case CardDraw.State.EndDraw:
                StartCoroutine(EndDraw());
                break;

            case CardDraw.State.Draw:
                if (_currentState == CardDraw.State.Idle)
                {
                    StartDraw();
                }
                foreach (var card in _cards)
                {
                    card.setValue(1f);
                }
                _handFullWarning.gameObject.SetActive(false);
                break;

            case CardDraw.State.HandFull:
                foreach (var card in _cards)
                {
                    card.setValue(0f);
                }
                _handFullWarning.gameObject.SetActive(true);
                break;

            default:
                break;
        }

        _currentState = state;
    }

    private void OnCardChange(Card[] cards)
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
            card.setValue(cards[i].value);
            card.setDescription();

            cardObject.GetComponent<CardFlip>().ShowBack();

            var cardEvent = cardObject.GetComponent<CardEvent>();
            cardEvent.Id = card.id;
            cardEvent.OnEvent += OnCardEvent;

            _cards.Add(card);
        }
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
        else if (_currentState != CardDraw.State.HandFull)
        {
            _cards[index].SelectedFeedback.PlayFeedbacks();
            _cards[index].setSelectable(false);
            _cards.RemoveAt(index);

            _cardDraw.CardSelected(index);
        }
    }

    public void ExitDraw()
    {
        _cardDraw.Done();
    }

    private IEnumerator EndDraw()
    {
        foreach (var card in _cards)
        {
            card.setSelectable(false);
        }

        _animator.Play("End");

        yield return new WaitForSecondsRealtime(_cardExitDuration);

        _cardDraw.EndDrawDone();
    }

    private void CloseDraw()
    {
        _handBackground.gameObject.SetActive(false);
        _handFullWarning.gameObject.SetActive(false);
    }

    private void StartDraw()
    {
        _animator.Play("Start");
        _handBackground.gameObject.SetActive(true);
    }
}

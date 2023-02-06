using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Sirenix.OdinInspector;

// TODO: add state machine
public class CardSystem : SingletonAbstract<CardSystem>
{
    public enum State
    {
        Show,
        Hide,
        Draw,
        RemoveCard
    };

    [SerializeField] private CardDeckSO playerDeck;

    private List<CardSO> _deck = new List<CardSO>();

    private CardHand _cardHand;
    private CardDraw _cardDraw;

    private State _currentState;

    protected override void Awake()
    {
        base.Awake();

        CreateDeck();

        _cardDraw = GetComponent<CardDraw>();
        _cardHand = GetComponent<CardHand>();

        _cardDraw.OnCardSelected += CardDraw_OnCardSelected;
        _cardHand.OnCardSelected += CardHand_OnCardSelected;
    }

    private void Start()
    {
        SetState(State.Hide);
    }

    private void CreateDeck()
    {
        foreach (var card in playerDeck.deck)
        {
            _deck.Add(card);
        }
    }

    #region Handle Events
    private void CardHand_OnCardSelected(CardSO card)
    {
        switch (_currentState)
        {
            case State.RemoveCard:
                RemoveCard(card);
                break;

            default:
                ActivateCard(card);
                break;
        }
    }

    private void CardDraw_OnCardSelected(CardSO card)
    {
        switch (_currentState)
        {
            case State.Draw:
                AddCard(card);
                break;

            default:
                break;
        }
    }
    #endregion

    #region Public Interface
    [Button("Draw Card")]
    public void Draw()
    {
        SetState(State.Draw);
    }

    [Button("Show")]
    public void Show()
    {
        SetState(State.Show);
    }

    [Button("Hide")]
    public void Hide()
    {
        SetState(State.Hide);
    }
    #endregion

    #region StateMachine
    void SetState(State state)
    {
        if (state == _currentState)
        {
            return;
        }

        UpdateState(_currentState, state);

        _currentState = state;
    }

    private void UpdateState(State previousState, State newState)
    {
        switch (newState)
        {
            case State.Show:
                _cardHand.Show(true);
                break;
            case State.Hide:
                _cardHand.Show(false);
                break;
            case State.Draw:
                _cardHand.Show(true);
                _cardDraw.Draw(_deck.ToArray());
                break;
            case State.RemoveCard:
                // TODO: display text to player
                Debug.Log("Your Hand is Full! Select a Card to be removed.");
                break;
        }
    }

    private void ActivateCard(CardSO card)
    {
        Card.Action(card);

        _cardHand.Remove(card);

        SetState(State.Hide);
    }

    private void RemoveCard(CardSO card)
    {
        _cardHand.Remove(card);

        SetState(State.Hide);
    }

    private void AddCard(CardSO card)
    {
        _cardHand.Add(card);

        if (_cardHand.Full())
        {
            SetState(State.RemoveCard);
        }
        else
        {
            SetState(State.Hide);
        }
    }
    #endregion
}

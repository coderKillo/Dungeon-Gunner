using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Sirenix.OdinInspector;

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

    private List<Card> _deck = new List<Card>();

    private CardHand _cardHand;
    private CardDraw _cardDraw;

    private State _currentState;

    private Player _player;

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
        _player = FindObjectOfType<Player>(); // FIXME: find better solution to find player

        SetState(State.Hide);
    }

    private void CreateDeck()
    {
        for (int i = 0; i < playerDeck.deck.Length; i++)
        {
            var card = new Card();
            card.id = i;
            card.details = playerDeck.deck[i];
            card.level = UnityEngine.Random.Range(1, 6);

            _deck.Add(card);
        }
    }

    #region Handle Events
    private void CardHand_OnCardSelected(Card[] cards)
    {
        switch (_currentState)
        {
            case State.RemoveCard:
                if (cards.Length == 1)
                {
                    RemoveCard(cards[0]);
                }
                break;

            default:
                switch (cards.Length)
                {
                    case 1:
                        ActivateCard(cards[0]);
                        break;
                    case 2:
                        LevelUpCard(cards[0], cards[1]);
                        break;
                    case 3:
                        CombineCards(cards[0], cards[1], cards[2]);
                        break;

                }
                break;
        }
    }

    private void CardDraw_OnCardSelected(Card card)
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

    private void ActivateCard(Card card)
    {
        card.Action(_player);

        _cardHand.Remove(card);

        SetState(State.Hide);
    }

    private void CombineCards(Card card1, Card card2, Card card3)
    {
        if (card1.details.rarity != card2.details.rarity || card1.details.rarity != card3.details.rarity)
        {
            return;
        }

        var rarity = card1.details.rarity + 1;

        var specificRarityCards = _deck.FindAll((x) => x.details.rarity == rarity);

        if (specificRarityCards.Count <= 0)
        {
            Debug.Log("no card rarity in deck: " + rarity);
            return;
        }

        var randomCard = specificRarityCards[UnityEngine.Random.Range(0, specificRarityCards.Count - 1)];

        _cardHand.Remove(card1);
        _cardHand.Remove(card2);
        _cardHand.Remove(card3);

        AddCard(randomCard);
    }

    private void LevelUpCard(Card card1, Card card2)
    {
        if (card1.details != card2.details)
        {
            return;
        }

        var level = Math.Max(card1.level, card2.level) + 1;

        var card = new Card();
        card.id = card1.id;
        card.details = card1.details;
        card.level = level;

        _cardHand.Remove(card1);
        _cardHand.Remove(card2);

        AddCard(card);
    }

    private void RemoveCard(Card card)
    {
        _cardHand.Remove(card);

        SetState(State.Show);
    }

    private void AddCard(Card card)
    {
        _cardHand.Add(card);

        if (_cardHand.Full())
        {
            SetState(State.RemoveCard);
        }
        else
        {
            SetState(State.Show);
        }
    }
    #endregion
}

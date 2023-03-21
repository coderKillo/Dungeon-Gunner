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

    private CardHand _cardHand;
    private CardDraw _cardDraw;

    private State _currentState;

    protected override void Awake()
    {
        base.Awake();

        _cardDraw = GetComponent<CardDraw>();
        _cardHand = GetComponent<CardHand>();

        _cardDraw.OnCardSelected += CardDraw_OnCardSelected;
        _cardHand.OnCardSelected += CardHand_OnCardSelected;

    }

    private void Start()
    {
        SetState(State.Hide);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Show();
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
    public void Draw(CardRarity rarity = CardRarity.Common)
    {
        _cardDraw.Priority = rarity;
        SetState(State.Draw);
    }

    [Button("Show/Hide")]
    public void Show()
    {
        if (!CardSystem.Instance.IsVisiable())
        {
            SetState(State.Show);
        }
        else
        {
            SetState(State.Hide);
        }
    }

    public bool IsVisiable()
    {
        return _currentState != State.Hide;
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
                PauseGame();
                _cardHand.Show(true);
                break;
            case State.Hide:
                _cardHand.Show(false);
                ResumeGame();
                break;
            case State.Draw:
                PauseGame();
                _cardHand.Show(true);
                _cardDraw.Draw(playerDeck.deck.ToArray());
                break;
            case State.RemoveCard:
                GameManager.Instance.DisplayMessage.DisplayText("Your Hand is Full! Select a Card to be removed.", 3f, Color.white, 0.5f, 1f);
                break;
        }
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        GameManager.Instance.Player.EnablePlayer(true);
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        GameManager.Instance.Player.EnablePlayer(false);
    }

    private void ActivateCard(Card card)
    {
        card.Action(GameManager.Instance.Player);

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

        var specificRarityCards = playerDeck.deck.FindAll((x) => x.rarity == rarity);

        if (specificRarityCards.Count <= 0)
        {
            Debug.Log("no card rarity in deck: " + rarity);
            return;
        }

        var card = new Card();
        card.details = specificRarityCards[UnityEngine.Random.Range(0, specificRarityCards.Count - 1)];
        card.level = Math.Min(Math.Min(card1.level, card2.level), card3.level);
        card.id = Guid.NewGuid();

        _cardHand.Remove(card1);
        _cardHand.Remove(card2);
        _cardHand.Remove(card3);

        AddCard(card);
    }

    private void LevelUpCard(Card card1, Card card2)
    {
        if (card1.details != card2.details)
        {
            return;
        }

        var level = Math.Max(card1.level, card2.level) + 1;

        var card = new Card();
        card.id = Guid.NewGuid();
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
            SetState(State.Hide);
        }
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Sirenix.OdinInspector;

public class CardSystem : SingletonAbstract<CardSystem>
{
    [SerializeField] private CardDeckSO playerDeck;

    private CardHand _cardHand;
    public CardHand Hand { get { return _cardHand; } }
    private CardDraw _cardDraw;

    protected override void Awake()
    {
        base.Awake();

        _cardDraw = GetComponent<CardDraw>();
        _cardHand = GetComponent<CardHand>();

        _cardDraw.OnStateChange += CardDraw_OnStateChange;
    }

    #region Handle Events

    private void CardDraw_OnStateChange(CardDraw.State state)
    {
        switch (state)
        {
            case CardDraw.State.Draw:
                PauseGame();
                break;

            case CardDraw.State.Idle:
                ResumeGame();
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
        _cardDraw.Draw(rarity);
    }

    public CardSO[] CardDeck()
    {
        return playerDeck.deck.ToArray();
    }
    #endregion

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

}

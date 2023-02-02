using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Sirenix.OdinInspector;

public class CardSystem : SingletonAbstract<CardSystem>
{
    [SerializeField] private CardDeckSO playerDeck;
    [SerializeField] private int firstDraw;
    [SerializeField] private int handMax = 5;

    private List<CardSO> _hand = new List<CardSO>();
    private List<CardSO> _draw = new List<CardSO>();
    private List<CardSO> _deck = new List<CardSO>();

    static public Action<CardSO[]> OnDraw;
    static public Action<CardSO[]> OnHandChanged;
    static public Action<Boolean> OnShowHand;

    protected override void Awake()
    {
        base.Awake();

        foreach (var card in playerDeck.deck)
        {
            _deck.Add(card);
        }
    }

    private void Start()
    {
        for (int i = 0; i < firstDraw; i++)
        {
            Draw();
        }
    }

    [Button("Draw Card")]
    public void Draw()
    {
        _draw.Clear();

        for (int i = 0; i < Settings.drawSize; i++)
        {
            _draw.Add(_deck[UnityEngine.Random.Range(0, _deck.Count)]);
        }

        OnDraw?.Invoke(_draw.ToArray());
    }

    [Button("Show")]
    public void Show()
    {
        OnShowHand(true);
    }

    [Button("Hide")]
    public void Hide()
    {
        OnShowHand(false);
    }

    public void DrawSelectCard(int id)
    {
        AddCardToHand(_draw[id]);

        _draw.Clear();
    }

    private void AddCardToHand(CardSO card)
    {
        if (_hand.Count >= handMax)
        {
            return;
        }

        _hand.Add(card);
        OnHandChanged?.Invoke(_hand.ToArray());
    }
}

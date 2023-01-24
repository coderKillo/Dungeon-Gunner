using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Sirenix.OdinInspector;

public class CardSystem : MonoBehaviour
{
    [SerializeField] private CardDeckSO playerDeck;
    [SerializeField] private int firstDraw;

    private List<CardSO> _hand = new List<CardSO>();
    private List<CardSO> _draw = new List<CardSO>();
    private List<CardSO> _deck = new List<CardSO>();
    private List<CardSO> _selected = new List<CardSO>();

    static public Action<CardSO[]> OnDraw;

    private void Awake()
    {
        foreach (var card in playerDeck.deck)
        {
            _deck.Add(card);
        }
    }

    private void Start()
    {
        for (int i = 0; i < firstDraw; i++)
        {
            draw();
        }
    }

    [Button("Draw Card")]
    public void draw()
    {
        _draw.Clear();

        for (int i = 0; i < Settings.drawSize; i++)
        {
            _draw.Add(_deck[UnityEngine.Random.Range(0, _deck.Count)]);
        }

        OnDraw?.Invoke(_draw.ToArray());
    }

    public void select(CardSO card)
    {
        _selected.Add(card);
    }

}

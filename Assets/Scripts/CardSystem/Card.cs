using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card
{
    private CardSO _details;
    public CardSO Details { get { return _details; } }

    public void Instantiate(CardSO card)
    {
        _details = card;
    }

    public void Action()
    {

    }
}


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card
{
    public CardSO details;
    public int id;

    static public void Action(CardSO card)
    {
        Debug.Log("Activate Card: " + card.title);
    }
}


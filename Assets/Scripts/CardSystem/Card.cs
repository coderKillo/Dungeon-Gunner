using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card
{
    static public void Action(CardSO card)
    {
        Debug.Log("Activate Card: " + card.title);
    }
}


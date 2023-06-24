using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCardToHand : MonoBehaviour
{
    [SerializeField] private List<CardSO> _cardsToAdd = new List<CardSO>();

    void Start()
    {
        if (CardSystem.Instance == null)
        {
            Debug.LogWarning("Can not add cards, CardSystem not init yet!");
            return;
        }

        foreach (var card in _cardsToAdd)
        {
            CardSystem.Instance.Hand.Add(card);
        }
    }

}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card
{
    public CardSO details;
    public int id;
    public int level = 1;

    public void Action(Player player)
    {
        // TODO: add other actions
        switch (details.action)
        {
            case CardAction.Heal:
                // TODO: add level
                player.health.Heal(details.healAmount);
                break;

            case CardAction.Shield:
                break;

            case CardAction.PowerUp:
                break;

            case CardAction.Ammo:
                break;

            case CardAction.AddWeapon:
                break;

            default:
                break;
        }

    }
}


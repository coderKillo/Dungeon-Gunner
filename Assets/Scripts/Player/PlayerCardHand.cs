using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerCardHand : MonoBehaviour
{
    private Player _player;
    private Card _lastSelected;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        _player.weaponFiredEvent.OnWeaponFired += WeaponFiredEvent_OnWeaponFired;
        _player.fireWeaponEvent.OnFireWeapon += FireWeaponEvent_OnFireWeapon;
    }

    private void OnDisable()
    {
        _player.weaponFiredEvent.OnWeaponFired -= WeaponFiredEvent_OnWeaponFired;
        _player.fireWeaponEvent.OnFireWeapon -= FireWeaponEvent_OnFireWeapon;
    }

    private void FireWeaponEvent_OnFireWeapon(FireWeaponEvent @event, FireWeaponEventArgs args)
    {
        if (IsCurrentCardWeapon())
        {
            // Ignore because is handled in WeaponFired
            return;
        }

        // only activate on last frame of fire pressed
        if (!args.fire && args.fireLastFrame)
        {
            ActiveCurrentCard();
        }

    }

    private void WeaponFiredEvent_OnWeaponFired(WeaponFiredEvent @event, WeaponFiredEventArgs args)
    {
        ActiveCurrentCard();
    }

    public void Start()
    {
        var card = new Card();
        card.id = Guid.NewGuid();
        card.level = 1;
        card.details = _player.playerDetails.startingWeapon;

        CardSystem.Instance.Hand.Add(card);
    }

    public void NextCard()
    {
        if (!CardSystem.Instance.Hand.HasCard())
        {
            return;
        }

        CardSystem.Instance.Hand.Next();
    }

    public void PreviousCard()
    {
        if (!CardSystem.Instance.Hand.HasCard())
        {
            return;
        }

        CardSystem.Instance.Hand.Previous();
    }

    public void ActiveCurrentCard()
    {
        if (!CardSystem.Instance.Hand.HasCard())
        {
            return;
        }

        CardSystem.Instance.Hand.ActivateCurrentCard();
    }

    public void SacrificeCurrentCard()
    {
        if (!CardSystem.Instance.Hand.HasCard())
        {
            return;
        }

        var card = CardSystem.Instance.Hand.CurrentActive();

        if (card.details == _player.playerDetails.startingWeapon)
        {
            return;
        }

        CardSystem.Instance.Hand.Remove(card);

        StaticEventHandler.CallPointScoredEvent(GetPointFromCard(card));
    }

    private int GetPointFromCard(Card card)
    {
        var points = card.level;

        switch (card.details.rarity)
        {
            case CardRarity.Common:
                points *= 100;
                break;
            case CardRarity.Rare:
                points *= 500;
                break;
            case CardRarity.Epic:
                points *= 1000;
                break;
            case CardRarity.Legendary:
                points *= 2000;
                break;

            default:
                break;
        }

        return Mathf.RoundToInt(points * card.value);
    }

    private bool IsCurrentCardWeapon()
    {
        return CardSystem.Instance.Hand.CurrentActive().details.action == CardAction.AddWeapon;
    }
}

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CardHand : MonoBehaviour
{
    [Header("SOUND EFFECTS")]
    [SerializeField] private SoundEffectSO _addCardSound;
    [SerializeField] private SoundEffectSO _removeCardSound;
    [SerializeField] private SoundEffectSO _switchCardSound;

    public static int ACTIVE_CARD_INDEX = 0;

    private List<Card> _hand = new List<Card>();

    [HideInInspector] public Action<Card[]> OnCardUpdate;

    private Card _lastSelected;

    public void Add(Card card)
    {
        _hand.Insert(0, card);

        if (card.details.action == CardAction.AddWeapon)
        {
            GameManager.Instance.Player.AddWeaponToPlayer(card.details.weapon);
        }

        SoundEffectManager.Instance.PlaySoundEffect(_addCardSound);

        UpdateHand();
    }

    public void Remove(Card card)
    {
        _hand.Remove(card);

        if (card.details.action == CardAction.AddWeapon)
        {
            GameManager.Instance.Player.RemoveWeaponFromPlayer(card.details.weapon);
        }

        SoundEffectManager.Instance.PlaySoundEffect(_removeCardSound);

        UpdateHand();
    }

    public void Next()
    {
        if (_hand.Count <= 1)
        {
            return;
        }

        var firstCard = _hand[0];
        _hand.RemoveAt(0);
        _hand.Add(firstCard);

        SoundEffectManager.Instance.PlaySoundEffect(_switchCardSound);

        UpdateHand();
    }

    public void Previous()
    {
        if (_hand.Count <= 1)
        {
            return;
        }

        var lastCard = _hand[_hand.Count - 1];
        _hand.RemoveAt(_hand.Count - 1);
        _hand.Insert(0, lastCard);

        SoundEffectManager.Instance.PlaySoundEffect(_switchCardSound);

        UpdateHand();
    }

    public void ActivateCurrentCard()
    {
        if (_hand.Count <= 0)
        {
            return;
        }

        var card = CurrentActive();

        card.Activate(GameManager.Instance.Player);
        if (card.value <= 0f)
        {
            Remove(card);
        }

        UpdateHand();
    }

    public Card CurrentActive()
    {
        return _hand[ACTIVE_CARD_INDEX];
    }

    public bool HasCard()
    {
        return _hand.Count > 0;
    }

    public bool Full() { return _hand.Count > Settings.cardHandSize; }

    private void UpdateHand()
    {
        if (HasCard() && _lastSelected != CurrentActive())
        {
            if (_lastSelected != null)
            {
                _lastSelected.Deselect(GameManager.Instance.Player);
            }

            CurrentActive().Select(GameManager.Instance.Player);

            _lastSelected = CurrentActive();
        }

        OnCardUpdate?.Invoke(_hand.ToArray());
    }
}

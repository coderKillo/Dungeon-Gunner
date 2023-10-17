using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

public class CardSystemSettings : SerializedMonoBehaviour
{
    [Space(10)]
    [Header("Rarity Ratio")]
    [SerializeField] private Dictionary<CardRarity, int> _rarityRatio;
    [SerializeField] private float _rationIncreasePerLevel = 1.012f;

    private CardDeck _cardDeck;

    private float _currentRatioIncrease = 1f;

    private void Awake()
    {
        _cardDeck = GetComponent<CardDeck>();
    }

    private void OnEnable()
    {
        CardSystemLevel.OnLevelChange += CardSystemLevel_OnLevelChange;
    }

    private void OnDisable()
    {
        CardSystemLevel.OnLevelChange -= CardSystemLevel_OnLevelChange;
    }

    private void CardSystemLevel_OnLevelChange(int level)
    {
        _currentRatioIncrease = Mathf.Pow(_rationIncreasePerLevel, level);
    }

    static public Color GetColor(CardRarity rarity)
    {
        return GameResources.Instance.rarityColorLookup.GetValueOrDefault(rarity, Color.gray);
    }

    public CardSO PickRandomCard()
    {
        var roll = Random.Range(0, 100);
        var selectedRarity = CardRarity.Common;
        var sortedRarityRatio = from entry in _rarityRatio orderby entry.Value descending select entry;

        foreach (var item in sortedRarityRatio)
        {
            if (roll <= (item.Value * _currentRatioIncrease))
            {
                selectedRarity = item.Key;
            }
        }

        return PickRandomCardWithSpecificRarity(selectedRarity);
    }

    public CardSO PickRandomCardWithSpecificRarity(CardRarity rarity)
    {
        return _cardDeck.PickCard(rarity);
    }
}
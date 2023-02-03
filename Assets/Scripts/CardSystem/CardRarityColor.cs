using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CardRarityColor : SerializedMonoBehaviour
{
    [Space(10)]
    [Header("Rarity Color")]
    [SerializeField] private Dictionary<CardRarity, Color> _rarityColorLookup;

    public Color GetColor(CardRarity rarity)
    {
        return _rarityColorLookup.GetValueOrDefault(rarity, Color.white);
    }
}

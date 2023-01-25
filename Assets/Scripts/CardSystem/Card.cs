using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;

public class Card : SerializedMonoBehaviour
{
    [Space(10)]
    [Header("References")]
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private Image _icon;
    [SerializeField] private Image _background;

    [Space(10)]
    [Header("Rarity Color")]
    [SerializeField] private Dictionary<CardRarity, Color> _rarityColorLookup;

    private CardFlip _cardFlip;
    public CardFlip Flip { get { return _cardFlip; } }

    private void Awake()
    {
        _cardFlip = GetComponent<CardFlip>();
    }

    public void Instantiate(CardSO card)
    {
        _title.text = card.title;
        _description.text = card.description;
        _icon.sprite = card.sprite;
        _background.color = _rarityColorLookup.GetValueOrDefault(card.rarity, Color.white);
    }
}


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;
using MoreMountains.Feedbacks;

public class Card : SerializedMonoBehaviour
{
    [Space(10)]
    [Header("References")]
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private Image _icon;
    [SerializeField] private Image _background;

    [Space(10)]
    [Header("Feedbacks")]
    [SerializeField] private MMF_Player _pointerEnterFeedback;
    public MMF_Player PointerEnterFeedback { get { return _pointerEnterFeedback; } }


    [Space(10)]
    [Header("Rarity Color")]
    [SerializeField] private Dictionary<CardRarity, Color> _rarityColorLookup;

    private CardSO _details;
    public CardSO Details { get { return _details; } }

    private CardFlip _cardFlip;
    public CardFlip Flip { get { return _cardFlip; } }

    private void Awake()
    {
        _cardFlip = GetComponent<CardFlip>();
    }

    public void Instantiate(CardSO card)
    {
        _details = card;

        if (_title != null)
        {
            _title.text = card.title;
        }
        if (_description != null)
        {
            _description.text = card.description;
        }
        if (_icon != null)
        {
            _icon.sprite = card.sprite;
        }
        if (_background != null)
        {
            _background.color = _rarityColorLookup.GetValueOrDefault(card.rarity, Color.white);
        }
    }
}


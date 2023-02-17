using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using DG.Tweening;

public class HandUI : MonoBehaviour
{
    [Space(10)]
    [Header("Reference")]
    [SerializeField] private GameObject _cardPreviewPrefab;
    [SerializeField] private GameObject _cardMiniPrefab;
    [SerializeField] private Transform _handPreview;
    [SerializeField] private Transform _handGroup;
    [SerializeField] private Transform _handBackground;
    [SerializeField] private CardHand _cardHand;
    [SerializeField] private CardRarityColor _rarityColor;

    [Space(10)]
    [Header("Animation")]
    [SerializeField] private float _floatInTime;
    [SerializeField] private float _floatInDistance;

    private List<CardUI> _cards;
    private CardUI _cardPreview;
    private float _hideTimer = 0f;
    private bool _show = false;

    private void Awake()
    {
        _cards = new List<CardUI>();

        var cardObject = GameObject.Instantiate(_cardPreviewPrefab, Vector3.zero, Quaternion.identity, _handPreview);
        _cardPreview = cardObject.GetComponent<CardUI>();
        cardObject.SetActive(_show);
    }

    void Start()
    {
        _cardHand.OnCardAdd += OnCardAdd;
        _cardHand.OnCardRemove += OnCardRemove;
        _cardHand.OnShow += OnShowHand;
    }

    private void Update()
    {
        if (_hideTimer > 0f)
        {
            _hideTimer -= Time.deltaTime;
        }
    }

    private void OnCardRemove(Card card)
    {
        var result = _cards.Find(x => x.id == card.id);

        if (result == null)
        {
            return;
        }

        result.DestroyFeedback.PlayFeedbacks();
        _hideTimer = result.DestroyFeedback.TotalDuration;

        _cards.Remove(result);
        for (int i = 0; i < _cards.Count; i++)
        {
            _cards[i].GetComponent<CardEvent>().Id = i;
        }

        HidePreviewCard();
    }

    private void OnCardAdd(Card card)
    {
        var cardObject = GameObject.Instantiate(_cardMiniPrefab, Vector3.zero, Quaternion.identity, _handGroup);

        var cardUI = cardObject.GetComponent<CardUI>();
        cardUI.icon.sprite = card.details.iconMini;
        cardUI.background.color = _rarityColor.GetColor(card.details.rarity);
        cardUI.details = card.details;
        cardUI.id = card.id;
        cardUI.level = card.level;

        cardUI.setLevel(card.level);

        cardUI.StartFeedback.PlayFeedbacks();

        _hideTimer = cardUI.StartFeedback.TotalDuration;

        var cardEvent = cardObject.GetComponent<CardEvent>();
        cardEvent.Id = _cards.Count;
        cardEvent.OnEvent += OnCardEvent;

        _cards.Add(cardUI);
    }

    private void OnShowHand(bool show)
    {
        if (_show == show)
        {
            return;
        }

        _show = show;

        if (show)
        {
            _handBackground.gameObject.SetActive(true);
            _handGroup.gameObject.SetActive(true);

            var origin = _handGroup.localPosition;
            _handGroup.localPosition += new Vector3(0f, _floatInDistance, 0f);
            _handGroup.DOLocalMoveY(origin.y, _floatInTime).SetUpdate(true);
        }
        else
        {
            if (_hideTimer > 0)
            {
                Invoke(nameof(Hide), _hideTimer);
            }
            else
            {
                Hide();
            }
        }
    }

    private void Hide()
    {
        DeselectAll();

        _handBackground.gameObject.SetActive(false);

        var origin = _handGroup.localPosition;
        _handGroup.DOLocalMoveY(origin.y + _floatInDistance, _floatInTime).SetUpdate(true).OnComplete(() =>
        {
            _handGroup.gameObject.SetActive(false);
            _handGroup.localPosition = origin;
        });
    }

    private void OnCardEvent(CardEvent arg1, CardEventArgs arg2)
    {
        switch (arg2.cardEventType)
        {
            case CardEventType.PointerEnter:
                ShowPreviewCard(arg2.id);
                break;

            case CardEventType.PointerExit:
                HidePreviewCard();
                break;

            case CardEventType.ClickLeft:
                SelectCard(arg2.id);
                CardClicked();
                DeselectAll();
                break;

            case CardEventType.ClickRight:
                ToggleSelection(arg2.id);
                break;

            default:
                break;
        }
    }

    private void ToggleSelection(int index)
    {
        if (_cards[index].selected)
        {
            DeselectCard(index);
        }
        else
        {
            SelectCard(index);
        }
    }

    private void SelectCard(int index)
    {
        _cards[index].selected = true;
        _cards[index].selectBorder.gameObject.SetActive(true);
    }

    private void DeselectAll()
    {
        for (int i = 0; i < _cards.Count; i++)
        {
            DeselectCard(i);
        }
    }

    private void DeselectCard(int index)
    {
        _cards[index].selected = false;
        _cards[index].selectBorder.gameObject.SetActive(false);
    }

    private void CardClicked()
    {
        List<int> indexes = new List<int>();

        for (int i = 0; i < _cards.Count; i++)
        {
            if (_cards[i].selected)
            {
                indexes.Add(i);
            }
        }

        _cardHand.CardSelected(indexes.ToArray());
    }

    private void ShowPreviewCard(int index)
    {
        _cards[index].PointerEnterFeedback.PlayFeedbacks();

        _cardPreview.title.text = _cards[index].details.title;
        _cardPreview.description.text = _cards[index].details.description;
        _cardPreview.icon.sprite = _cards[index].details.icon;
        _cardPreview.background.color = _rarityColor.GetColor(_cards[index].details.rarity);
        _cardPreview.setLevel(_cards[index].level);

        _cardPreview.gameObject.SetActive(true);
    }

    private void HidePreviewCard()
    {
        _cardPreview.gameObject.SetActive(false);
    }
}

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
    [SerializeField] private CardUI _cardPreview;
    [SerializeField] private Transform _handGroup;
    [SerializeField] private Transform _handBackground;
    [SerializeField] private CardHand _cardHand;
    [SerializeField] private CardSystemSettings _cardSystemSettings;

    [Space(10)]
    [Header("Animation")]
    [SerializeField] private float _floatInTime;
    [SerializeField] private float _floatInDistance;

    private List<CardUI> _cards;
    private float _hideTimer = 0f;
    private bool _show = false;
    private Vector3 _handGroupOrigin;
    private Coroutine _hideCoroutine;

    private void Awake()
    {
        _cards = new List<CardUI>();

        _cardPreview.gameObject.SetActive(_show);
    }

    void Start()
    {
        _cardHand.OnCardAdd += OnCardAdd;
        _cardHand.OnCardRemove += OnCardRemove;
        _cardHand.OnShow += OnShowHand;

        _handGroupOrigin = _handGroup.localPosition;
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
        SetHideTimer(result.DestroyFeedback.TotalDuration);

        _cards.Remove(result);

        HidePreviewCard();
    }

    private void OnCardAdd(Card card)
    {
        var cardObject = GameObject.Instantiate(_cardMiniPrefab, Vector3.zero, Quaternion.identity, _handGroup);

        var cardUI = cardObject.GetComponent<CardUI>();
        cardUI.icon.sprite = card.details.iconMini;
        cardUI.background.color = _cardSystemSettings.GetColor(card.details.rarity);
        cardUI.details = card.details;
        cardUI.id = card.id;
        cardUI.level = card.level;

        cardUI.setLevel(card.level);

        cardUI.StartFeedback.PlayFeedbacks();

        SetHideTimer(cardUI.StartFeedback.TotalDuration);

        var cardEvent = cardObject.GetComponent<CardEvent>();
        cardEvent.Id = cardUI.id;
        cardEvent.OnEvent += OnCardEvent;

        _cards.Add(cardUI);
    }

    private void OnShowHand(bool show)
    {
        if (_show == show)
        {
            return;
        }

        foreach (var card in _cards)
        {
            card.setSelectable(show);
        }

        _show = show;

        if (_hideCoroutine != null)
        {
            StopCoroutine(_hideCoroutine);
        }

        if (show)
        {
            _handBackground.gameObject.SetActive(true);
            _handGroup.gameObject.SetActive(true);

            _handGroup.localPosition += new Vector3(0f, _floatInDistance, 0f);
            _handGroup.DOLocalMoveY(_handGroupOrigin.y, _floatInTime).SetUpdate(true);
        }
        else
        {
            _hideCoroutine = StartCoroutine(Hide());
        }
    }

    private IEnumerator Hide()
    {
        _handBackground.gameObject.SetActive(false);

        DeselectAll();

        yield return new WaitForSeconds(_hideTimer + 0.1f);

        var origin = _handGroup.localPosition;
        _handGroup.DOLocalMoveY(origin.y + _floatInDistance, _floatInTime).SetUpdate(true).OnComplete(() =>
        {
            _handGroup.gameObject.SetActive(_show);
            _handGroup.localPosition = _handGroupOrigin;
        });
    }

    private void SetHideTimer(float duration)
    {
        if (duration > _hideTimer)
        {
            _hideTimer = duration;
        }
    }

    private void OnCardEvent(CardEvent arg1, CardEventArgs arg2)
    {
        var index = _cards.FindIndex((x) => x.id == arg2.id);
        if (index < 0)
        {
            return;
        }

        switch (arg2.cardEventType)
        {
            case CardEventType.PointerEnter:
                ShowPreviewCard(index);
                break;

            case CardEventType.PointerExit:
                HidePreviewCard();
                break;

            case CardEventType.ClickLeft:
                SelectCard(index);
                CardClicked();
                DeselectAll();
                break;

            case CardEventType.ClickRight:
                ToggleSelection(index);
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
        _cardPreview.icon.sprite = _cards[index].details.icon;
        _cardPreview.background.color = _cardSystemSettings.GetColor(_cards[index].details.rarity);
        _cardPreview.details = _cards[index].details;
        _cardPreview.setLevel(_cards[index].level);
        _cardPreview.setDescription();

        _cardPreview.gameObject.SetActive(true);
    }

    private void HidePreviewCard()
    {
        _cardPreview.gameObject.SetActive(false);
    }
}

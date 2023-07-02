using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;

public class CardUI : MonoBehaviour
{
    [Space(10)]
    [Header("References")]
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public Image icon;
    public Image background;
    public Image backgroundGrey;
    public Transform disabledOverlay;
    public List<Image> levelIcons;
    public Transform selectBorder;
    public Image[] raycastTargets;

    [Space(10)]
    [Header("Material")]
    [SerializeField] private Material _cardLevelOn;
    [SerializeField] private Material _cardLevelOff;

    [Space(10)]
    [Header("Feedbacks")]
    [SerializeField] private MMF_Player _pointerEnterFeedback;
    public MMF_Player PointerEnterFeedback { get { return _pointerEnterFeedback; } }

    [SerializeField] private MMF_Player _destroyFeedback;
    public MMF_Player DestroyFeedback { get { return _destroyFeedback; } }

    [SerializeField] private MMF_Player _selectedFeedback;
    public MMF_Player SelectedFeedback { get { return _selectedFeedback; } }

    [SerializeField] private MMF_Player _startFeedback;
    public MMF_Player StartFeedback { get { return _startFeedback; } }

    [SerializeField] private MMF_Player _activateFeedback;
    public MMF_Player ActivateFeedback { get { return _activateFeedback; } }

    [SerializeField] private MMF_Player _newCardFeedback;
    public MMF_Player NewCardFeedback { get { return _newCardFeedback; } }

    [HideInInspector] public CardSO details;
    [HideInInspector] public Guid id;
    [HideInInspector] public int level;
    [HideInInspector] public bool selected = false;

    private void Start()
    {
        NewCardFeedback.Initialization();
    }

    public void Initialize(CardSO details, Guid id, int level, float value)
    {
        this.title.text = details.title;
        this.icon.sprite = details.icon;
        this.icon.rectTransform.sizeDelta = details.iconSize;
        this.background.color = CardSystemSettings.GetColor(details.rarity);
        this.id = id;
        this.details = details;

        setLevel(level);
        setValue(value);
        setDescription();
    }

    public void setDescription()
    {
        switch (details.action)
        {
            case CardAction.Heal:
                description.text = details.description.Replace("$", "" + details.healAmount * level);
                break;

            case CardAction.Shield:
                description.text = details.description.Replace("$", "" + details.shieldAmount * level);
                break;

            case CardAction.PowerUp:
                var power = (details.powerUpAbility + (details.powerUpScaleAbility * level));
                var duration = details.powerUpDuration + (details.powerUpScaleDuration * level);

                description.text = details.description.Replace("$1%", power * 100f + "%").Replace("$1", "" + power).Replace("$2", "" + duration);
                break;

            case CardAction.Ammo:
                description.text = details.description.Replace("$", "" + details.ammoAmount * level);
                break;

            case CardAction.AddWeapon:
                description.text = details.description
                    .Replace("$1", "" + (1 + details.weaponDamageFactorPerLevel * (level - 1)) * details.weapon.ammo.damage)
                    .Replace("$2", "" + details.weapon.fireRate)
                    .Replace("$3", "" + details.weapon.ammo.critChance * 100 + "%");

                break;

            default:
                break;
        }

    }

    public void setValue(float value)
    {
        backgroundGrey.fillAmount = 1f - value;
        disabledOverlay.gameObject.SetActive(value <= 0f);
    }

    public void setSelectable(bool active)
    {
        foreach (var target in raycastTargets)
        {
            target.raycastTarget = active;
        }
    }

    public void setLevel(int level)
    {
        this.level = level;

        for (int i = 0; i < levelIcons.Count; i++)
        {
            levelIcons[i].material = i < level ? _cardLevelOn : _cardLevelOff;
        }
    }

    public void ShowNewBanner(float delay)
    {

        if (details.rarity == CardRarity.Common || details.rarity == CardRarity.Default)
        {
            return;
        }

        if (details.IsKnown())
        {
            return;
        }

        details.MakeKnown();

        NewCardFeedback.InitialDelay = delay;
        NewCardFeedback.PlayFeedbacks();
    }
}

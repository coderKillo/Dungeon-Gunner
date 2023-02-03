using System.Collections;
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

    [Space(10)]
    [Header("Feedbacks")]
    [SerializeField] private MMF_Player _pointerEnterFeedback;
    public MMF_Player PointerEnterFeedback { get { return _pointerEnterFeedback; } }

    [HideInInspector] public CardSO details;
}

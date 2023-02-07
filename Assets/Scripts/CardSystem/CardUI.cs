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

    [SerializeField] private MMF_Player _destroyFeedback;
    public MMF_Player DestroyFeedback { get { return _destroyFeedback; } }

    [SerializeField] private MMF_Player _selectedFeedback;
    public MMF_Player SelectedFeedback { get { return _selectedFeedback; } }

    [SerializeField] private MMF_Player _startFeedback;
    public MMF_Player StartFeedback { get { return _startFeedback; } }

    [HideInInspector] public CardSO details;
    [HideInInspector] public int id;
}

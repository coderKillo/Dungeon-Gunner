using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MoreMountains.Feedbacks;

public class CardFlip : MonoBehaviour
{
    [SerializeField] private Transform _back;
    [SerializeField] private Transform _front;

    [Space(10)]
    [Header("Feedback")]
    [SerializeField] private MMF_Player _flipFeedback;

    private bool _isFlipped = false;
    public bool IsFlipped { get { return _isFlipped; } }

    public void ShowFront()
    {
        ShowBack();

        _back.DOScaleX(0f, 0.3f).SetUpdate(true).OnComplete(() => _front.DOScaleX(1f, 0.3f).SetUpdate(true));

        _flipFeedback.PlayFeedbacks();

        _isFlipped = true;
    }

    public void ShowBack()
    {
        _back.localScale = new Vector3(1f, 1f, 1f);
        _front.localScale = new Vector3(0f, 1f, 1f);

        _isFlipped = false;
    }
}

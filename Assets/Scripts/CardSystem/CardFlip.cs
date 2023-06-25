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

    [Space(10)]
    [Header("Sounds")]
    [SerializeField] private SoundEffectSO _flipSound;

    private bool _isFlipped = false;
    public bool IsFlipped { get { return _isFlipped; } }

    [SerializeField] private float _flipDuration = 0.6f;
    public float Duration { get { return _flipDuration; } }

    public void ShowFront()
    {
        ShowBack();

        _back.DOScaleX(0f, _flipDuration / 2).SetUpdate(true).OnComplete(() => _front.DOScaleX(1f, _flipDuration / 2).SetUpdate(true));

        _flipFeedback.PlayFeedbacks();

        if (SoundEffectManager.Instance != null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(_flipSound);
        }

        _isFlipped = true;
    }

    public void ShowBack()
    {
        _back.localScale = new Vector3(1f, 1f, 1f);
        _front.localScale = new Vector3(0f, 1f, 1f);

        _isFlipped = false;
    }
}

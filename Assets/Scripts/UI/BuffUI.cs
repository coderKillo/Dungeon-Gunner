using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BuffUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _durationText;
    [SerializeField] private Image _buffImage;
    [SerializeField] private Image _background;

    public void Initialize(Sprite sprite, Color color, float duration)
    {
        _buffImage.sprite = sprite;
        _background.color = color;
        StartCoroutine(DurationTimer(duration));
    }

    private IEnumerator DurationTimer(float duration)
    {
        int durationLeft = (int)duration;

        while (durationLeft > 0)
        {
            durationLeft--;
            UpdateDurationText(durationLeft);
            yield return new WaitForSeconds(1f);
        }

        End();
    }

    private void End()
    {
        Destroy(gameObject);
    }

    private void UpdateDurationText(int duration)
    {
        _durationText.text = duration.ToString() + "s";
    }
}

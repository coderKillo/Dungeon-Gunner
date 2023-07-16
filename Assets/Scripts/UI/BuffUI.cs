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

    private CardPowerUp _type;
    public CardPowerUp Type { get { return _type; } }

    public void Initialize(Sprite sprite, CardPowerUp type, Color color, float duration)
    {
        _type = type;
        _buffImage.sprite = sprite;
        _background.color = color;

        StartDurationTimer(duration);
    }

    public void StartDurationTimer(float duration)
    {
        StopAllCoroutines();
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

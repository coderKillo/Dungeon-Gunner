using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private Transform _levelBar;
    [SerializeField] private TextMeshProUGUI _levelText;

    private float _currentProgress = 0f;

    private void OnEnable()
    {
        CardSystemLevel.OnLevelChange += CardSystemLevel_OnLevelChange;
        CardSystemLevel.OnLevelPercentageChange += CardSystemLevel_OnLevelPercentageChange;
    }

    private void OnDisable()
    {
        CardSystemLevel.OnLevelChange -= CardSystemLevel_OnLevelChange;
        CardSystemLevel.OnLevelPercentageChange -= CardSystemLevel_OnLevelPercentageChange;
    }

    private void Update()
    {
        if (_currentProgress > _levelBar.transform.localScale.x)
        {
            _levelBar.transform.localScale += new Vector3(Time.deltaTime, 0f, 0f);
        }
    }

    private void CardSystemLevel_OnLevelPercentageChange(float pointPercent)
    {
        _currentProgress = pointPercent;
    }

    private void CardSystemLevel_OnLevelChange(int level)
    {
        _levelText.text = level.ToString();
        _levelBar.transform.localScale = new Vector3(0f, 1f, 1f);
    }
}

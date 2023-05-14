using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private Transform _levelBar;
    [SerializeField] private TextMeshProUGUI _levelText;

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

    private void CardSystemLevel_OnLevelPercentageChange(float pointPercent)
    {
        _levelBar.transform.localScale = new Vector3(pointPercent, 1f, 1f);
    }

    private void CardSystemLevel_OnLevelChange(int level)
    {
        _levelText.text = level.ToString();
    }
}

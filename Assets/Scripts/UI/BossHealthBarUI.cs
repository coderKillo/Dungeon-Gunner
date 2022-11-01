using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;

public class BossHealthBarUI : MonoBehaviour
{
    #region OBJECT REFERENCES
    [Space(10)]
    [Header("NAME")]
    [SerializeField] private TextMeshProUGUI nameText;

    [Space(10)]
    [Header("DAMAGE")]
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private float damageTextShowTime;

    [Space(10)]
    [Header("HEALTH")]
    [SerializeField] private GameObject healthBar;

    [Space(10)]
    [Header("DELAY")]
    [SerializeField] private GameObject healthBarDelayed;
    [SerializeField] private float delayTime;
    [SerializeField] private float shrinkSpeed;
    #endregion

    private Coroutine healthBarDelayRoutine;
    private Enemy trackedEnemy;

    public void Initialize(Enemy enemy)
    {
        trackedEnemy = enemy;

        nameText.text = enemy.enemyDetails.enemyName;
        damageText.text = "";

        trackedEnemy.healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }

    private void OnDisable()
    {
        trackedEnemy.healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }

    private void HealthEvent_OnHealthChanged(HealthEvent arg1, HealthEventArgs arg2)
    {
        SetHealthBar(arg2.healthPercent);

        SetDamageText(arg2.damageAmount);
    }

    private void SetHealthBar(float healthPercent)
    {
        if (healthPercent < 0)
            healthPercent = 0;

        healthBar.transform.localScale = new Vector3(healthPercent, 1f, 1f);

        healthBarDelayed.transform.DOScaleX(healthPercent, delayTime);
    }

    private void SetDamageText(int damageAmount)
    {
        if (damageAmount <= 0)
        {
            return;
        }

        damageText.text = "-" + damageAmount.ToString();
        damageText.alpha = 1f;
        damageText.DOFade(0f, damageTextShowTime);
    }
}

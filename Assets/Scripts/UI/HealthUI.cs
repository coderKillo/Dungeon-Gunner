using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using TMPro;

public class HealthUI : MonoBehaviour
{

    #region OBJECT REFERENCES
    [Space(10)]
    [Header("HEALTH")]
    [SerializeField] private GameObject healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    [Space(10)]
    [Header("SHIELD")]
    [SerializeField] private GameObject shieldBar;
    [SerializeField] private TextMeshProUGUI shieldText;

    [Space(10)]
    [Header("Level")]
    [SerializeField] private GameObject levelBar;
    [SerializeField] private TextMeshProUGUI levelText;

    [Space(10)]
    [Header("DELAY")]
    [SerializeField] private GameObject healthBarDelayed;
    [SerializeField] private float delayTime;
    [SerializeField] private float shrinkSpeed;

    [Space(10)]
    [Header("FEEDBACKS")]
    [SerializeField] private MMF_Player damageFeedback;
    #endregion

    private Coroutine healthBarDelayRoutine;

    private Player player;

    private void Awake()
    {
        player = GameManager.Instance.Player;
    }

    private void OnEnable()
    {
        player.healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;

        CardSystemLevel.OnLevelChange += CardSystemLevel_OnLevelChange;
        CardSystemLevel.OnLevelPercentageChange += CardSystemLevel_OnLevelPercentageChange;
    }

    private void OnDisable()
    {
        player.healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
        CardSystemLevel.OnLevelChange -= CardSystemLevel_OnLevelChange;
        CardSystemLevel.OnLevelPercentageChange -= CardSystemLevel_OnLevelPercentageChange;
    }

    private void CardSystemLevel_OnLevelPercentageChange(float levelPercentage)
    {
        SetLevelBar(levelPercentage);
    }

    private void CardSystemLevel_OnLevelChange(int level)
    {
        SetLevelBar(0f);
        SetLevelText(level);
    }


    private void HealthEvent_OnHealthChanged(HealthEvent arg1, HealthEventArgs arg2)
    {
        if (arg2.damageAmount > 0f)
        {
            damageFeedback.PlayFeedbacks();
        }

        SetHealthBar(arg2.healthPercent);
        SetShieldBar(arg2.shieldPercent);

        SetHealthText(arg2.healthAmount, arg2.totalHealth);
        SetShieldText(arg2.shieldAmount, arg2.totalShield);

        if (healthBarDelayRoutine != null)
        {
            StopCoroutine(healthBarDelayRoutine);
        }
        healthBarDelayRoutine = StartCoroutine(StartHealthBarDelayRoutine(arg2.healthPercent));
    }

    private void SetShieldBar(float shieldPercent)
    {
        shieldBar.transform.localScale = new Vector3(shieldPercent, 1f, 1f);
    }

    private void SetHealthBar(float healthPercent)
    {
        healthBar.transform.localScale = new Vector3(healthPercent, 1f, 1f);
    }

    private void SetLevelBar(float levelPercentage)
    {
        levelBar.transform.localScale = new Vector3(levelPercentage, 1f, 1f);
    }

    private void SetLevelText(int level)
    {
        levelText.text = "LV. " + level;
    }

    private void SetShieldText(int shieldAmount, int totalShield)
    {
        shieldText.text = shieldAmount + "/" + totalShield;
    }

    private void SetHealthText(int healthAmount, int totalHealth)
    {
        healthText.text = healthAmount + "/" + totalHealth;
    }

    private IEnumerator StartHealthBarDelayRoutine(float healthPercent)
    {
        yield return new WaitForSeconds(delayTime);


        while (healthBarDelayed.transform.localScale.x > healthPercent)
        {
            healthBarDelayed.transform.localScale -= new Vector3(shrinkSpeed, 0f, 0f);
            yield return null;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour
{

    #region OBJECT REFERENCES
    [Space(10)]
    [Header("HEALTH")]
    [SerializeField] private GameObject healthBar;

    [Space(10)]
    [Header("SHIELD")]
    [SerializeField] private GameObject shieldBar;

    [Space(10)]
    [Header("DELAY")]
    [SerializeField] private GameObject healthBarDelayed;
    [SerializeField] private float delayTime;
    [SerializeField] private float shrinkSpeed;
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
    }

    private void OnDisable()
    {
        player.healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }

    private void HealthEvent_OnHealthChanged(HealthEvent arg1, HealthEventArgs arg2)
    {
        SetHealthBar(arg2.healthPercent);
        SetShieldBar(arg2.shieldPercent);

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

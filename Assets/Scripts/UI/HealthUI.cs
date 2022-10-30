using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class HealthUI : MonoBehaviour
{

    #region OBJECT REFERENCES
    [Space(10)]
    [Header("HEALTH")]
    [SerializeField] private GameObject healthBar;

    [Space(10)]
    [Header("DELAY")]
    [SerializeField] private GameObject healthBarDelayed;
    [SerializeField] private float delayTime;
    [SerializeField] private float shrinkSpeed;
    #endregion

    [HorizontalGroup("Split", 0.5f)]
    [Button("+10", ButtonSizes.Large), GUIColor(0, 1, 0)]
    private void Heal()
    {
        player.health.TakeDamage(-10);
    }

    [HorizontalGroup("Split", 0.5f)]
    [Button("-10", ButtonSizes.Large), GUIColor(1, 0, 0)]
    private void Damage()
    {
        player.health.TakeDamage(10);
    }

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

        if (healthBarDelayRoutine != null)
        {
            StopCoroutine(healthBarDelayRoutine);
        }
        healthBarDelayRoutine = StartCoroutine(StartHealthBarDelayRoutine(arg2.healthPercent));
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(SpriteRenderer))]
[DisallowMultipleComponent]
public class PostHitImmunity : MonoBehaviour
{
    [SerializeField] private float spriteFlashInterval = 0.2f;
    [SerializeField] private Color spriteFlashColor = Color.red;
    [SerializeField] private float triggerImmunityThreshold = 1f;

    [HideInInspector] public float immunityTime = 0f;

    private Health health;
    private HealthEvent healthEvent;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        health = GetComponent<Health>();
        healthEvent = GetComponent<HealthEvent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }

    private void OnDisable()
    {
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }

    private void HealthEvent_OnHealthChanged(HealthEvent arg1, HealthEventArgs arg2)
    {
        if (immunityTime > 0f && arg2.damageAmount > triggerImmunityThreshold && arg2.healthPercent > 0f)
        {
            StartCoroutine(HitImmunityRoutine());
        }
    }

    private IEnumerator HitImmunityRoutine()
    {
        var intervals = Mathf.CeilToInt(immunityTime / (spriteFlashInterval * 2f));

        health.isDamageable = false;

        for (int i = 0; i < intervals; i++)
        {
            spriteRenderer.color = spriteFlashColor;
            yield return new WaitForSeconds(spriteFlashInterval);

            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(spriteFlashInterval);
        }

        health.isDamageable = true;
    }
}

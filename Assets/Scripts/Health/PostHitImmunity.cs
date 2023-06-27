using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(SpriteRenderer))]
[DisallowMultipleComponent]
public class PostHitImmunity : MonoBehaviour
{
    [SerializeField] private float spriteFlashInterval = 0.2f;
    [SerializeField] private Color spriteFlashColor = Color.red;
    [SerializeField] private float triggerImmunityThreshold = 1f;

    [SerializeField] private SoundEffectSO hitSoundEffect;
    [SerializeField] private SoundEffectSO hitArmorSoundEffect;

    [HideInInspector] public float immunityTime = 0f;

    private Health health;
    private HealthEvent healthEvent;
    private SpriteRenderer spriteRenderer;
    private int prevArmorAmount;

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
            PlayHitEffect();
            PlayHitSound();
        }

        if (prevArmorAmount > arg2.shieldAmount)
        {
            PlayArmorHitEffect();
            PlayArmorHitSound();
        }

        prevArmorAmount = arg2.shieldAmount;
    }

    [Button]
    private void PlayHitEffect()
    {
        var intervals = Mathf.CeilToInt(immunityTime / (spriteFlashInterval * 2f));

        Sequence mySequence = DOTween.Sequence();
        mySequence.SetLoops(intervals)
        .AppendCallback(() => health.isDamageable = false)
        .AppendCallback(() => spriteRenderer.material.EnableKeyword("HITEFFECT_ON"))
        .AppendInterval(spriteFlashInterval)
        .AppendCallback(() => spriteRenderer.material.DisableKeyword("HITEFFECT_ON"))
        .AppendInterval(spriteFlashInterval)
        .OnComplete(() => health.isDamageable = true);
    }

    [Button]
    private void PlayArmorHitEffect()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence
        .AppendCallback(() => health.isDamageable = false)
        .AppendCallback(() => spriteRenderer.material.EnableKeyword("SHINE_ON"))
        .AppendCallback(() => spriteRenderer.material.SetFloat("_ShineLocation", 0.3f))
        .Append(spriteRenderer.material.DOFloat(0.7f, "_ShineLocation", 1f))
        .AppendCallback(() => spriteRenderer.material.DisableKeyword("SHINE_ON"))
        .OnComplete(() => health.isDamageable = true);
    }

    [Button]
    private void PlayHitSound()
    {
        if (hitSoundEffect == null)
        {
            return;
        }

        SoundEffectManager.Instance.PlaySoundEffect(hitSoundEffect);
    }

    [Button]
    private void PlayArmorHitSound()
    {
        if (hitArmorSoundEffect == null)
        {
            return;
        }

        SoundEffectManager.Instance.PlaySoundEffect(hitArmorSoundEffect);
    }
}

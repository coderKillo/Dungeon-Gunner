using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(ReceiveContactDamage))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
public class DestroyableItem : MonoBehaviour
{
    [SerializeField] private int startingHealth = 5;
    [SerializeField] private SoundEffectSO destroySoundEffect;

    private HealthEvent healthEvent;
    private Health health;
    private Animator animator;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        healthEvent = GetComponent<HealthEvent>();
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        health.StartingHealth = startingHealth;
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
        if (arg2.healthAmount <= 0f)
        {
            StartCoroutine(DestroyRoutine());
        }
    }

    private IEnumerator DestroyRoutine()
    {
        if (destroySoundEffect != null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(destroySoundEffect);
        }

        boxCollider.enabled = false;

        animator.SetBool(Animations.destroy, true);

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(Animations.stateDestroyed))
        {
            yield return null;
        }

        animator.enabled = false;
    }
}

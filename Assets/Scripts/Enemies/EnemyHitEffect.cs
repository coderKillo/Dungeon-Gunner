using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyHitEffect : MonoBehaviour
{
    [SerializeField] private float _hitEffectTime;
    [SerializeField] private float _hitForce;

    private HealthEvent _healthEvent;
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _healthEvent = GetComponent<HealthEvent>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        _healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }

    private void OnDisable()
    {
        _healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }

    private void HealthEvent_OnHealthChanged(HealthEvent arg1, HealthEventArgs arg2)
    {
        if (arg2.healthAmount <= 0)
        {
            return;
        }

        if (arg2.damageAmount <= 0)
        {
            return;
        }

        StartCoroutine(HitEffectRoutine());

        PushBack();
    }

    public void PushBack()
    {
        var direction = transform.position - GameManager.Instance.PlayerPosition;

        // Debug.DrawRay(transform.position, direction, Color.red, 1f);

        _rigidbody.AddForce(new Vector2(direction.x, direction.y) * _hitForce, ForceMode2D.Impulse);
    }

    private IEnumerator HitEffectRoutine()
    {
        _spriteRenderer.material.EnableKeyword("HITEFFECT_ON");

        yield return new WaitForSeconds(_hitEffectTime);

        _spriteRenderer.material.DisableKeyword("HITEFFECT_ON");
    }
}

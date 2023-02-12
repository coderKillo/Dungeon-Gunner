using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
public class HealthEffect : MonoBehaviour
{
    [Space(10)]
    [Header("Effects")]
    [SerializeField] private SpriteEffectSO _damageEffect;
    [SerializeField] private SpriteEffectSO _healEffect;

    [Space(10)]
    [Header("References")]
    [SerializeField] private SpriteEffect _spriteEffect;

    private HealthEvent _healthEvent;

    private void Awake()
    {
        _healthEvent = GetComponent<HealthEvent>();
    }

    private void Start()
    {
        _spriteEffect.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _healthEvent.OnHealthChanged += OnHealthChanged;
    }

    private void OnDisable()
    {
        _healthEvent.OnHealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(HealthEvent arg1, HealthEventArgs arg2)
    {
        if (arg2.damageAmount > 0 && _damageEffect)
        {
            _spriteEffect.Initialize(_damageEffect);
            _spriteEffect.gameObject.SetActive(true);
        }

        if (arg2.healAmount > 0 && _healEffect)
        {
            _spriteEffect.Initialize(_healEffect);
            _spriteEffect.gameObject.SetActive(true);
        }
    }
}

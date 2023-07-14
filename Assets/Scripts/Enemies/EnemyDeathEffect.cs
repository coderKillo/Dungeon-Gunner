using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DestroyedEvent))]
public class EnemyDeathEffect : MonoBehaviour
{
    [SerializeField] private SpriteEffectSO _deathEffect;
    [SerializeField] private GameObject _corpsePrefab;

    private DestroyedEvent _destroyEvent;

    private void Awake()
    {
        _destroyEvent = GetComponent<DestroyedEvent>();
    }

    private void OnEnable()
    {
        _destroyEvent.OnDestroyed += DestroyEvent_OnDestroyed;
    }

    private void OnDisable()
    {
        _destroyEvent.OnDestroyed -= DestroyEvent_OnDestroyed;
    }

    private void DestroyEvent_OnDestroyed(DestroyedEvent arg1, DestroyedEventArgs arg2)
    {
        var effect = (SpriteEffect)PoolManager.Instance.ReuseComponent(GameResources.Instance.spriteEffectPrefab, arg1.transform.position, Quaternion.identity);
        effect.Initialize(_deathEffect);
        effect.gameObject.SetActive(true);

        GameObject.Instantiate(_corpsePrefab, transform.position, Quaternion.identity, transform.parent);
    }
}

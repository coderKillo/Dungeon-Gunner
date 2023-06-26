using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _warningCircle;
    [SerializeField] private GameObject _explosionEffect;

    [Header("Config")]
    [SerializeField] private float _delay = 1f;
    [SerializeField] private float _animationDuration = 1f;
    [SerializeField] private Vector2 _size = new Vector2(1f, 1f);
    [SerializeField] private LayerMask _mask;
    [SerializeField] private SoundEffectSO _soundEffect;

    [HideInInspector] public Action<Collider2D[], Explosion> OnExplosionHit;

    private Coroutine _explosionDelayCoroutine;

    private void OnEnable()
    {
        _explosionEffect.SetActive(false);
        _warningCircle.SetActive(true);

        _explosionDelayCoroutine = StartCoroutine(ExplosionRoutine());
    }

    private void OnDisable()
    {
        if (_explosionDelayCoroutine != null)
        {
            StopCoroutine(_explosionDelayCoroutine);
        }

        _explosionEffect.SetActive(false);
        _warningCircle.SetActive(false);
    }

    private IEnumerator ExplosionRoutine()
    {
        yield return new WaitForSeconds(_delay);

        _warningCircle.SetActive(false);

        PlayExplosionEffect();
        PlayExplosionSound();
        SpawnExplosion();

        yield return new WaitForSeconds(_animationDuration);

        gameObject.SetActive(false);
    }

    private void SpawnExplosion()
    {
        Collider2D[] colliders = Physics2D.OverlapCapsuleAll((Vector2)transform.position, _size, CapsuleDirection2D.Horizontal, 0f, _mask);

        if (colliders.Length > 0)
        {
            OnExplosionHit?.Invoke(colliders, this);
        }
    }

    private void PlayExplosionEffect()
    {
        _explosionEffect.SetActive(true);
    }

    private void PlayExplosionSound()
    {
        if (_soundEffect == null)
        {
            return;
        }

        SoundEffectManager.Instance.PlaySoundEffect(_soundEffect);
    }
}

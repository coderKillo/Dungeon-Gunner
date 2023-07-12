using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;

[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(ChargeWeaponEvent))]
[RequireComponent(typeof(SpriteRenderer))]
public class AnimateSpriteEnemy : MonoBehaviour
{
    public enum EnemyAnimation
    {
        Idle,
        Move,
        Attack,
        Death,
    }

    [SerializeField] private float frameRate = 12f;

    [SerializeField] private List<Sprite> _moveAnimation = new List<Sprite>();
    [SerializeField] private List<Sprite> _attackAnimation = new List<Sprite>();

    private WeaponFiredEvent _weaponFiredEvent;
    private MovementToPositionEvent _movementToPositionEvent;
    private AimWeaponEvent _aimWeaponEvent;
    private ChargeWeaponEvent _chargeWeaponEvent;
    private SpriteRenderer _spriteRenderer;

    private Coroutine _animationCoroutine;
    private bool _animationRunning = false;

    private void Awake()
    {
        _weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        _movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        _aimWeaponEvent = GetComponent<AimWeaponEvent>();
        _chargeWeaponEvent = GetComponent<ChargeWeaponEvent>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        _movementToPositionEvent.OnMovementToPosition += Enemy_OnMoveToPosition;
        _weaponFiredEvent.OnWeaponFired += Enemy_OnWeaponFired;
        _aimWeaponEvent.OnWeaponAim += Enemy_OnWeaponAim;
        _chargeWeaponEvent.OnChargeWeapon += Enemy_OnWeaponCharge;
    }

    private void OnDisable()
    {
        _movementToPositionEvent.OnMovementToPosition -= Enemy_OnMoveToPosition;
        _weaponFiredEvent.OnWeaponFired -= Enemy_OnWeaponFired;
        _aimWeaponEvent.OnWeaponAim -= Enemy_OnWeaponAim;
    }

    private void Enemy_OnWeaponAim(AimWeaponEvent @event, AimWeaponEventArgs args)
    {
        _spriteRenderer.flipX = (Mathf.Abs(args.aimAngle) < 90f);
    }

    private void Enemy_OnWeaponFired(WeaponFiredEvent @event, WeaponFiredEventArgs args)
    {
        // Do Nothing
    }

    private void Enemy_OnMoveToPosition(MovementToPositionEvent @event, MovementToPositionArgs args)
    {
        PlayAnimationNoOverride(_moveAnimation);
    }

    private void Enemy_OnWeaponCharge(ChargeWeaponEvent @event, ChargeWeaponEventArgs args)
    {
        if (args.active)
        {
            PlayAnimation(_moveAnimation);
        }
        else
        {
            PlayAnimation(_attackAnimation);

        }
    }


    [Button]
    private void PlayAnimation(List<Sprite> animation)
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }

        _animationCoroutine = StartCoroutine(AnimationCoroutine(animation));
    }

    [Button]
    private void PlayAnimationNoOverride(List<Sprite> animation)
    {
        if (!_animationRunning)
        {
            PlayAnimation(animation);
        }
    }

    private IEnumerator AnimationCoroutine(List<Sprite> frames)
    {
        _animationRunning = true;

        foreach (var sprite in frames)
        {
            _spriteRenderer.sprite = sprite;
            yield return new WaitForSeconds(1 / frameRate);
        }

        _animationRunning = false;
    }
}

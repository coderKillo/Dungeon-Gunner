using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;

[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(SpriteRenderer))]
public class AnimateSpriteEnemy : SerializedMonoBehaviour
{
    public enum EnemyAnimation
    {
        Idle,
        Move,
        Attack,
        Death,
    }

    [Serializable]
    public class AnimationStage
    {
        public List<Sprite> frames = new List<Sprite>();
        public float frameRate = 24f;
    }

    [SerializeField] private Dictionary<EnemyAnimation, AnimationStage> _animations;

    private WeaponFiredEvent _weaponFiredEvent;
    private MovementToPositionEvent _movementToPositionEvent;
    private AimWeaponEvent _aimWeaponEvent;
    private SpriteRenderer _spriteRenderer;

    private Coroutine _animationCoroutine;
    private bool _animationRunning = false;

    private void Awake()
    {
        _weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        _movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        _aimWeaponEvent = GetComponent<AimWeaponEvent>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        _movementToPositionEvent.OnMovementToPosition += Enemy_OnMoveToPosition;
        _weaponFiredEvent.OnWeaponFired += Enemy_OnWeaponFired;
        _aimWeaponEvent.OnWeaponAim += Enemy_OnWeaponAim;
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
        PlayAnimation(EnemyAnimation.Attack);
    }

    private void Enemy_OnMoveToPosition(MovementToPositionEvent @event, MovementToPositionArgs args)
    {
        PlayAnimationNoOverride(EnemyAnimation.Move);
    }

    [Button]
    private void PlayAnimation(EnemyAnimation animation)
    {
        if (!_animations.ContainsKey(animation))
        {
            return;
        }

        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }

        _animationCoroutine = StartCoroutine(AnimationCoroutine(_animations[animation].frames, _animations[animation].frameRate));
    }

    [Button]
    private void PlayAnimationNoOverride(EnemyAnimation animation)
    {
        if (!_animationRunning)
        {
            PlayAnimation(animation);
        }
    }

    private IEnumerator AnimationCoroutine(List<Sprite> frames, float frameRate)
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

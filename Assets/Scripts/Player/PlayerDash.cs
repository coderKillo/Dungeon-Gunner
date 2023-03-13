using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using System;

[RequireComponent(typeof(MovementToPositionEvent))]
public class PlayerDash : MonoBehaviour
{
    [Space(10)]
    [Header("Effects")]
    [SerializeField] private SpriteEffectSO _dashEffect;
    [SerializeField] private Transform _dashEffectSpawnLocation;
    [SerializeField] private SoundEffectSO _dashSoundEffect;

    [Space(10)]
    [Header("Feedbacks")]
    [SerializeField] private MMF_Player _startDashFeedback;
    [SerializeField] private MMF_Player _stopDashFeedback;

    private bool _isDashing = false;
    private float _dashingCooldownTimer = 0f;
    private Coroutine _dashCoroutine;
    private MovementDetailsSO _movementDetails;
    private WaitForFixedUpdate _waitForFixedUpdate;

    private MovementToPositionEvent movementToPositionEvent;

    private void Awake()
    {
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
    }

    private void Start()
    {
        _waitForFixedUpdate = new WaitForFixedUpdate();
    }

    private void Update()
    {
        DashCooldownTimer();
    }

    public bool CanDash()
    {
        return _dashingCooldownTimer <= 0f;
    }

    public bool IsDashing() { return _isDashing; }

    public void Dash(MovementDetailsSO details, Vector2 direction)
    {
        _movementDetails = details;

        DashEffect(direction);
        DashSoundEffect();

        _dashCoroutine = StartCoroutine(DashCoroutine(direction));
    }

    public void StopDash()
    {
        if (_dashCoroutine != null)
        {
            StopCoroutine(_dashCoroutine);
        }

        _dashingCooldownTimer = _movementDetails.dashCooldown;
        _stopDashFeedback.PlayFeedbacks();
        _isDashing = false;
    }

    private IEnumerator DashCoroutine(Vector2 direction)
    {
        _isDashing = true;

        var targetPosition = transform.position + (Vector3)direction * _movementDetails.dashTime * _movementDetails.dashSpeed;
        var timeElapsed = 0f;

        _startDashFeedback.PlayFeedbacks();

        while (timeElapsed < _movementDetails.dashTime)
        {
            timeElapsed += Time.fixedDeltaTime;

            var dashSpeedMultiplier = _movementDetails.dashSpeedMultiplier.Evaluate(timeElapsed / _movementDetails.dashTime);
            var dashSpeed = _movementDetails.dashSpeed * dashSpeedMultiplier;

            movementToPositionEvent.CallMovementToPositionEvent(transform.position, targetPosition, dashSpeed, direction, _isDashing);
            yield return _waitForFixedUpdate;
        }

        StopDash();
    }

    private void DashCooldownTimer()
    {
        if (_dashingCooldownTimer >= 0f)
        {
            _dashingCooldownTimer -= Time.deltaTime;
        }
    }

    private void DashEffect(Vector2 direction)
    {
        if (_dashEffect == null)
        {
            return;
        }


        var effect = (SpriteEffect)PoolManager.Instance.ReuseComponent(GameResources.Instance.spriteEffectPrefab, _dashEffectSpawnLocation.position, Quaternion.identity);
        effect.Initialize(_dashEffect);
        effect.gameObject.SetActive(true);


        var angle = HelperUtilities.GetAngleFromVector(direction);

        effect.transform.Rotate(new Vector3(0f, 0f, angle));

        switch (HelperUtilities.GetAimDirection(angle))
        {
            case AimDirection.Left:
            case AimDirection.UpLeft:
                effect.transform.localScale = new Vector3(1f, -1f, 0f);
                break;

            case AimDirection.Up:
            case AimDirection.Right:
            case AimDirection.UpRight:
            case AimDirection.Down:
                effect.transform.localScale = new Vector3(1f, 1f, 0f);
                break;
        }
    }


    private void DashSoundEffect()
    {
        SoundEffectManager.Instance.PlaySoundEffect(_dashSoundEffect);
    }

}

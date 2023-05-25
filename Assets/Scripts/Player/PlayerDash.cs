using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using System;

[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(DealContactDamage))]
[RequireComponent(typeof(PlayerControl))]
public class PlayerDash : MonoBehaviour
{
    [Space(10)]
    [Header("Contact Damage")]
    [SerializeField] private Transform _contactDamageStartLocation;
    [SerializeField] private LayerMask _contactDamageMask;
    [SerializeField] private int _contactDamage = 0;
    [SerializeField] private float _contactDamageCollisionRadius = 0.7f;
    public int Damage { set { _contactDamage = value; } }

    [Space(10)]
    [Header("Effects")]
    [SerializeField] private SpriteEffectSO _dashEffect;
    public SpriteEffectSO Effect { set { _dashEffect = value; } }

    [SerializeField] private Transform _dashEffectSpawnLocation;
    [SerializeField] private SoundEffectSO _dashSoundEffect;
    [SerializeField] private SpriteEffect _dashCooldownEffect;

    [Space(10)]
    [Header("Feedbacks")]
    [SerializeField] private MMF_Player _startDashFeedback;
    [SerializeField] private MMF_Player _stopDashFeedback;

    private bool _isDashing = false;
    private float _dashingCooldownTimer = 0f;
    private Coroutine _dashCoroutine;
    private WaitForFixedUpdate _waitForFixedUpdate;

    private MovementToPositionEvent _movementToPositionEvent;
    private PlayerControl _playerControl;
    private Vector3 _startPosition;
    private Vector3 _targetPosition;

    private void Awake()
    {
        _movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        _playerControl = GetComponent<PlayerControl>();
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

    public void Dash(Vector2 direction)
    {
        if (_contactDamage > 0)
        {
            DealDamage(direction, _contactDamage);
        }

        DashSoundEffect();

        _dashCoroutine = StartCoroutine(DashCoroutine(direction));
    }

    public void StopDash()
    {
        if (_dashCoroutine != null)
        {
            StopCoroutine(_dashCoroutine);
        }

        if (_isDashing)
        {
            DashEffect();
            DashCooldownEffect();
        }

        _dashingCooldownTimer = _playerControl.MovementDetails.dashCooldown;
        _stopDashFeedback.PlayFeedbacks();
        _isDashing = false;
    }

    private IEnumerator DashCoroutine(Vector2 direction)
    {
        _isDashing = true;
        _startPosition = transform.position;
        _targetPosition = transform.position + (Vector3)direction * _playerControl.MovementDetails.dashTime * _playerControl.MovementDetails.dashSpeed;

        var timeElapsed = 0f;

        _startDashFeedback.PlayFeedbacks();

        while (timeElapsed < _playerControl.MovementDetails.dashTime)
        {
            timeElapsed += Time.fixedDeltaTime;

            var dashSpeedMultiplier = _playerControl.MovementDetails.dashSpeedMultiplier.Evaluate(timeElapsed / _playerControl.MovementDetails.dashTime);
            var dashSpeed = _playerControl.MovementDetails.dashSpeed * dashSpeedMultiplier;

            _movementToPositionEvent.CallMovementToPositionEvent(transform.position, _targetPosition, dashSpeed, direction, _isDashing);
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

    private void DashCooldownEffect()
    {
        _dashCooldownEffect.FrameRate = 8f / _playerControl.MovementDetails.dashCooldown;
        _dashCooldownEffect.gameObject.SetActive(true);
    }

    private void DashEffect()
    {
        if (_dashEffect == null)
        {
            return;
        }

        var targetVector = _targetPosition - _startPosition;
        var currentVector = transform.position - _startPosition;
        var relativeDistanceTraveled = currentVector.magnitude / targetVector.magnitude;

        var effect = (SpriteEffect)PoolManager.Instance.ReuseComponent(GameResources.Instance.spriteEffectMaskedPrefab, _dashEffectSpawnLocation.position - currentVector, Quaternion.identity);
        effect.Initialize(_dashEffect);
        effect.gameObject.SetActive(true);

        var spriteRenderer = effect.GetComponent<SpriteRenderer>();
        var spriteRect = spriteRenderer.sprite.rect.size / spriteRenderer.sprite.pixelsPerUnit;
        spriteRenderer.size = new Vector2(relativeDistanceTraveled * spriteRect.x, spriteRect.y);

        var angle = HelperUtilities.GetAngleFromVector(currentVector);

        effect.transform.Rotate(new Vector3(0f, 0f, angle));

        if (Mathf.Abs(angle) >= 90f)
        {
            effect.transform.localScale = new Vector3(1, -1f, 0f);
        }
        else
        {
            effect.transform.localScale = new Vector3(1, 1f, 0f);
        }
    }

    private void DashSoundEffect()
    {
        SoundEffectManager.Instance.PlaySoundEffect(_dashSoundEffect);
    }

    private void DealDamage(Vector2 direction, int damage)
    {
        var dashVector = direction.normalized * _playerControl.MovementDetails.dashSpeed * _playerControl.MovementDetails.dashTime;
        var colliders = Physics2D.CircleCastAll(new Vector2(_contactDamageStartLocation.position.x, _contactDamageStartLocation.position.y), _contactDamageCollisionRadius, dashVector, dashVector.magnitude, _contactDamageMask);

        foreach (var hit in colliders)
        {
            var health = hit.collider.GetComponent<Health>();
            if (health == null)
            {
                continue;
            }

            health.TakeDamage(damage, false);
        }
    }
}

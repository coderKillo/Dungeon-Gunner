using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyHitEffect : MonoBehaviour
{
    [SerializeField] private float _hitEffectTime;
    [SerializeField] private float _hitForce;
    [SerializeField] private SpriteEffectSO[] _bloodEffectArray;

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

        var direction = transform.position - GameManager.Instance.PlayerPosition;

        PushBack(direction);
        BloodEffect(direction);
    }

    private void BloodEffect(Vector3 direction)
    {
        if (_bloodEffectArray.Length <= 0)
        {
            return;
        }

        var effect = (SpriteEffect)PoolManager.Instance.ReuseComponent(GameResources.Instance.spriteEffectPrefab, transform.position, Quaternion.identity);
        effect.Initialize(_bloodEffectArray[Random.Range(0, _bloodEffectArray.Length)]);
        effect.gameObject.SetActive(true);
    }

    public void PushBack(Vector3 direction)
    {
        _rigidbody.AddForce(new Vector2(direction.x, direction.y) * _hitForce, ForceMode2D.Impulse);
    }

    private IEnumerator HitEffectRoutine()
    {
        _spriteRenderer.material.EnableKeyword("HITEFFECT_ON");

        yield return new WaitForSeconds(_hitEffectTime);

        _spriteRenderer.material.DisableKeyword("HITEFFECT_ON");
    }
}

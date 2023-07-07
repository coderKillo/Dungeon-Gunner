using System;
using UnityEngine;

public class FrostStatusEffect : MonoBehaviour
{
    [Space(10)]
    [Header("Config")]
    [SerializeField] private float _duration = 5f;
    [SerializeField] private int _iceBlockStackThreshold = 3;
    [SerializeField] private int _iceBlockDamageThreshold = 20;
    [SerializeField] private int _iceBlockExplosionDamage = 20;
    public int Damage { set { _iceBlockExplosionDamage = value; } }
    [SerializeField] private float _iceBlockExplosionRadius = 5f;
    public float Radius { set { _iceBlockExplosionRadius = value; } }
    [SerializeField] private LayerMask _mask;

    [Space(10)]
    [Header("References")]
    [SerializeField] private SpriteEffectSO _iceBlockExplosionEffect;
    [SerializeField] private SpriteEffect _iceBlockEffect;

    private enum Stage
    {
        NotApplied,
        Applied,
        Frozen,
    }

    private Enemy _enemy;
    private SpriteRenderer _enemySpriteRenderer;

    private Stage _currentStage = Stage.NotApplied;
    private int _stack = 0;
    private float _durationTimer = 99999f;
    private int _iceBlockDamageTaken = 0;

    private void Awake()
    {
        _enemy = GetComponentInParent<Enemy>();
        _enemySpriteRenderer = _enemy.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _enemy.healthEvent.OnHealthChanged += Enemy_OnHealthChanged;
    }

    private void Enemy_OnHealthChanged(HealthEvent @event, HealthEventArgs args)
    {
        if (_currentStage != Stage.Frozen || _stack < (_iceBlockStackThreshold + 1))
        {
            return;
        }

        _iceBlockDamageTaken += args.damageAmount;

        if (_iceBlockDamageTaken >= _iceBlockDamageThreshold)
        {
            IceBlockExplode();
        }
    }

    private void Update()
    {
        if (_currentStage == Stage.NotApplied)
        {
            return;
        }

        _durationTimer -= Time.deltaTime;
        if (_durationTimer < 0)
        {
            EndEffect();
        }
    }

    public void Activate()
    {
        _durationTimer = _duration;

        switch (_currentStage)
        {
            case Stage.NotApplied:
                {

                    StartEffect();
                }
                break;

            case Stage.Applied:
                {
                    _stack++;

                    if (_stack >= _iceBlockStackThreshold)
                    {
                        StartIceBlock();
                    }
                }
                break;

            case Stage.Frozen:
                {
                    _stack++;
                }
                break;


            default:
                break;
        }
    }

    private void StartEffect()
    {
        _currentStage = Stage.Applied;

        _stack = 1;
        _iceBlockDamageTaken = 0;

        _enemySpriteRenderer.material.SetFloat("_GreyscaleBlend", 1f);
        _enemySpriteRenderer.material.SetColor("_Color", Color.cyan);
    }


    private void StartIceBlock()
    {
        _currentStage = Stage.Frozen;

        _iceBlockEffect.gameObject.SetActive(true);
    }

    private void IceBlockExplode()
    {
        EndEffect();

        var effect = (SpriteEffect)PoolManager.Instance.ReuseComponent(GameResources.Instance.spriteEffectPrefab, transform.position, Quaternion.identity);
        effect.Initialize(_iceBlockExplosionEffect);
        effect.gameObject.SetActive(true);

        Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)transform.position, _iceBlockExplosionRadius, _mask);

        foreach (var collider in colliders)
        {
            var health = collider.GetComponent<Health>();

            if (health != null)
            {
                health.TakeDamage(_iceBlockExplosionDamage, false);
            }
        }
    }

    private void EndEffect()
    {
        _currentStage = Stage.NotApplied;

        _iceBlockEffect.gameObject.SetActive(false);

        _enemySpriteRenderer.material.SetFloat("_GreyscaleBlend", 0f);
        _enemySpriteRenderer.material.SetColor("_Color", Color.white);
    }
}
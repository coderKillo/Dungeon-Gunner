using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class OnHitExplosion : MonoBehaviour, IOnHit
{
    [SerializeField] private float _radius;
    [SerializeField] private int _damage = 10;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private SpriteEffectSO _effect;

    void Update()
    {
        Debug.DrawCircle(transform.position, _radius, 20, Color.green);
    }

    public void SetDamage(int damage)
    {
        _damage = damage;
    }

    public void SetRadius(float radius)
    {
        _radius = radius;
    }

    [Button()]
    public void Hit(Collider2D collider)
    {
        transform.position = collider.transform.position;

        HitEffect();

        var hits = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), _radius, _layerMask);

        foreach (var hit in hits)
        {
            if (hit == null)
            {
                continue;
            }

            var health = hit.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(_damage, false);
            }
        }

        gameObject.SetActive(false);
    }

    private void HitEffect()
    {
        var hitEffect = (SpriteEffect)PoolManager.Instance.ReuseComponent(GameResources.Instance.spriteEffectPrefab, transform.position, Quaternion.identity);
        hitEffect.Initialize(_effect);
        hitEffect.gameObject.SetActive(true);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}

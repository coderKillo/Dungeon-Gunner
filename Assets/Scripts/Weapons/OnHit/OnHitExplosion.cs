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
    public void Hit()
    {
        var colliderList = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), _radius, _layerMask);

        foreach (var collider in colliderList)
        {
            if (collider == null)
            {
                continue;
            }

            var health = collider.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(_damage, false);
            }
        }

        gameObject.SetActive(false);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}

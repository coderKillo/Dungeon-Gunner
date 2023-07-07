using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class OnHitFrost : MonoBehaviour, IOnHit
{
    [SerializeField] private float _radius;
    [SerializeField] private int _damage = 10;
    [SerializeField] private LayerMask _layerMask;

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
        var frostStatusEffect = collider.GetComponentInChildren<FrostStatusEffect>();
        if (frostStatusEffect != null)
        {
            frostStatusEffect.Damage = _damage;
            frostStatusEffect.Radius = _radius;
            frostStatusEffect.Activate();
        }
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}

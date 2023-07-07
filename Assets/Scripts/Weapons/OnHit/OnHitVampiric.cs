using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class OnHitVampiric : MonoBehaviour, IOnHit
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
        GameManager.Instance.Player.health.Heal(_damage);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}

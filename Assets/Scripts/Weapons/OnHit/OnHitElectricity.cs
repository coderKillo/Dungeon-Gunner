using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class OnHitElectricity : MonoBehaviour, IOnHit
{
    [SerializeField] private float _radius;
    [SerializeField] private int _damage = 10;
    [SerializeField] private GameObject _electricityBulletPrefab;
    [SerializeField] private LayerMask _layerMask;

    private IEnumerator ChainLightning(Collider2D[] colliderList)
    {
        var sourcePos = transform.position;

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

                var targetPos = collider.transform.position;
                var bullet = (ElectricityEffect)PoolManager.Instance.ReuseComponent(_electricityBulletPrefab, sourcePos, Quaternion.identity);
                bullet.Target = targetPos;
                bullet.Source = sourcePos;
                bullet.gameObject.SetActive(true);
                bullet.Fire();

                yield return new WaitForSeconds(bullet.TravelTime);

                sourcePos = targetPos;
            }
        }

        gameObject.SetActive(false);
    }

    void Update()
    {
        Debug.DrawCircle(transform.position, _radius, 20, Color.green);
    }

    public void SetDamage(int damage)
    {
        _damage = damage;
    }

    [Button()]
    public void Hit()
    {
        var colliderList = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), _radius, _layerMask);
        StartCoroutine(ChainLightning(colliderList));
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}

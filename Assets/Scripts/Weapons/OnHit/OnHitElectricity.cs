using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OnHitElectricity : MonoBehaviour
{
    [SerializeField] private float _radius;
    [SerializeField] private int _damage = 10;
    [SerializeField] private GameObject _electricityBulletPrefab;

    private void OnEnable()
    {
        var colliderList = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), _radius);
        StartCoroutine(ChainLightning(colliderList));
    }

    private IEnumerator ChainLightning(Collider2D[] colliderList)
    {
        var sourcePos = transform.position;

        foreach (var collider in colliderList)
        {
            var health = collider.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(10, false);

                var targetPos = collider.transform.position;
                var bullet = (ElectricityEffect)PoolManager.Instance.ReuseComponent(_electricityBulletPrefab, sourcePos, Quaternion.identity);
                bullet.Target = targetPos;
                bullet.Source = sourcePos;
                bullet.gameObject.SetActive(true);

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
}

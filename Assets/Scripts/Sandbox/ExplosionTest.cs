using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionTest : MonoBehaviour
{

    [SerializeField] private GameObject _prefab;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SpawnExplosion(HelperUtilities.GetWorldMousePosition());
        }
    }

    private void SpawnExplosion(Vector3 position)
    {
        var explosion = (Explosion)PoolManager.Instance.ReuseComponent(_prefab, position, Quaternion.identity);
        explosion.gameObject.SetActive(true);
    }
}

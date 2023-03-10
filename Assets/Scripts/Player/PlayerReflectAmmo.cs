using System.Net.WebSockets;
using System.Net;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReflectAmmo : MonoBehaviour
{
    [SerializeField] private GameObject _ammoPrefab;
    [SerializeField] private Material _ammoMaterial;

    private Health _health;
    private bool _enable = false;

    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    public void Enable()
    {
        _health.isDamageable = false;
        _enable = true;
    }

    public void Disable()
    {
        _health.isDamageable = true;
        _enable = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_enable)
        {
            return;
        }
        if (other.tag != Settings.enemyAmmoTag)
        {
            return;
        }

        var enemyAmmo = other.GetComponent<Ammo>();

        var ammo = (IFireable)PoolManager.Instance.ReuseComponent(_ammoPrefab, other.transform.position, Quaternion.identity);
        var directionVector = -enemyAmmo.FireDirectionVector;
        var angle = HelperUtilities.GetAngleFromVector(directionVector);
        var speed = Random.Range(enemyAmmo.AmmoDetails.speedMin, enemyAmmo.AmmoDetails.speedMax);
        var damage = Mathf.RoundToInt(enemyAmmo.AmmoDetails.damage);
        var critChance = enemyAmmo.AmmoDetails.critChance;

        ammo.InitialAmmo(enemyAmmo.AmmoDetails, angle, angle, speed, directionVector, damage, critChance);
        ammo.GetGameObject().GetComponent<SpriteRenderer>().material = _ammoMaterial;
    }
}

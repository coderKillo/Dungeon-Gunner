using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class AmmoAirStrike : Ammo
{
    [SerializeField] private LayerMask explosionMask;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionDuration;
    [SerializeField] private float explosionPadding;

    private bool disable = false;
    private float explosionDistance = 0f;

    public override void InitialAmmo(AmmoDetailsSO ammoDetails, float aimAngel, float weaponAngle, float speed, Vector3 weaponAimDirection, int damage, float critChance, bool overrideAmmoMovement = false)
    {
        this.ammoDetails = ammoDetails;
        this.range = ammoDetails.range;
        this.speed = speed;
        this.isColliding = false;
        this.damage = damage;
        this.critChance = critChance;
        this.fireDirectionVector = HelperUtilities.GetVectorFromAngle(aimAngel);
        this.disable = false;
        this.explosionDistance = 0f;

        transform.eulerAngles = new Vector3(0, 0, aimAngel);

        gameObject.SetActive(true);
        SetupTrail(ammoDetails);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (HelperUtilities.IsObjectOnLayerMask(other.gameObject, explosionMask))
        {
            return;
        }

        if (isColliding)
        {
            return;
        }

        isColliding = true;

        disable = true;
    }

    private void Update()
    {
        if (disable)
        {
            gameObject.SetActive(false);
            return;
        }

        CheckExplosion();
        CheckRange();
    }

    private void CheckExplosion()
    {
        var distance = fireDirectionVector.normalized * speed * Time.deltaTime;

        explosionDistance -= distance.magnitude;
        if (explosionDistance <= 0f)
        {
            explosionDistance = explosionPadding;
            SpawnExplosion(transform.position);
        }
    }

    protected void SpawnExplosion(Vector3 position)
    {
        PlayHitSound();

        Debug.DrawCircle(position, explosionRadius, 16, Color.green);

        Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)position, explosionRadius, explosionMask);

        foreach (var hit in colliders)
        {
            DealDamage(hit);
        }

    }

    protected void CheckRange()
    {
        var distance = fireDirectionVector.normalized * speed * Time.deltaTime;

        range -= distance.magnitude;
        if (range > 0f)
        {
            transform.position += distance;
        }
        else
        {
            disable = true;
        }
    }
}

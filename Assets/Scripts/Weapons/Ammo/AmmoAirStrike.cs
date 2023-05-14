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
    private float disableTimer = 0f;
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
        this.disableTimer = 0f;
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
        disableTimer -= Time.time;
        if (disable && disableTimer <= 0f)
        {
            gameObject.SetActive(false);
            return;
        }

        var distance = fireDirectionVector.normalized * speed * Time.deltaTime;

        CheckExplosion(distance);
        CheckRange(distance);
    }

    private void CheckExplosion(Vector3 distance)
    {
        explosionDistance -= distance.magnitude;
        if (explosionDistance <= 0f)
        {
            explosionDistance = explosionPadding;
            SpawnExplosion();
        }
    }

    private void SpawnExplosion()
    {
        PlayHitSound();

        Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)transform.position, explosionRadius, explosionMask);

        foreach (var hit in colliders)
        {
            DealDamage(hit);
        }

    }

    private void CheckRange(Vector3 distance)
    {
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

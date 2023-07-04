using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoTargetLocation : Ammo
{
    [SerializeField] private LayerMask mask;

    private float radius = 0f;

    public override void InitialAmmo(AmmoDetailsSO ammoDetails, float aimAngel, float weaponAngle, float speed, Vector3 weaponAimDirection, int damage, float critChance, bool overrideAmmoMovement = false)
    {
        this.ammoDetails = ammoDetails;
        this.speed = speed;
        this.range = ammoDetails.range;
        this.damage = damage;
        this.critChance = critChance;
        this.radius = ammoDetails.damageRadius;

        var target = HelperUtilities.GetWorldMousePosition();

        transform.eulerAngles = new Vector3(0, 0, aimAngel);
        transform.position += Vector3.ClampMagnitude(target - transform.position, range);

        TriggerAmmo();
    }

    private void TriggerAmmo()
    {
        Debug.DrawCircle(transform.position, radius, 16, Color.green);

        Vector2 position2D = new Vector2(transform.position.x, transform.position.y);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position2D, radius, mask);

        foreach (var hit in colliders)
        {
            DealDamage(hit);
        }

        PlayHitSound();
        AmmoHitEffect();

        gameObject.SetActive(false);
    }
}

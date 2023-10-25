using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoLightningStrike : Ammo
{
    [SerializeField] private GameObject electricityBulletPrefab;
    [SerializeField] private LayerMask mask;

    private float radius;
    private int maxHitCount; // 0 = hit all

    public override void InitialAmmo(AmmoDetailsSO ammoDetails, float aimAngel, float weaponAngle, float speed, Vector3 weaponAimDirection, int damage, float critChance, bool overrideAmmoMovement = false)
    {
        this.ammoDetails = ammoDetails;
        this.speed = speed;
        this.range = ammoDetails.range;
        this.damage = damage;
        this.critChance = critChance;
        this.radius = ammoDetails.damageRadius;
        this.maxHitCount = ammoDetails.hitLimit;
        this.fireDirectionVector = HelperUtilities.GetVectorFromAngle(aimAngel);
        this.onHitEffects.Clear();

        transform.eulerAngles = new Vector3(0, 0, aimAngel);

        gameObject.SetActive(true);

        TriggerAmmo();
    }

    private void TriggerAmmo()
    {
        Collider2D[] colliders = ConeCastAll();

        if (colliders.Length > 0)
        {
            var strikes = maxHitCount > 0 ? maxHitCount : colliders.Length;

            for (int i = 0; i < strikes; i++)
            {
                var randomCollider = colliders[Random.Range(0, colliders.Length)];

                var bullet = (ElectricityEffect)PoolManager.Instance.ReuseComponent(electricityBulletPrefab, transform.position, Quaternion.identity);
                bullet.Target = randomCollider.transform.position;
                bullet.Source = transform.position;
                bullet.gameObject.SetActive(true);
                bullet.Fire();

                DealDamage(randomCollider);
            }

            PlayHitSound();
            AmmoHitEffect();
        }

        gameObject.SetActive(false);
    }

    private Collider2D[] ConeCastAll()
    {
        List<Collider2D> coneCastHit = new List<Collider2D>();

        var circleCastHits = Physics2D.CircleCastAll((Vector2)transform.position, radius, fireDirectionVector, range, mask);
        var coneAngle = HelperUtilities.GetAngleFromVector(new Vector3(range, radius / 2, 0f));

        for (int i = 0; i < circleCastHits.Length; i++)
        {
            Vector3 hitDirection = circleCastHits[i].point - (Vector2)transform.position;
            float hitAngle = Mathf.Abs(Vector3.Angle(fireDirectionVector, hitDirection));

            if (hitAngle < coneAngle || hitDirection.magnitude < range)
            {
                if (circleCastHits[i].collider.GetComponent<Health>() != null)
                {
                    coneCastHit.Add(circleCastHits[i].collider);
                }
            }
        }

        return coneCastHit.ToArray();
    }
}
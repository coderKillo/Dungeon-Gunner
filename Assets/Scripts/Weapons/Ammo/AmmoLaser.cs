using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AmmoLaser : Ammo
{
    private List<GameObject> alreadyCollided;
    private float lifetime;
    private float radius;

    private void Start()
    {
        alreadyCollided = new List<GameObject>();
    }

    public override void InitialAmmo(AmmoDetailsSO ammoDetails, float aimAngel, float weaponAngle, float speed, Vector3 weaponAimDirection, int damage, float critChance, bool overrideAmmoMovement = false)
    {
        this.ammoDetails = ammoDetails;
        this.range = ammoDetails.range;
        this.speed = speed;
        this.isColliding = false;
        this.damage = damage;
        this.critChance = critChance;
        this.fireDirectionVector = HelperUtilities.GetVectorFromAngle(aimAngel);
        this.lifetime = ammoDetails.trailLifetime;
        this.radius = ammoDetails.trailStartWidth;

        transform.eulerAngles = new Vector3(0, 0, aimAngel);

        gameObject.SetActive(true);
        SetupTrail(ammoDetails);
        Fire();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        return;
    }

    private void Fire()
    {
        var colliders = Physics2D.CircleCastAll((Vector2)transform.position, radius, (Vector2)fireDirectionVector, ammoDetails.range);
        foreach (var hit in colliders)
        {
            DealDamage(hit.collider);
        }
    }

    private void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            gameObject.SetActive(false);
            return;
        }

        var distance = fireDirectionVector.normalized * speed * Time.deltaTime;

        range -= distance.magnitude;
        if (range > 0f)
        {
            transform.position += distance;
        }
    }
}

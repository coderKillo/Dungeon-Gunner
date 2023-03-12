using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoExplosion : Ammo
{
    [SerializeField] private float radius = 30f;

    public override void InitialAmmo(AmmoDetailsSO ammoDetails, float aimAngel, float weaponAngle, float speed, Vector3 weaponAimDirection, int damage, float critChance, bool overrideAmmoMovement = false)
    {
        this.ammoDetails = ammoDetails;
        this.speed = speed;
        this.range = ammoDetails.range;
        this.damage = damage;
        this.critChance = critChance;

        transform.eulerAngles = new Vector3(0, 0, aimAngel);

        gameObject.SetActive(true);
    }

    private void Update()
    {
        var distance = transform.right * speed * Time.deltaTime;
        transform.position += distance;

        range -= distance.magnitude;
        if (range < 0f)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isColliding)
        {
            return;
        }

        isColliding = true;

        Vector2 explosionPos = new Vector2(transform.position.x, transform.position.y);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos, radius);

        foreach (var hit in colliders)
        {
            DealDamage(hit);
        }

        PlayHitSound();
        AmmoHitEffect();

        gameObject.SetActive(false);
    }
}

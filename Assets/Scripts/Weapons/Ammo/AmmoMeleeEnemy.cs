using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoMeleeEnemy : Ammo
{
    private List<GameObject> alreadyCollided;

    private BoxCollider2D boxCollider;

    private float aimAngle;
    private float timer = 0f;

    protected override void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public override void InitialAmmo(AmmoDetailsSO ammoDetails, float aimAngel, float weaponAngle, float speed, Vector3 weaponAimDirection, int damage, float critChance, bool overrideAmmoMovement = false)
    {
        this.ammoDetails = ammoDetails;
        this.speed = speed;
        this.range = ammoDetails.range;
        this.alreadyCollided = new List<GameObject>();
        this.damage = damage;
        this.critChance = critChance;
        this.aimAngle = aimAngel;
        this.timer = 0f;

        boxCollider.size = new Vector2(ammoDetails.hitboxWidth, this.range);
        boxCollider.offset = new Vector2(0f, this.range / 2f);

        gameObject.SetActive(true);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        var angle = Mathf.Lerp(ammoDetails.startAngle, ammoDetails.endAngle, timer / ammoDetails.rotationDuration);

        transform.eulerAngles = new Vector3(0, 0, aimAngle + angle);

        if (timer > ammoDetails.rotationDuration)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (alreadyCollided.Contains(other.gameObject))
        {
            return;
        }
        alreadyCollided.Add(other.gameObject);

        DealDamage(other);
        PlayHitSound();
    }
}

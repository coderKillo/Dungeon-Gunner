using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoMelee : Ammo
{
    private List<GameObject> alreadyCollided;

    private BoxCollider2D boxCollider;

    private float aimAngle;
    private float timer = 0f;

    protected override void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        GameManager.Instance.Player.aimWeaponEvent.OnWeaponAim += Player_OnAimWeaponEvent;
    }

    private void OnDisable()
    {
        GameManager.Instance.Player.aimWeaponEvent.OnWeaponAim -= Player_OnAimWeaponEvent;
    }

    private void Player_OnAimWeaponEvent(AimWeaponEvent @event, AimWeaponEventArgs args)
    {
        aimAngle = args.aimAngle;

        transform.localScale = new Vector3(1f, Mathf.Abs(aimAngle) >= 90f ? -1f : 1f, 0f);
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
        this.onHitEffects.Clear();

        boxCollider.size = new Vector2(ammoDetails.hitboxWidth, this.range);
        boxCollider.offset = new Vector2(0f, this.range / 2f);

        gameObject.SetActive(true);
    }

    private void Update()
    {
        transform.position = GameManager.Instance.Player.activeWeapon.Position.position;

        timer += Time.deltaTime;

        var angle = Mathf.Lerp(ammoDetails.startAngle, ammoDetails.endAngle, timer / ammoDetails.rotationDuration);

        if (Mathf.Abs(aimAngle) >= 90f)
        {
            transform.eulerAngles = new Vector3(0, 0, aimAngle - angle);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, aimAngle + angle);
        }

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

        HealPlayer();

        DealDamage(other);
        PlayHitSound();
    }

    private void HealPlayer()
    {
        if (ammoDetails.healOnHit > 0)
        {
            GameManager.Instance.Player.health.Heal(ammoDetails.healOnHit);
        }
    }
}

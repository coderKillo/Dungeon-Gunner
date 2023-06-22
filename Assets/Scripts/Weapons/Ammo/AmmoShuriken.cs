using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AmmoShuriken : Ammo
{
    [SerializeField] private LayerMask pickUpMask;
    [SerializeField] private float pickUpRadius;
    [SerializeField] private CardSO shurikenCard;

    private List<GameObject> alreadyCollided;

    private bool isFlyingBack = false;

    public override void InitialAmmo(AmmoDetailsSO ammoDetails, float aimAngel, float weaponAngle, float speed, Vector3 weaponAimDirection, int damage, float critChance, bool overrideAmmoMovement = false)
    {
        this.ammoDetails = ammoDetails;
        this.speed = speed;
        this.range = ammoDetails.range;
        this.damage = damage;
        this.critChance = critChance;
        this.alreadyCollided = new List<GameObject>();
        this.fireDirectionVector = HelperUtilities.GetVectorFromAngle(aimAngel);
        this.isFlyingBack = false;

        transform.eulerAngles = new Vector3(0, 0, aimAngel);

        gameObject.SetActive(true);
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

    private void Update()
    {
        var distance = fireDirectionVector * speed * Time.deltaTime;

        transform.Rotate(Vector3.forward, ammoDetails.rotationSpeed * Time.deltaTime);
        transform.position += distance;

        CheckPlayerPickUp(distance);
        CheckRange(distance);
    }

    private void CheckRange(Vector3 distance)
    {
        range -= distance.magnitude;
        if (range >= 0f)
        {
            return;
        }

        if (!isFlyingBack)
        {
            isFlyingBack = true;
            fireDirectionVector = -fireDirectionVector;
            range = ammoDetails.range;
            alreadyCollided.Clear();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void CheckPlayerPickUp(Vector3 distance)
    {
        if (!isFlyingBack)
        {
            return;
        }
        var hit = Physics2D.CircleCast((Vector2)transform.position, pickUpRadius, (Vector2)distance, distance.magnitude, pickUpMask);
        if (!hit)
        {
            return;
        }
        var player = hit.collider.GetComponent<Player>();
        if (player == null)
        {
            return;
        }
        var weapon = player.GetWeapon(shurikenCard.weapon);
        if (weapon == null)
        {
            CardSystem.Instance.Hand.Add(shurikenCard, CardSystem.Instance.Level.GetRandomCardSpawnLevel());

            weapon = player.GetWeapon(shurikenCard.weapon);
            weapon.clipAmmo = 1;
            weapon.totalAmmo = 1;
        }
        else
        {
            weapon.totalAmmo += 1;
        }

        player.reloadWeaponEvent.CallReloadWeaponEvent(weapon);
        gameObject.SetActive(false);
    }
}

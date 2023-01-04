using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class Ammo : MonoBehaviour, IFireable
{
    [SerializeField] private TrailRenderer trailRenderer;

    private float range;
    private float speed;

    private Vector3 fireDirectionVector;
    private float fireDirectionAngle;

    private SpriteRenderer spriteRenderer;
    private AmmoDetailsSO ammoDetails;

    private float chargeTimer;
    private bool ammoMaterialIsSet = false;
    private bool isColliding = false;
    private bool overrideAmmoMovement;

    public void InitialAmmo(AmmoDetailsSO ammoDetails, float aimAngel, float weaponAngle, float speed, Vector3 weaponAimDirection, bool overrideAmmoMovement = false)
    {
        this.ammoDetails = ammoDetails;
        this.chargeTimer = ammoDetails.chargeTime;
        this.range = ammoDetails.range;
        this.speed = speed;
        this.overrideAmmoMovement = overrideAmmoMovement;
        this.isColliding = false;

        spriteRenderer.sprite = ammoDetails.ammoSprite;

        #region AIM DIRECTION
        var randomSpread = Random.Range(ammoDetails.spreadMin, ammoDetails.spreadMax);

        fireDirectionAngle = (weaponAimDirection.magnitude < Settings.useAimAngleDistance) ? aimAngel : weaponAngle;
        fireDirectionAngle += randomSpread * HelperUtilities.RandomSign();

        fireDirectionVector = HelperUtilities.GetVectorFromAngle(fireDirectionAngle);

        transform.eulerAngles = new Vector3(0, 0, fireDirectionAngle);
        #endregion

        #region CHARGE
        if (chargeTimer > 0f)
        {
            spriteRenderer.material = ammoDetails.chargeMaterial;
            ammoMaterialIsSet = false;
        }
        else
        {
            spriteRenderer.material = ammoDetails.ammoMaterial;
            ammoMaterialIsSet = true;
        }
        #endregion

        #region TRAIL
        if (ammoDetails.hasTrail)
        {
            trailRenderer.emitting = true;
            trailRenderer.gameObject.SetActive(true);

            trailRenderer.startWidth = ammoDetails.trailStartWidth;
            trailRenderer.endWidth = ammoDetails.trailEndWidth;
            trailRenderer.material = ammoDetails.trailMaterial;
            trailRenderer.time = ammoDetails.trailLifetime;
        }
        else
        {
            trailRenderer.emitting = false;
            trailRenderer.gameObject.SetActive(false);
        }
        #endregion

        gameObject.SetActive(true);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (chargeTimer > 0f)
        {
            chargeTimer -= Time.deltaTime;
            return;
        }

        if (!ammoMaterialIsSet)
        {
            spriteRenderer.material = ammoDetails.ammoMaterial;
            ammoMaterialIsSet = true;
        }

        var distance = fireDirectionVector * speed * Time.deltaTime;

        if (!overrideAmmoMovement)
        {
            transform.position += distance;
        }

        #region RANGE
        range -= distance.magnitude;
        if (range < 0f)
        {
            gameObject.SetActive(false);
        }
        #endregion

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isColliding)
        {
            return;
        }

        isColliding = true;

        AmmoHitEffect();

        DealDamage(other);

        gameObject.SetActive(false);
    }

    private void DealDamage(Collider2D collider)
    {
        var health = collider.GetComponent<Health>();

        if (health != null)
        {
            health.TakeDamage(ammoDetails.damage);
        }
    }

    private void AmmoHitEffect()
    {
        var visualEffect = ammoDetails.hitVisualEffect;

        if (visualEffect == null)
        {
            return;
        }

        var ammoHitEffect = (SpriteEffect)PoolManager.Instance.ReuseComponent(GameResources.Instance.spriteEffectPrefab, transform.position, Quaternion.identity);
        ammoHitEffect.Initialize(visualEffect);
        ammoHitEffect.gameObject.SetActive(true);
    }
}

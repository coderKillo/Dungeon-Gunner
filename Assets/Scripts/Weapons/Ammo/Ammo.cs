using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Ammo : MonoBehaviour, IFireable
{
    [SerializeField] protected TrailRenderer trailRenderer;

    protected float range;
    protected float speed;
    protected int damage = 0;
    protected float critDamage = 1.5f;
    protected float critChance = 0f;
    protected bool isColliding = false;
    protected AmmoDetailsSO ammoDetails;
    public AmmoDetailsSO AmmoDetails { get { return ammoDetails; } }

    protected Vector3 fireDirectionVector;
    public Vector3 FireDirectionVector { get { return fireDirectionVector; } }

    private float fireDirectionAngle;

    private SpriteRenderer spriteRenderer;
    private PolygonCollider2D polygonCollider;

    private float chargeTimer;
    private bool ammoMaterialIsSet = false;
    private bool overrideAmmoMovement;
    private GameObject onHitEffect;
    private int onHitDamage = 0;
    private float onHitRadius = 0;

    public virtual void InitialAmmo(AmmoDetailsSO ammoDetails, float aimAngel, float weaponAngle, float speed, Vector3 weaponAimDirection, int damage, float critChance, bool overrideAmmoMovement = false)
    {
        this.ammoDetails = ammoDetails;
        this.chargeTimer = ammoDetails.chargeTime;
        this.range = ammoDetails.range;
        this.speed = speed;
        this.overrideAmmoMovement = overrideAmmoMovement;
        this.isColliding = false;
        this.damage = damage;
        this.critChance = critChance;

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

        SetupTrail(ammoDetails);

        gameObject.SetActive(true);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();
    }

    protected void SetupTrail(AmmoDetailsSO detailsSO)
    {
        if (trailRenderer == null)
        {
            return;
        }

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

        PlayHitSound();
        AmmoHitEffect();

        if (DealDamage(other))
        {
            gameObject.SetActive(false);
        }
    }

    protected bool DealDamage(Collider2D collider)
    {
        var health = collider.GetComponent<Health>();

        if (health == null)
        {
            return true;
        }

        if (health.EvadeAttack())
        {
            return false;
        }

        OnHit(collider);

        if (IsCrit())
        {
            health.TakeDamage(Mathf.RoundToInt(damage * critDamage), true);
        }
        else
        {
            health.TakeDamage(damage, false);
        }

        return true;
    }

    protected void PlayHitSound()
    {
        var soundEffect = ammoDetails.hitAudioEffect;
        if (soundEffect == null)
        {
            return;
        }

        SoundEffectManager.Instance.PlaySoundEffect(soundEffect);
    }

    protected bool IsCrit()
    {
        return Random.Range(0, 100) < critChance * 100;
    }

    protected void AmmoHitEffect()
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

    protected void OnHit(Collider2D collider)
    {
        if (onHitEffect == null)
        {
            return;
        }

        var ammoHitEffect = (IOnHit)PoolManager.Instance.ReuseComponent(onHitEffect, transform.position, Quaternion.identity);
        ammoHitEffect.GetGameObject().SetActive(true);
        ammoHitEffect.SetDamage(onHitDamage);
        ammoHitEffect.SetRadius(onHitRadius);
        ammoHitEffect.Hit(collider);
    }

    public void SetOnHitEffect(GameObject onHitEffect, int damage, float radius)
    {
        this.onHitEffect = onHitEffect;
        this.onHitDamage = damage;
        this.onHitRadius = radius;
    }
}

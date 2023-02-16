using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class AmmoMelee : MonoBehaviour, IFireable
{
    private PolygonCollider2D polyCollider;
    private AmmoDetailsSO ammoDetails;
    private bool isColliding = false;
    private float speed = 0f;
    private float range = 0f;
    private int damage = 0;
    private float critDamage = 2.0f;
    private float critChance = 0f;
    private List<GameObject> alreadyCollided;

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    private void Awake()
    {
        polyCollider = GetComponent<PolygonCollider2D>();
    }

    public void InitialAmmo(AmmoDetailsSO ammoDetails, float aimAngel, float weaponAngle, float speed, Vector3 weaponAimDirection, int damage, float critChance, bool overrideAmmoMovement = false)
    {
        this.ammoDetails = ammoDetails;
        this.speed = speed;
        this.range = ammoDetails.range;
        this.alreadyCollided = new List<GameObject>();
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
        if (alreadyCollided.Contains(other.gameObject))
        {
            return;
        }
        alreadyCollided.Add(other.gameObject);

        DealDamage(other);
        PlayHitSound();
    }

    private void PlayHitSound()
    {
        var soundEffect = ammoDetails.hitAudioEffect;
        if (soundEffect == null)
        {
            return;
        }

        SoundEffectManager.Instance.PlaySoundEffect(soundEffect);
    }

    private void DealDamage(Collider2D collider)
    {
        var health = collider.GetComponent<Health>();

        if (health != null)
        {
            if (IsCrit())
            {
                health.TakeDamage((int)(ammoDetails.damage * critDamage), true);
            }
            else
            {
                health.TakeDamage(ammoDetails.damage, false);
            }
        }
    }

    private bool IsCrit()
    {
        return Random.Range(0, 100) < critChance * 100;
    }
}

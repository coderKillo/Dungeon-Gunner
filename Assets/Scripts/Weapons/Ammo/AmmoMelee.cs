using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class AmmoMelee : MonoBehaviour, IFireable
{
    private PolygonCollider2D polyCollider;
    private AmmoDetailsSO ammoDetails;
    private bool isColliding = false;

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    private void Awake()
    {
        polyCollider = GetComponent<PolygonCollider2D>();
    }

    public void InitialAmmo(AmmoDetailsSO ammoDetails, float aimAngel, float weaponAngle, float speed, Vector3 weaponAimDirection, bool overrideAmmoMovement = false)
    {
        this.ammoDetails = ammoDetails;

        transform.eulerAngles = new Vector3(0, 0, aimAngel);

        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        var allOverlappingColliders = new Collider2D[16];
        var contactFilter = new ContactFilter2D();

        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;

        int overlapCount = Physics2D.OverlapCollider(polyCollider, contactFilter, allOverlappingColliders);

        for (int i = 0; i < overlapCount; i++)
        {
            DealDamage(allOverlappingColliders[i]);
        }

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
}

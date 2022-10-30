using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[DisallowMultipleComponent]
public class DealContactDamage : MonoBehaviour
{
    [SerializeField] private int damageAmount;
    [SerializeField] LayerMask mask;

    private bool isColliding = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        ContactDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        ContactDamage(other);
    }

    private void ContactDamage(Collider2D collider)
    {
        if (isColliding)
        {
            return;
        }

        if (!HelperUtilities.IsObjectOnLayerMask(collider.gameObject, mask))
        {
            return;
        }

        var receiver = collider.GetComponent<ReceiveContactDamage>();

        if (receiver == null)
        {
            return;
        }

        isColliding = true;

        receiver.TakeContactDamage(damageAmount);

        Invoke(nameof(ResetCollision), Settings.contactDamageCollisionResetDelay);
    }

    private void ResetCollision()
    {
        isColliding = false;
    }

}

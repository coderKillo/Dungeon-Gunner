using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[DisallowMultipleComponent]
public class DealContactDamage : MonoBehaviour
{
    [SerializeField] private int damageAmount;
    public int Damage { set { damageAmount = value; } }

    [SerializeField] LayerMask mask;

    private List<GameObject> alreadyCollided = new List<GameObject>();

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
        if (alreadyCollided.Contains(collider.gameObject))
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

        alreadyCollided.Add(collider.gameObject);

        receiver.TakeContactDamage(damageAmount);

        Invoke(nameof(ResetCollision), Settings.contactDamageCollisionResetDelay);
    }

    private void ResetCollision()
    {
        alreadyCollided.Clear();
    }

}

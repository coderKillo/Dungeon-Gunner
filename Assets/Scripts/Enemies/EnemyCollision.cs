using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    [SerializeField] private float _pushForce = 2.0f;
    [SerializeField] private float _collisionRadius = 0.5f;
    [SerializeField] private LayerMask _enemyLayer;

    void FixedUpdate()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll((Vector2)transform.position, _collisionRadius, _enemyLayer);
        foreach (Collider2D hitCollider in hitColliders)
        {
            Vector3 pushDirection = (hitCollider.transform.position - transform.position).normalized;
            hitCollider.attachedRigidbody.AddForce(pushDirection * _pushForce, ForceMode2D.Impulse);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _collisionRadius);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Table : MonoBehaviour, IUseable
{
    [SerializeField] private float itemMass;

    private Animator animator;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rigidBody;

    private bool isUsed = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    public void useItem()
    {
        if (isUsed)
        {
            return;
        }

        var closestPointToPlayer = boxCollider.bounds.ClosestPoint(GameManager.Instance.PlayerPosition);

        if (closestPointToPlayer.x == boxCollider.bounds.min.x)
        {
            animator.SetBool(Animations.flipRight, true);
        }
        else if (closestPointToPlayer.x == boxCollider.bounds.max.x)
        {
            animator.SetBool(Animations.flipLeft, true);
        }
        else if (closestPointToPlayer.y == boxCollider.bounds.min.y)
        {
            animator.SetBool(Animations.flipUp, true);
        }
        else // if (closestPointToPlayer.y == boxCollider.bounds.max.y)
        {
            animator.SetBool(Animations.flipDown, true);
        }

        gameObject.layer = LayerMask.NameToLayer("Environment");

        rigidBody.mass = itemMass;

        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.tableFlipSoundEffect);

        isUsed = true;
    }
}

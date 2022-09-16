using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[DisallowMultipleComponent]
public class MovementByVelocity : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private MovementByVelocityEvent movementByVelocityEvent;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
    }

    private void OnEnable()
    {
        movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;
    }

    private void OnDisable()
    {
        movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;
    }

    private void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent arg1, MovementByVelocityArgs arg2)
    {
        MoveRigidbody(arg2.moveDirection, arg2.moveSpeed);
    }

    private void MoveRigidbody(Vector2 moveDirection, float moveSpeed)
    {
        rigidBody.velocity = moveDirection * moveSpeed;
    }
}

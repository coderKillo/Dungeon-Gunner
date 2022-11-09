using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[RequireComponent(typeof(IdleEvent))]
[DisallowMultipleComponent]
public class MovementByVelocity : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private MovementByVelocityEvent movementByVelocityEvent;
    private IdleEvent idleEvent;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        idleEvent = GetComponent<IdleEvent>();
    }

    private void OnEnable()
    {
        movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;
        idleEvent.OnIdle += IdleEvent_OnIdle;
    }

    private void OnDisable()
    {
        movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;
    }

    private void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent arg1, MovementByVelocityArgs arg2)
    {
        MoveRigidbody(arg2.moveDirection, arg2.moveSpeed);
    }

    private void IdleEvent_OnIdle(IdleEvent obj)
    {
        StopRigidbody();
    }

    private void StopRigidbody()
    {
        rigidBody.velocity = Vector3.zero;
    }

    private void MoveRigidbody(Vector2 moveDirection, float moveSpeed)
    {
        rigidBody.velocity = moveDirection * moveSpeed;
    }
}

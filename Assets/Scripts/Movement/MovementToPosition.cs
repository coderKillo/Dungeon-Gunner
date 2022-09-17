using System.Globalization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
public class MovementToPosition : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private MovementToPositionEvent movementToPositionEvent;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
    }

    private void OnEnable()
    {

        movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;

    }

    private void OnDisable()
    {
        movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
    }

    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent arg1, MovementToPositionArgs arg2)
    {
        MoveRigidbody(arg2.movePosition, arg2.currentPosition, arg2.moveSpeed);
    }

    private void MoveRigidbody(Vector3 movePosition, Vector3 currentPosition, float moveSpeed)
    {
        Vector2 directionNormal = Vector3.Normalize(movePosition - currentPosition);

        rigidBody.MovePosition(rigidBody.position + (directionNormal * moveSpeed * Time.fixedDeltaTime));
    }
}

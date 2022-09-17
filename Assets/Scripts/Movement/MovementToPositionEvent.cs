using System.Reflection;
using System;
using UnityEngine;

public class MovementToPositionEvent : MonoBehaviour
{
    public event Action<MovementToPositionEvent, MovementToPositionArgs> OnMovementToPosition;

    public void CallMovementToPositionEvent(Vector3 currentPosition, Vector3 movePosition, float moveSpeed, Vector2 moveDirection, bool isRolling)
    {
        OnMovementToPosition?.Invoke(
            this,
            new MovementToPositionArgs()
            {
                currentPosition = currentPosition,
                movePosition = movePosition,
                moveSpeed = moveSpeed,
                moveDirection = moveDirection,
                isRolling = isRolling
            }
        );
    }
}

public class MovementToPositionArgs
{
    public Vector3 currentPosition;
    public Vector3 movePosition;
    public float moveSpeed;
    public Vector2 moveDirection;
    public bool isRolling;
}
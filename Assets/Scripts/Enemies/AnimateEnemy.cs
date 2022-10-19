using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class AnimateEnemy : MonoBehaviour
{
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        enemy.movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;
        enemy.idleEvent.OnIdle += IdleEvent_OnIdle;
    }

    private void OnDisable()
    {
        enemy.movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
        enemy.idleEvent.OnIdle -= IdleEvent_OnIdle;
    }

    private void IdleEvent_OnIdle(IdleEvent obj)
    {
        SetIdleParameters();
    }

    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent arg1, MovementToPositionArgs arg2)
    {
        SetMovementParameters();

        var aimAngle = HelperUtilities.GetAngleFromVector(GameManager.Instance.PlayerPosition - transform.position);
        var aimDirection = HelperUtilities.GetAimDirection(aimAngle);
        SetDirectionParameters(aimDirection);
    }

    private void SetIdleParameters()
    {
        enemy.animator.SetBool(Animations.isIdle, true);
        enemy.animator.SetBool(Animations.isMoving, false);
    }

    private void SetMovementParameters()
    {
        enemy.animator.SetBool(Animations.isIdle, false);
        enemy.animator.SetBool(Animations.isMoving, true);
    }

    private void SetDirectionParameters(AimDirection direction)
    {
        ResetDirectionParameters();

        switch (direction)
        {
            case AimDirection.Up:
                enemy.animator.SetBool(Animations.aimUp, true);
                break;

            case AimDirection.UpLeft:
                enemy.animator.SetBool(Animations.aimUpLeft, true);
                break;

            case AimDirection.UpRight:
                enemy.animator.SetBool(Animations.aimUpRight, true);
                break;

            case AimDirection.Left:
                enemy.animator.SetBool(Animations.aimLeft, true);
                break;

            case AimDirection.Right:
                enemy.animator.SetBool(Animations.aimRight, true);
                break;

            case AimDirection.Down:
                enemy.animator.SetBool(Animations.aimDown, true);
                break;
        }
    }

    private void ResetDirectionParameters()
    {
        enemy.animator.SetBool(Animations.aimUpRight, false);
        enemy.animator.SetBool(Animations.aimUpLeft, false);
        enemy.animator.SetBool(Animations.aimDown, false);
        enemy.animator.SetBool(Animations.aimUp, false);
        enemy.animator.SetBool(Animations.aimRight, false);
        enemy.animator.SetBool(Animations.aimLeft, false);
    }
}

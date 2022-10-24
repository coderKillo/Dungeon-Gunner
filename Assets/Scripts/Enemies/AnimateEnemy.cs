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
        enemy.idleEvent.OnIdle += IdleEvent_OnIdle;
        enemy.movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;
        enemy.aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
    }

    private void OnDisable()
    {
        enemy.idleEvent.OnIdle -= IdleEvent_OnIdle;
        enemy.movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
        enemy.aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
    }

    private void IdleEvent_OnIdle(IdleEvent obj)
    {
        SetIdleParameters();
    }

    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent arg1, MovementToPositionArgs arg2)
    {
        SetMovementParameters();
    }

    private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent arg1, AimWeaponEventArgs arg2)
    {
        SetDirectionParameters(arg2.aimDirection);
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

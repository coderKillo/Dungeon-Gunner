using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class AnimatePlayer : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        player.idleEvent.OnIdle += IdleEvent_OnIdle;
        player.aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
        player.movementByVelocityEvent.OnMovementByVelocity += MovementByVelocityEvent_OnMovementByVelocity;
        player.movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;
    }

    private void OnDisable()
    {
        player.idleEvent.OnIdle -= IdleEvent_OnIdle;
        player.aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
        player.movementByVelocityEvent.OnMovementByVelocity -= MovementByVelocityEvent_OnMovementByVelocity;
        player.movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;
    }

    private void IdleEvent_OnIdle(IdleEvent obj)
    {
        ResetRollAnimationParameters();
        SetIdleAnimationParameters();
    }

    private void MovementByVelocityEvent_OnMovementByVelocity(MovementByVelocityEvent arg1, MovementByVelocityArgs arg2)
    {
        ResetRollAnimationParameters();
        SetMovementAnimationParameters();
    }

    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent arg1, MovementToPositionArgs arg2)
    {
        ResetAimAnimationParameters();
        ResetRollAnimationParameters();
        SetMovementToPositionAnimationParameters(arg2);
    }

    private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent arg1, AimWeaponEventArgs arg2)
    {
        ResetAimAnimationParameters();
        ResetRollAnimationParameters();
        SetAimWeaponAnimationParameters(arg2.aimDirection);
    }

    private void SetMovementToPositionAnimationParameters(MovementToPositionArgs arg2)
    {
        if (arg2.isRolling)
        {
            if (arg2.moveDirection.x > 0f)
            {
                player.animator.SetBool(Animations.rollRight, true);
            }
            else if (arg2.moveDirection.x < 0f)
            {
                player.animator.SetBool(Animations.rollLeft, true);
            }
            else if (arg2.moveDirection.y > 0f)
            {
                player.animator.SetBool(Animations.rollUp, true);
            }
            else if (arg2.moveDirection.y < 0f)
            {
                player.animator.SetBool(Animations.rollDown, true);
            }
        }
    }


    private void SetMovementAnimationParameters()
    {
        player.animator.SetBool(Animations.isIdle, false);
        player.animator.SetBool(Animations.isMoving, true);
    }

    private void SetIdleAnimationParameters()
    {
        player.animator.SetBool(Animations.isIdle, true);
        player.animator.SetBool(Animations.isMoving, false);
    }

    private void SetAimWeaponAnimationParameters(AimDirection direction)
    {
        switch (direction)
        {
            case AimDirection.Up:
                player.animator.SetBool(Animations.aimUp, true);
                break;

            case AimDirection.UpLeft:
                player.animator.SetBool(Animations.aimUpLeft, true);
                break;

            case AimDirection.UpRight:
                player.animator.SetBool(Animations.aimUpRight, true);
                break;

            case AimDirection.Left:
                player.animator.SetBool(Animations.aimLeft, true);
                break;

            case AimDirection.Right:
                player.animator.SetBool(Animations.aimRight, true);
                break;

            case AimDirection.Down:
                player.animator.SetBool(Animations.aimDown, true);
                break;
        }
    }

    private void ResetAimAnimationParameters()
    {
        player.animator.SetBool(Animations.aimUp, false);
        player.animator.SetBool(Animations.aimUpRight, false);
        player.animator.SetBool(Animations.aimUpLeft, false);
        player.animator.SetBool(Animations.aimRight, false);
        player.animator.SetBool(Animations.aimLeft, false);
        player.animator.SetBool(Animations.aimDown, false);
    }

    private void ResetRollAnimationParameters()
    {
        player.animator.SetBool(Animations.rollDown, false);
        player.animator.SetBool(Animations.rollLeft, false);
        player.animator.SetBool(Animations.rollRight, false);
        player.animator.SetBool(Animations.rollUp, false);
    }
}

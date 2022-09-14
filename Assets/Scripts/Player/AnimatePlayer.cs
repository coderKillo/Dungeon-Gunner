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
    }

    private void OnDisable()
    {
        player.idleEvent.OnIdle -= IdleEvent_OnIdle;
        player.aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
    }

    #region IDLE HANDLER
    private void IdleEvent_OnIdle(IdleEvent obj)
    {
        SetIdleAnimationParameters();
    }

    private void SetIdleAnimationParameters()
    {
        player.animator.SetBool(Animations.isIdle, true);
        player.animator.SetBool(Animations.isMoving, false);
    }
    #endregion

    #region AIM WEAPON HANDLER
    private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent arg1, AimWeaponEventArgs arg2)
    {
        ResetAimAnimationParameters();
        SetAimWeaponAnimationParameters(arg2.aimDirection);
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
    #endregion
}

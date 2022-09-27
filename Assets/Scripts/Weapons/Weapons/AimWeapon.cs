using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AimWeaponEvent))]
[DisallowMultipleComponent]
public class AimWeapon : MonoBehaviour
{
    [SerializeField] private Transform weaponRotationPointTransform;

    private AimWeaponEvent aimWeaponEvent;

    private void Awake()
    {
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
    }

    private void OnEnable()
    {
        aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
    }

    private void OnDisable()
    {
        aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
    }

    private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent arg1, AimWeaponEventArgs arg2)
    {
        Aim(arg2.aimDirection, arg2.aimAngle);
    }

    private void Aim(AimDirection aimDirection, float aimAngle)
    {
        weaponRotationPointTransform.eulerAngles = new Vector3(0f, 0f, aimAngle);

        switch (aimDirection)
        {
            case AimDirection.Left:
            case AimDirection.UpLeft:
                weaponRotationPointTransform.localScale = new Vector3(1f, -1f, 0f);
                break;

            case AimDirection.Up:
            case AimDirection.Right:
            case AimDirection.UpRight:
            case AimDirection.Down:
                weaponRotationPointTransform.localScale = new Vector3(1f, 1f, 0f);
                break;
        }
    }

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponRotationPointTransform), weaponRotationPointTransform);
    }
#endif
    #endregion
}

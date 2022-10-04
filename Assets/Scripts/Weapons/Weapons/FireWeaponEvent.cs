using System;
using UnityEngine;

[DisallowMultipleComponent]
public class FireWeaponEvent : MonoBehaviour
{
    public event Action<FireWeaponEvent, FireWeaponEventArgs> OnFireWeapon;

    public void CallFireWeaponEvent(bool fire, bool fireLastFrame, AimDirection aimDirection, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        OnFireWeapon?.Invoke(
            this,
            new FireWeaponEventArgs()
            {
                fire = fire,
                fireLastFrame = fireLastFrame,
                aimDirection = aimDirection,
                aimAngle = aimAngle,
                weaponAimAngle = weaponAimAngle,
                weaponAimDirectionVector = weaponAimDirectionVector,
            }
        );
    }
}

public class FireWeaponEventArgs : EventArgs
{
    public bool fire;
    public bool fireLastFrame;
    public AimDirection aimDirection;
    public float aimAngle;
    public float weaponAimAngle;
    public Vector3 weaponAimDirectionVector;
}
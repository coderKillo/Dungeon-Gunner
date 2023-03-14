using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ChargeWeaponEvent : MonoBehaviour
{
    public Action<ChargeWeaponEvent, ChargeWeaponEventArgs> OnChargeWeapon;

    public void CallChargeWeaponEvent(float chargeTime, bool active)
    {
        OnChargeWeapon?.Invoke(this, new ChargeWeaponEventArgs()
        {
            chargeTime = chargeTime,
            active = active
        });
    }
}

public class ChargeWeaponEventArgs : EventArgs
{
    public float chargeTime;
    public bool active;
}
using System;
using UnityEngine;

[DisallowMultipleComponent]
public class ReloadWeaponEvent : MonoBehaviour
{
    public event Action<ReloadWeaponEvent, ReloadWeaponEventArgs> OnReloadWeapon;

    public void CallReloadWeaponEvent(Weapon weapon, int totalUpAmmoPercent = 0)
    {
        OnReloadWeapon?.Invoke(
            this,
            new ReloadWeaponEventArgs()
            {
                weapon = weapon,
                totalUpAmmoPercent = totalUpAmmoPercent,
            }
        );
    }
}

public class ReloadWeaponEventArgs : EventArgs
{
    public Weapon weapon;
    public int totalUpAmmoPercent;
}

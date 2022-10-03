using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[DisallowMultipleComponent]
public class ReloadWeapon : MonoBehaviour
{
    private SetActiveWeaponEvent setActiveWeaponEvent;
    private ReloadWeaponEvent reloadWeaponEvent;
    private WeaponReloadedEvent weaponReloadedEvent;

    private Coroutine reloadCoroutine;

    private void Awake()
    {
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
    }

    private void OnEnable()
    {
        reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnReloadWeapon;
        setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
    }

    private void OnDisable()
    {
        reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnReloadWeapon;
        setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
    }

    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent arg1, SetActiveWeaponEventArgs arg2)
    {
        if (arg2.weapon.isReloading)
        {
            StartReloadWeapon(arg2.weapon, 0);
        }
    }

    private void ReloadWeaponEvent_OnReloadWeapon(ReloadWeaponEvent arg1, ReloadWeaponEventArgs arg2)
    {
        StartReloadWeapon(arg2.weapon, arg2.totalUpAmmoPercent);
    }

    private void StartReloadWeapon(Weapon weapon, int totalUpAmmoPercent)
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
        }

        reloadCoroutine = StartCoroutine(ReloadCoroutine(weapon, totalUpAmmoPercent));
    }

    private IEnumerator ReloadCoroutine(Weapon weapon, int totalUpAmmoPercent)
    {
        weapon.isReloading = true;
        weapon.reloadTimer = weapon.weaponDetails.reloadTime;

        while (weapon.reloadTimer > 0)
        {
            weapon.reloadTimer -= Time.deltaTime;
            yield return null;
        }

        if (totalUpAmmoPercent > 0)
        {
            FillUpAmmo(weapon, totalUpAmmoPercent);
        }

        Reload(weapon);

        weapon.reloadTimer = 0f;
        weapon.isReloading = false;

        ReloadComplete(weapon);
    }

    private void FillUpAmmo(Weapon weapon, int percentAmount)
    {
        var increase = Mathf.RoundToInt(weapon.totalAmmo * (percentAmount / 100f));
        weapon.totalAmmo = Mathf.Clamp(weapon.totalAmmo + increase, 0, weapon.weaponDetails.ammoCapacity);
    }

    private void Reload(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteAmmo)
        {
            weapon.clipAmmo = weapon.weaponDetails.ammoClipCapacity;
            return;
        }

        weapon.clipAmmo = Mathf.Clamp(weapon.totalAmmo, 0, weapon.weaponDetails.ammoCapacity);
    }

    private void ReloadComplete(Weapon weapon)
    {
        weaponReloadedEvent.CallWeaponReloadedEvent(weapon);
    }
}

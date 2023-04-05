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
    [Space(10)]
    [Header("Fast Reload")]
    [SerializeField] private float minFastReloadThreshold = 0.4f;
    [SerializeField] private float maxFastReloadThreshold = 0.6f;

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
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
        }

        if (arg2.weapon.isReloading)
        {
            StartReloadWeapon(arg2.weapon);
        }
    }

    private void ReloadWeaponEvent_OnReloadWeapon(ReloadWeaponEvent arg1, ReloadWeaponEventArgs arg2)
    {
        if (FastReload(arg2.weapon))
        {
            Reload(arg2.weapon);
            ReloadComplete(arg2.weapon);
            return;
        }

        StartReloadWeapon(arg2.weapon);
    }

    private void StartReloadWeapon(Weapon weapon)
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
        }

        reloadCoroutine = StartCoroutine(ReloadCoroutine(weapon));
    }

    private IEnumerator ReloadCoroutine(Weapon weapon)
    {
        weapon.isReloading = true;
        weapon.reloadTimer = weapon.weaponDetails.reloadTime;

        ReloadSoundEffect(weapon);

        while (weapon.reloadTimer > 0)
        {
            weapon.reloadTimer -= Time.deltaTime;
            yield return null;
        }

        Reload(weapon);
        ReloadComplete(weapon);
    }

    private void Reload(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteAmmo)
        {
            weapon.clipAmmo = weapon.weaponDetails.ammoClipCapacity;
            return;
        }

        weapon.clipAmmo = Mathf.Clamp(weapon.totalAmmo, 0, weapon.weaponDetails.ammoClipCapacity);
    }

    private void ReloadComplete(Weapon weapon)
    {
        weapon.reloadTimer = 0f;
        weapon.isReloading = false;

        weaponReloadedEvent.CallWeaponReloadedEvent(weapon);
    }

    private bool FastReload(Weapon weapon)
    {
        if (!weapon.isReloading)
        {
            return false;
        }

        var relativeReloadTime = weapon.reloadTimer / weapon.weaponDetails.reloadTime;

        if (relativeReloadTime < minFastReloadThreshold || relativeReloadTime > maxFastReloadThreshold)
        {
            return false;
        }

        return true;
    }

    private void ReloadSoundEffect(Weapon weapon)
    {
        var soundEffect = weapon.weaponDetails.reloadSoundEffect;
        if (soundEffect == null)
        {
            return;
        }

        SoundEffectManager.Instance.PlaySoundEffect(weapon.weaponDetails.reloadSoundEffect);
    }
}

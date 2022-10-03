using UnityEngine;

[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(ActiveWeapon))]
[DisallowMultipleComponent]
public class FireWeapon : MonoBehaviour
{
    private float fireRateCooldownTimer = 0f;

    private FireWeaponEvent fireWeaponEvent;
    private WeaponFiredEvent weaponFiredEvent;
    private ReloadWeaponEvent reloadWeaponEvent;
    private ActiveWeapon activeWeapon;

    private void Awake()
    {
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        activeWeapon = GetComponent<ActiveWeapon>();
    }

    private void OnEnable()
    {
        fireWeaponEvent.OnFireWeapon += FireWeaponEvent_OnFireWeapon;
    }

    private void OnDisable()
    {
        fireWeaponEvent.OnFireWeapon -= FireWeaponEvent_OnFireWeapon;
    }

    private void Update()
    {
        fireRateCooldownTimer -= Time.deltaTime;
    }

    private void FireWeaponEvent_OnFireWeapon(FireWeaponEvent arg1, FireWeaponEventArgs arg2)
    {
        if (!arg2.fire)
        {
            return;
        }

        if (!IsWeaponReadyToFire())
        {
            return;
        }

        FireAmmo(arg2.aimAngle, arg2.weaponAimAngle, arg2.weaponAimDirectionVector);

        if (IsClipEmpty())
        {
            ReloadWeapon();
        }

        ResetCooldownTimer();
    }

    private bool IsWeaponReadyToFire()
    {
        if (fireRateCooldownTimer > 0f)
        {
            return false;
        }

        if (activeWeapon.CurrentWeapon.isReloading)
        {
            return false;
        }

        if (IsAmmoEmpty())
        {
            return false;
        }

        if (IsClipEmpty())
        {
            return false;
        }

        return true;
    }

    private void FireAmmo(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        if (activeWeapon.CurrentAmmo == null)
        {
            return;
        }

        var speed = Random.Range(activeWeapon.CurrentAmmo.speedMin, activeWeapon.CurrentAmmo.speedMax);
        var prefab = activeWeapon.CurrentAmmo.prefabArray[Random.Range(0, activeWeapon.CurrentAmmo.prefabArray.Length)];

        var ammo = (IFireable)PoolManager.Instance.ReuseComponent(prefab, activeWeapon.ShootPosition, Quaternion.identity);
        ammo.InitialAmmo(activeWeapon.CurrentAmmo, aimAngle, weaponAimAngle, speed, weaponAimDirectionVector);

        if (!activeWeapon.CurrentWeapon.weaponDetails.hasInfiniteClipCapacity)
        {
            activeWeapon.CurrentWeapon.clipAmmo--;
            activeWeapon.CurrentWeapon.totalAmmo--;
        }

        weaponFiredEvent.CallWeaponFiredEvent(activeWeapon.CurrentWeapon);
    }

    private void ResetCooldownTimer()
    {
        fireRateCooldownTimer = activeWeapon.CurrentWeapon.weaponDetails.fireRate;
    }

    private void ReloadWeapon()
    {
        reloadWeaponEvent.CallReloadWeaponEvent(activeWeapon.CurrentWeapon);
    }

    private bool IsAmmoEmpty()
    {
        return (activeWeapon.CurrentWeapon.totalAmmo <= 0 && !activeWeapon.CurrentWeapon.weaponDetails.hasInfiniteAmmo);
    }

    private bool IsClipEmpty()
    {
        return (activeWeapon.CurrentWeapon.clipAmmo <= 0 && !activeWeapon.CurrentWeapon.weaponDetails.hasInfiniteClipCapacity);
    }

}

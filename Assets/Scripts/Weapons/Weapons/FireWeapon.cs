using System.Collections;
using UnityEngine;
using MoreMountains.Feedbacks;

[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(ActiveWeapon))]
[DisallowMultipleComponent]
public class FireWeapon : MonoBehaviour
{
    [SerializeField] private MMF_Player fireFeedback;

    private float fireRateCooldownTimer = 0f;
    private float chargeTimer = 0f;

    private FireWeaponEvent fireWeaponEvent;
    private WeaponFiredEvent weaponFiredEvent;
    private ReloadWeaponEvent reloadWeaponEvent;
    private ActiveWeapon activeWeapon;
    private SpriteEffect fireWeaponEffect;

    private void Awake()
    {
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        activeWeapon = GetComponent<ActiveWeapon>();
        fireWeaponEffect = activeWeapon.ShootEffect.GetComponent<SpriteEffect>();
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
        if (fireRateCooldownTimer > 0f)
        {
            fireRateCooldownTimer -= Time.deltaTime;
        }

        if (chargeTimer > 0f)
        {
            chargeTimer -= Time.deltaTime;
        }
    }

    private void FireWeaponEvent_OnFireWeapon(FireWeaponEvent arg1, FireWeaponEventArgs arg2)
    {
        if (!arg2.fire)
        {
            return;
        }

        if (IsFirePressedFrame(arg2))
        {
            ResetChargeTimer();
        }

        if (!IsWeaponReadyToFire())
        {
            return;
        }

        FireAmmo(arg2.aimAngle, arg2.weaponAimAngle, arg2.weaponAimDirectionVector);

        ResetFireRateCooldownTimer();
        ResetChargeTimer();
    }

    private bool IsWeaponReadyToFire()
    {
        if (fireRateCooldownTimer > 0f)
        {
            return false;
        }

        if (chargeTimer > 0f)
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

        FireWeaponSoundEffect();

        FireWeaponVisualEffect(aimAngle);

        fireFeedback.PlayFeedbacks();

        PlayFireAnimation();

        StartCoroutine(FireAmmoCoroutine(aimAngle, weaponAimAngle, weaponAimDirectionVector));
    }

    private void PlayFireAnimation()
    {
        if (activeWeapon.Animator == null)
        {
            return;
        }

        activeWeapon.Animator.SetTrigger(Animations.shot);
    }

    private IEnumerator FireAmmoCoroutine(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        var spawnAmount = Random.Range(activeWeapon.CurrentAmmo.spawnAmmoMin, activeWeapon.CurrentAmmo.spawnAmmoMax);
        var spawnInterval = Random.Range(activeWeapon.CurrentAmmo.spawnIntervalMin, activeWeapon.CurrentAmmo.spawnIntervalMax);
        spawnInterval = (spawnAmount > 1) ? spawnInterval : 0f;

        for (int i = 0; i < spawnAmount; i++)
        {
            var speed = Random.Range(activeWeapon.CurrentAmmo.speedMin, activeWeapon.CurrentAmmo.speedMax);
            var prefab = activeWeapon.CurrentAmmo.prefabArray[Random.Range(0, activeWeapon.CurrentAmmo.prefabArray.Length)];

            var ammo = (IFireable)PoolManager.Instance.ReuseComponent(prefab, activeWeapon.ShootPosition, Quaternion.identity);
            ammo.InitialAmmo(activeWeapon.CurrentAmmo, aimAngle, weaponAimAngle, speed, weaponAimDirectionVector);

            yield return new WaitForSeconds(spawnInterval);
        }

        if (!activeWeapon.CurrentWeapon.weaponDetails.hasInfiniteClipCapacity)
        {
            activeWeapon.CurrentWeapon.clipAmmo--;
            activeWeapon.CurrentWeapon.totalAmmo--;
        }

        if (IsClipEmpty())
        {
            ReloadWeapon();
        }

        weaponFiredEvent.CallWeaponFiredEvent(activeWeapon.CurrentWeapon);
    }

    private void ResetFireRateCooldownTimer()
    {
        fireRateCooldownTimer = activeWeapon.CurrentWeapon.weaponDetails.fireRate;
    }

    private void ResetChargeTimer()
    {
        chargeTimer = activeWeapon.CurrentWeapon.weaponDetails.prechargeTime;
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

    private bool IsFirePressedFrame(FireWeaponEventArgs arg2)
    {
        return arg2.fire && !arg2.fireLastFrame;
    }

    private void FireWeaponSoundEffect()
    {
        var soundEffect = activeWeapon.CurrentWeapon.weaponDetails.fireSoundEffect;
        if (soundEffect == null)
        {
            return;
        }

        SoundEffectManager.Instance.PlaySoundEffect(soundEffect);
    }

    private void FireWeaponVisualEffect(float aimAngle)
    {
        var visualEffect = activeWeapon.CurrentWeapon.weaponDetails.fireVisualEffect;

        if (visualEffect == null)
        {
            return;
        }

        fireWeaponEffect.Initialize(visualEffect);
        fireWeaponEffect.gameObject.SetActive(true);
    }

}

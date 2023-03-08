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
    [SerializeField] private float multiShotOffset = 10f;

    private int multiShot = 1;
    public int MultiShot { set { multiShot = value; } }

    private float fireRateCooldownTimer = 0f;
    private float chargeTimer = 0f;

    private float weaponDamageFactor = 1f;
    private float weaponCritChanceFactor = 1f;
    public float WeaponCritChanceFactor { get { return weaponCritChanceFactor; } set { weaponCritChanceFactor = value; } }

    private FireWeaponEvent fireWeaponEvent;
    private WeaponFiredEvent weaponFiredEvent;
    private ReloadWeaponEvent reloadWeaponEvent;
    private ActiveWeapon activeWeapon;
    private SpriteEffect fireWeaponEffect;

    enum WeaponState
    {
        Idle,
        Charge,
        Shot
    }

    private WeaponState state = WeaponState.Idle;

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
        if (!IsWeaponReadyToFire())
        {
            return;
        }

        switch (state)
        {
            case WeaponState.Idle:

                ResetChargeTimer();

                if (chargeTimer > 0f)
                {
                    PlayChargeAnimation();
                    state = WeaponState.Charge;
                }
                else
                {
                    state = WeaponState.Shot;
                }

                break;


            case WeaponState.Charge:

                if (!arg2.fire)
                {
                    StopChargingAnimation();
                    state = WeaponState.Idle;
                    return;
                }

                if (chargeTimer <= 0f)
                {
                    StopChargingAnimation();
                    state = WeaponState.Shot;
                }

                break;


            case WeaponState.Shot:

                FireAmmo(arg2.aimAngle, arg2.weaponAimAngle, arg2.weaponAimDirectionVector);
                ResetFireRateCooldownTimer();

                state = WeaponState.Idle;

                break;

            default:
                break;
        }
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

    private void PlayChargeAnimation()
    {
        if (activeWeapon.Animator == null)
        {
            return;
        }

        activeWeapon.Animator.SetTrigger(Animations.charge);
    }

    private void StopChargingAnimation()
    {
        if (activeWeapon.Animator == null)
        {
            return;
        }

        activeWeapon.Animator.SetTrigger(Animations.cancelCharge);
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
            var damage = Mathf.RoundToInt(activeWeapon.CurrentAmmo.damage * activeWeapon.CurrentWeapon.damageFactor * weaponDamageFactor);
            var critChance = activeWeapon.CurrentAmmo.critChance * weaponCritChanceFactor;

            for (int j = 0; j < multiShot; j++)
            {
                var sign = j % 2 > 0 ? -1 : 1;
                var offset = (j + 1) / 2;

                var offsetAngle = sign * offset * multiShotOffset;
                var offsetVector = HelperUtilities.GetVectorFromAngle(offsetAngle);

                var ammo = (IFireable)PoolManager.Instance.ReuseComponent(prefab, activeWeapon.ShootPosition, Quaternion.identity);
                ammo.InitialAmmo(activeWeapon.CurrentAmmo, aimAngle + offsetAngle, weaponAimAngle + offsetAngle, speed, weaponAimDirectionVector + offsetVector, damage, critChance);
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        if (!activeWeapon.CurrentWeapon.weaponDetails.hasInfiniteClipCapacity)
        {
            activeWeapon.CurrentWeapon.clipAmmo--;
            activeWeapon.CurrentWeapon.totalAmmo--;
        }

        if (IsAmmoEmpty() && activeWeapon.CurrentWeapon.weaponDetails.removeAfterAmmoIsEmpty)
        {
            GetComponent<Player>().RemoveWeaponFromPlayer(activeWeapon.CurrentWeapon.weaponDetails);
        }
        else if (IsClipEmpty())
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

    private bool IsFireReleaseFrame(FireWeaponEventArgs arg2)
    {
        return !arg2.fire && arg2.fireLastFrame;
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

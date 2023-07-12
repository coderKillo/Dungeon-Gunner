using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(AnimatePlayer))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(MovementByVelocity))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(FireWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(DestroyedEvent))]
[RequireComponent(typeof(ChargeWeaponEvent))]
[RequireComponent(typeof(BuffEvent))]
[RequireComponent(typeof(PlayerDied))]
[RequireComponent(typeof(PlayerDash))]
[RequireComponent(typeof(PostHitImmunity))]
[RequireComponent(typeof(PlayerPowerUp))]
[RequireComponent(typeof(PlayerCardHand))]
#endregion
[DisallowMultipleComponent]
public class Player : MonoBehaviour
{
    [SerializeField] private SpriteRenderer handRenderer;

    [HideInInspector] public PlayerDetailsSO playerDetails;
    [HideInInspector] public PlayerControl playerControl;
    [HideInInspector] public PlayerPowerUp playerPowerUp;
    [HideInInspector] public PlayerReflectAmmo playerReflectAmmo;
    [HideInInspector] public PlayerDash playerDash;
    [HideInInspector] public PlayerCardHand playerCardHand;
    [HideInInspector] public Health health;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Animator animator;
    [HideInInspector] public ActiveWeapon activeWeapon;
    [HideInInspector] public PostHitImmunity postHitImmunity;
    [HideInInspector] public FireWeapon fireWeapon;
    #region EVENTS
    [HideInInspector] public AimWeaponEvent aimWeaponEvent;
    [HideInInspector] public IdleEvent idleEvent;
    [HideInInspector] public MovementByVelocityEvent movementByVelocityEvent;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public SetActiveWeaponEvent setActiveWeaponEvent;
    [HideInInspector] public FireWeaponEvent fireWeaponEvent;
    [HideInInspector] public WeaponFiredEvent weaponFiredEvent;
    [HideInInspector] public ReloadWeaponEvent reloadWeaponEvent;
    [HideInInspector] public WeaponReloadedEvent weaponReloadedEvent;
    [HideInInspector] public HealthEvent healthEvent;
    [HideInInspector] public DestroyedEvent destroyedEvent;
    [HideInInspector] public ChargeWeaponEvent chargeWeaponEvent;
    [HideInInspector] public BuffEvent buffEvent;
    #endregion

    public List<Weapon> weaponList = new List<Weapon>();

    private void Awake()
    {
        health = GetComponent<Health>();
        playerControl = GetComponent<PlayerControl>();
        playerPowerUp = GetComponent<PlayerPowerUp>();
        playerReflectAmmo = GetComponent<PlayerReflectAmmo>();
        playerDash = GetComponent<PlayerDash>();
        playerCardHand = GetComponent<PlayerCardHand>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        activeWeapon = GetComponent<ActiveWeapon>();
        postHitImmunity = GetComponent<PostHitImmunity>();
        fireWeapon = GetComponent<FireWeapon>();
        #region EVENTS
        idleEvent = GetComponent<IdleEvent>();
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
        healthEvent = GetComponent<HealthEvent>();
        destroyedEvent = GetComponent<DestroyedEvent>();
        chargeWeaponEvent = GetComponent<ChargeWeaponEvent>();
        buffEvent = GetComponent<BuffEvent>();
        #endregion
    }

    public void Initialize(PlayerDetailsSO details)
    {
        playerDetails = details;

        health.StartingHealth = details.healthAmount;
        postHitImmunity.immunityTime = details.hitImmunityTime;
        handRenderer.sprite = details.handSprite;

        weaponList.Clear();
    }

    private void OnEnable()
    {
        healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
        CardSystemLevel.OnLevelChange += CardSystemLevel_OnLevelChange;
    }

    private void OnDisable()
    {
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
        CardSystemLevel.OnLevelChange -= CardSystemLevel_OnLevelChange;
    }

    public Weapon AddWeaponToPlayer(WeaponDetailsSO weaponDetails)
    {
        var weapon = Weapon.CreateWeapon(weaponDetails);

        weaponList.Add(weapon);
        weapon.weaponListPosition = weaponList.Count;

        setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);

        return weapon;
    }

    public void RemoveWeaponFromPlayer(Guid id)
    {
        var weapon = GetWeapon(id);

        if (weapon == null)
        {
            return;
        }

        RemoveWeapon(weapon);
    }

    public void RemoveWeaponFromPlayer(WeaponDetailsSO weaponDetails)
    {
        var weapon = GetWeapon(weaponDetails);

        if (weapon == null)
        {
            return;
        }

        RemoveWeapon(weapon);
    }

    private void RemoveWeapon(Weapon weapon)
    {
        weaponList.Remove(weapon);
        weapon.weaponListPosition = 0;

        setActiveWeaponEvent.CallSetActiveWeaponEvent(weaponList[0]);
    }

    public Weapon[] GetAllWeapons()
    {
        return weaponList.ToArray();
    }

    public Weapon GetWeapon(WeaponDetailsSO weaponDetails)
    {
        return weaponList.Find((x) => x.weaponDetails == weaponDetails);
    }

    public Weapon GetWeapon(Guid id)
    {
        return weaponList.Find((x) => x.id == id);
    }

    public bool HasWeapon(WeaponDetailsSO weaponDetails)
    {
        foreach (var weapon in weaponList)
        {
            if (weapon.weaponDetails == weaponDetails)
            {
                return true;
            }
        }

        return false;
    }

    private void HealthEvent_OnHealthChanged(HealthEvent arg1, HealthEventArgs arg2)
    {
        if (arg2.healAmount > 0)
        {
            DamagePopup.Create(transform.position, "+" + arg2.healAmount.ToString(), Color.green);
        }

        if (arg2.healthAmount <= 0)
        {
            destroyedEvent.CallDestroyedEvent();
        }
    }

    private void CardSystemLevel_OnLevelChange(int level)
    {
        if (level > 1)
        {
            health.IncreaseMaxHealth(Settings.playerIncreaseHealthOnLevelUp);
        }
    }

    public bool IsDashing()
    {
        return playerDash.IsDashing();
    }

    public bool EnablePlayer(bool enabled)
    {
        return playerControl.isEnabled = enabled;
    }
}

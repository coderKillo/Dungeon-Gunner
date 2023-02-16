using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(SetActiveWeaponEvent))]
[DisallowMultipleComponent]
public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform weaponPositionTransform;
    [SerializeField] private PolygonCollider2D polygonCollider2D;

    [SerializeField] private Animator animator;
    public Animator Animator { get { return animator; } }

    [SerializeField] private Transform shootPositionTransform;
    public Vector3 ShootPosition { get { return shootPositionTransform.position; } }

    [SerializeField] private Transform shootEffectTransform;
    public Transform ShootEffect { get { return shootEffectTransform; } }

    private SetActiveWeaponEvent setActiveWeaponEvent;
    private AnimatorOverrideController animatorOverrideController;
    private AnimationClipOverrides clipOverrides;

    [ShowInInspector] private Weapon currentWeapon;
    public Weapon CurrentWeapon { get { return currentWeapon; } }
    public AmmoDetailsSO CurrentAmmo { get { return currentWeapon.weaponDetails.ammo; } }

    public void RemoveWeapon()
    {
        currentWeapon = null;
    }

    private void Awake()
    {
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();

        if (animator != null)
        {
            animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = animatorOverrideController;

            clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
            animatorOverrideController.GetOverrides(clipOverrides);
        }
    }

    private void LateUpdate()
    {
        if (animator == null)
        {
            return;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            spriteRenderer.sprite = currentWeapon.weaponDetails.weaponSprite;
        }
    }

    private void OnEnable()
    {
        setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
    }

    private void OnDisable()
    {
        setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
    }

    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent arg1, SetActiveWeaponEventArgs arg2)
    {
        SetWeapon(arg2.weapon);
    }

    private void SetWeapon(Weapon weapon)
    {
        currentWeapon = weapon;

        SetAnimations();

        spriteRenderer.sprite = weapon.weaponDetails.weaponSprite;

        SetWeaponPolygonCollider();

        shootPositionTransform.localPosition = weapon.weaponDetails.shootPosition;
        weaponPositionTransform.localPosition = new Vector3(weapon.weaponDetails.positionOffset, 0f, 0f);
    }

    private void SetWeaponPolygonCollider()
    {
        if (spriteRenderer.sprite == null)
        {
            return;
        }

        if (polygonCollider2D == null)
        {
            return;
        }

        var spritePhysicalShapePoints = new List<Vector2>();
        spriteRenderer.sprite.GetPhysicsShape(0, spritePhysicalShapePoints);
        polygonCollider2D.points = spritePhysicalShapePoints.ToArray();
    }

    private void SetAnimations()
    {
        if (animator == null)
        {
            return;
        }

        if (currentWeapon.weaponDetails.shotAnimation != null)
        {
            animatorOverrideController["Shot"] = currentWeapon.weaponDetails.shotAnimation;
            // animator.speed = currentWeapon.weaponDetails.shotAnimation.length / currentWeapon.weaponDetails.fireRate;
        }
        else
        {
            animatorOverrideController["Shot"] = GameResources.Instance.emptyAnimationClip;
            // animator.speed = 1;
        }

        if (currentWeapon.weaponDetails.chargeAnimation != null)
        {
            animatorOverrideController["Charge"] = currentWeapon.weaponDetails.chargeAnimation;
        }
        else
        {
            animatorOverrideController["Charge"] = GameResources.Instance.emptyAnimationClip;
        }

    }

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(spriteRenderer), spriteRenderer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(polygonCollider2D), polygonCollider2D);
        HelperUtilities.ValidateCheckNullValue(this, nameof(shootEffectTransform), shootEffectTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(shootPositionTransform), shootPositionTransform);
    }
#endif
    #endregion
}

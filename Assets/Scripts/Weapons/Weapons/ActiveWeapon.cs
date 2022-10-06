using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SetActiveWeaponEvent))]
[DisallowMultipleComponent]
public class ActiveWeapon : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PolygonCollider2D polygonCollider2D;

    [SerializeField] private Transform shootPositionTransform;
    public Vector3 ShootPosition { get { return shootPositionTransform.position; } }

    [SerializeField] private Transform shootEffectTransform;
    public Vector3 ShootEffectPosition { get { return shootEffectTransform.position; } }

    private SetActiveWeaponEvent setActiveWeaponEvent;

    private Weapon currentWeapon;
    public Weapon CurrentWeapon { get { return currentWeapon; } }
    public AmmoDetailsSO CurrentAmmo { get { return currentWeapon.weaponDetails.ammo; } }

    public void RemoveWeapon()
    {
        currentWeapon = null;
    }

    private void Awake()
    {
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
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

        spriteRenderer.sprite = weapon.weaponDetails.weaponSprite;

        SetWeaponPolygonCollider();

        shootPositionTransform.localPosition = weapon.weaponDetails.shootPosition;
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

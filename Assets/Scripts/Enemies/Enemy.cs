using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#region REQUIRED COMPONENT
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AnimateEnemy))]
[RequireComponent(typeof(MaterializeEffect))]
[RequireComponent(typeof(EnemyMovementAI))]
[RequireComponent(typeof(EnemyWeaponAI))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(FireWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(ReloadWeapon))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponReloadedEvent))]
#endregion
[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{
    [HideInInspector] public EnemyDetailsSO enemyDetails;
    [HideInInspector] public SpriteRenderer[] spriteRenderer;

    private CircleCollider2D circleCollider;
    private PolygonCollider2D polygonCollider;
    private EnemyMovementAI enemyMovementAI;
    private MaterializeEffect materializeEffect;
    private FireWeapon fireWeapon;
    private SetActiveWeaponEvent setActiveWeaponEvent;
    #region EVENTS
    [HideInInspector] public IdleEvent idleEvent;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public AimWeaponEvent aimWeaponEvent;
    [HideInInspector] public FireWeaponEvent fireWeaponEvent;
    [HideInInspector] public Animator animator;
    #endregion

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        polygonCollider = GetComponent<PolygonCollider2D>();
        spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
        materializeEffect = GetComponent<MaterializeEffect>();
        animator = GetComponent<Animator>();
        enemyMovementAI = GetComponent<EnemyMovementAI>();
        fireWeapon = GetComponent<FireWeapon>();
        idleEvent = GetComponent<IdleEvent>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
    }

    public void Initialize(EnemyDetailsSO enemyDetails, int enemySpawnNumber, DungeonLevelSO dungeonLevel)
    {
        this.enemyDetails = enemyDetails;

        enemyMovementAI.updateFrameNumber = enemySpawnNumber % Settings.targetFrameRateToSpreadRebuildPath;

        SetEnemyAnimationSpeed();

        SetEnemyStartingWeapon();

        StartCoroutine(MaterializeEnemy());
    }

    private void SetEnemyAnimationSpeed()
    {
        animator.speed = enemyMovementAI.moveSpeed / Animations.enemyAnimationBaseSpeed;
    }

    private void SetEnemyStartingWeapon()
    {
        if (enemyDetails.weaponDetails == null)
        {
            return;
        }

        var weapon = Weapon.CreateWeapon(enemyDetails.weaponDetails);

        setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);
    }

    private IEnumerator MaterializeEnemy()
    {
        EnableEnemy(false);

        yield return materializeEffect.MaterializeRoutine(
            enemyDetails.materializeShader,
            enemyDetails.materializeColor,
            enemyDetails.materializeTime,
            spriteRenderer,
            enemyDetails.standardMaterial);

        EnableEnemy(true);
    }


    private void EnableEnemy(bool enable)
    {
        circleCollider.enabled = enable;
        polygonCollider.enabled = enable;
        enemyMovementAI.enabled = enable;
        fireWeapon.enabled = enabled;
    }
}

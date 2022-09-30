using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class PlayerControl : MonoBehaviour
{
    [SerializeField] private MovementDetailsSO movementDetails;

    private Player player;
    private float moveSpeed;

    private Coroutine rollCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    private bool isRolling = false;
    private float rollingCooldownTimer = 0f;

    private int currentWeaponIndex = 1;

    private void Awake()
    {
        player = GetComponent<Player>();
        moveSpeed = movementDetails.GetRandomMovementSpeed();
    }

    private void Start()
    {
        waitForFixedUpdate = new WaitForFixedUpdate();

        SetPlayerAnimationSpeed();

        SetStartingWeapon();
    }

    private void Update()
    {
        if (isRolling)
        {
            return;
        }

        MovementInput();

        WeaponInput();

        RollCooldownTimer();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        StopRollingCoroutine();
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        StopRollingCoroutine();
    }

    private void WeaponInput()
    {
        #region AIM WEAPON
        var mouseWorldPosition = HelperUtilities.GetWorldMousePosition();

        var weaponDirection = (mouseWorldPosition - player.activeWeapon.ShootPosition);
        var playerDirection = (mouseWorldPosition - transform.position);

        var weaponAngle = HelperUtilities.GetAngleFromVector(weaponDirection);
        var playerAngle = HelperUtilities.GetAngleFromVector(playerDirection);

        var playerAimDirection = HelperUtilities.GetAimDirection(playerAngle);

        player.aimWeaponEvent.CallWeaponAimEvent(playerAimDirection, playerAngle, weaponAngle, weaponDirection);
        #endregion

        #region FIRE WEAPON
        bool fireWeapon = Input.GetMouseButton(0);
        if (fireWeapon)
        {
            player.fireWeaponEvent.CallFireWeaponEvent(true, playerAimDirection, playerAngle, weaponAngle, weaponDirection);
        }
        #endregion
    }

    private void MovementInput()
    {
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");
        bool shiftButtonPressed = Input.GetKeyDown(KeyCode.LeftShift);

        var direction = new Vector2(horizontalMovement, verticalMovement);

        direction.Normalize();

        if (direction != Vector2.zero)
        {
            if (shiftButtonPressed && rollingCooldownTimer <= 0f)
            {
                rollCoroutine = StartCoroutine(RollCoroutine(direction));
            }
            else
            {
                player.movementByVelocityEvent.CallMovementByVelocity(direction, moveSpeed);
            }
        }
        else
        {
            player.idleEvent.CallIdleEvent();
        }
    }

    private IEnumerator RollCoroutine(Vector2 direction)
    {
        float minimumDistance = 0.2f;

        isRolling = true;

        var targetPosition = transform.position + (Vector3)direction * movementDetails.rollDistance;

        while (Vector3.Distance(transform.position, targetPosition) > minimumDistance)
        {
            player.movementToPositionEvent.CallMovementToPositionEvent(transform.position, targetPosition, movementDetails.rollSpeed, direction, isRolling);
            yield return waitForFixedUpdate;
        }

        isRolling = false;
        rollingCooldownTimer = movementDetails.rollCooldown;
        player.transform.position = targetPosition;
    }

    private void RollCooldownTimer()
    {
        if (rollingCooldownTimer >= 0f)
        {
            rollingCooldownTimer -= Time.deltaTime;
        }
    }

    private void StopRollingCoroutine()
    {
        if (rollCoroutine != null)
        {
            StopCoroutine(rollCoroutine);
            isRolling = false;
        }
    }

    private void SetPlayerAnimationSpeed()
    {
        player.animator.speed = moveSpeed / Animations.playerAnimationBaseSpeed;
    }

    private void SetStartingWeapon()
    {
        for (int i = 0; i < player.weaponList.Count; i++)
        {
            if (player.weaponList[i].weaponDetails == player.playerDetails.startingWeapon)
            {
                currentWeaponIndex = i;
                player.setActiveWeaponEvent.CallSetActiveWeaponEvent(player.weaponList[i]);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class PlayerControl : MonoBehaviour
{
    [SerializeField] private MovementDetailsSO movementDetails;

    [HideInInspector] public bool isRolling = false;
    [HideInInspector] public bool isMovementEnabled = true;

    private Player player;
    private float moveSpeed;

    private Coroutine rollCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    private float rollingCooldownTimer = 0f;

    private int currentWeaponIndex = 0;

    private bool fireLastFrame = false;

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
        if (!isMovementEnabled)
        {
            return;
        }

        if (isRolling)
        {
            return;
        }

        MovementInput();

        WeaponInput();

        UseItemInput();

        RollCooldownTimer();
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
            player.fireWeaponEvent.CallFireWeaponEvent(true, fireLastFrame, playerAimDirection, playerAngle, weaponAngle, weaponDirection);
        }
        fireLastFrame = fireWeapon;
        #endregion

        #region SWITCH WEAPON
        if (Input.mouseScrollDelta.y > 0f)
        {
            currentWeaponIndex--;
            if (currentWeaponIndex < 0)
            {
                currentWeaponIndex = player.weaponList.Count - 1;
            }
            SetWeaponByIndex(currentWeaponIndex);
        }
        else if (Input.mouseScrollDelta.y < 0f)
        {
            currentWeaponIndex++;
            if (currentWeaponIndex >= player.weaponList.Count)
            {
                currentWeaponIndex = 0;
            }
            SetWeaponByIndex(currentWeaponIndex);
        }

        if (Input.GetKey(KeyCode.Alpha1))
        {
            SetWeaponByIndex(0);
        }

        if (Input.GetKey(KeyCode.Alpha2))
        {
            SetWeaponByIndex(1);
        }

        if (Input.GetKey(KeyCode.Alpha3))
        {
            SetWeaponByIndex(2);
        }

        if (Input.GetKey(KeyCode.Alpha4))
        {
            SetWeaponByIndex(3);
        }

        if (Input.GetKey(KeyCode.Alpha5))
        {
            SetWeaponByIndex(4);
        }

        if (Input.GetKey(KeyCode.Alpha6))
        {
            SetWeaponByIndex(5);
        }

        if (Input.GetKey(KeyCode.Alpha7))
        {
            SetWeaponByIndex(6);
        }

        if (Input.GetKey(KeyCode.Alpha8))
        {
            SetWeaponByIndex(7);
        }

        if (Input.GetKey(KeyCode.Alpha9))
        {
            SetWeaponByIndex(8);
        }

        #endregion

        #region RELOAD WEAPON
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadWeapon();
        }
        #endregion
    }

    private void UseItemInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            var radius = 2f;
            foreach (var collider in Physics2D.OverlapCircleAll(player.transform.position, radius))
            {
                var useable = collider.GetComponent<IUseable>();
                if (useable != null)
                {
                    useable.useItem();
                }
            }
        }
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
        int index = player.weaponList.FindIndex((weapon) => { return weapon.weaponDetails == player.playerDetails.startingWeapon; });
        SetWeaponByIndex(index);
    }

    private void SetWeaponByIndex(int index)
    {
        if (index >= player.weaponList.Count)
        {
            return;
        }

        currentWeaponIndex = index;
        player.setActiveWeaponEvent.CallSetActiveWeaponEvent(player.weaponList[index]);
    }

    private void ReloadWeapon()
    {
        var currentWeapon = player.activeWeapon.CurrentWeapon;

        if (currentWeapon.isReloading)
        {
            return;
        }

        if (currentWeapon.totalAmmo < currentWeapon.weaponDetails.ammoClipCapacity && !currentWeapon.weaponDetails.hasInfiniteAmmo)
        {
            return;
        }

        if (currentWeapon.clipAmmo == currentWeapon.weaponDetails.ammoClipCapacity)
        {
            return;
        }

        player.reloadWeaponEvent.CallReloadWeaponEvent(currentWeapon);
    }
}

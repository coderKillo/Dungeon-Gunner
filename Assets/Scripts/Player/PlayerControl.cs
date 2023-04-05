using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class PlayerControl : MonoBehaviour
{
    [SerializeField] private MovementDetailsSO movementDetails;
    public MovementDetailsSO MovementDetails { get { return movementDetails; } }

    [ReadOnly] public bool isEnabled = true;

    private Player player;
    private float moveSpeed;
    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }

    private int currentWeaponIndex = 0;

    private bool fireLastFrame = false;

    private void Awake()
    {
        player = GetComponent<Player>();
        moveSpeed = movementDetails.GetRandomMovementSpeed();
    }

    private void Start()
    {
        SetPlayerAnimationSpeed();

        SetStartingWeapon();
    }

    private void Update()
    {
        if (!isEnabled)
        {
            player.idleEvent.CallIdleEvent();
            return;
        }

        if (player.playerDash.IsDashing())
        {
            return;
        }

        MovementInput();

        WeaponInput();

        UseItemInput();

    }

    private void OnCollisionStay2D(Collision2D other)
    {
        player.playerDash.StopDash();
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
        if (fireWeapon || fireLastFrame)
        {
            player.fireWeaponEvent.CallFireWeaponEvent(fireWeapon, fireLastFrame, playerAimDirection, playerAngle, weaponAngle, weaponDirection);
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
            if (shiftButtonPressed && player.playerDash.CanDash())
            {
                player.playerDash.Dash(direction);
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

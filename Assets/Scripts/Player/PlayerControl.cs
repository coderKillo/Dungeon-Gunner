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

    private bool fireLastFrame = false;

    private void Awake()
    {
        player = GetComponent<Player>();
        moveSpeed = movementDetails.GetRandomMovementSpeed();
    }

    private void Start()
    {
        SetPlayerAnimationSpeed();
    }

    private void Update()
    {
        CardInput();

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

    private void CardInput()
    {
        if (Input.mouseScrollDelta.y > 0f)
        {
            player.playerCardHand.NextCard();
        }
        else if (Input.mouseScrollDelta.y < 0f)
        {
            player.playerCardHand.PreviousCard();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            player.playerCardHand.SacrificeCurrentCard();
        }
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

    private void SetWeaponByIndex(int index)
    {
        if (index >= player.weaponList.Count)
        {
            return;
        }

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

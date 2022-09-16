using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerControl : MonoBehaviour
{
    [SerializeField] private Transform weaponShootPosition;
    [SerializeField] private MovementDetailsSO movementDetails;

    private Player player;
    private float moveSpeed;

    private void Awake()
    {
        player = GetComponent<Player>();
        moveSpeed = movementDetails.GetRandomMovementSpeed();
    }

    private void Update()
    {
        MovementInput();

        WeaponInput();
    }

    private void WeaponInput()
    {
        var mouseWorldPosition = HelperUtilities.GetWorldMousePosition();

        var weaponDirection = (mouseWorldPosition - weaponShootPosition.position);
        var playerDirection = (mouseWorldPosition - transform.position);

        var weaponAngle = HelperUtilities.GetAngleFromVector(weaponDirection);
        var playerAngle = HelperUtilities.GetAngleFromVector(playerDirection);

        var playerAimDirection = HelperUtilities.GetAimDirection(playerAngle);

        player.aimWeaponEvent.CallWeaponAimEvent(playerAimDirection, playerAngle, weaponAngle, weaponDirection);
    }

    private void MovementInput()
    {
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");

        var direction = new Vector2(horizontalMovement, verticalMovement);

        direction.Normalize();

        if (direction != Vector2.zero)
        {
            player.movementByVelocityEvent.CallMovementByVelocity(direction, moveSpeed);
        }
        else
        {
            player.idleEvent.CallIdleEvent();
        }
    }
}

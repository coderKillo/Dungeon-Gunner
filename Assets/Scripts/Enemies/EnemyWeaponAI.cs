using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyWeaponAI : MonoBehaviour
{
    [SerializeField] private LayerMask lineOfSightLayerMask;
    [SerializeField] private Transform weaponShootPosition;

    private Enemy enemy;
    private EnemyDetailsSO enemyDetails;
    private float firingIntervalTimer;
    private float firingDurationTimer;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        enemyDetails = enemy.enemyDetails;
        firingIntervalTimer = enemyDetails.RandomFiringInterval;
        firingDurationTimer = enemyDetails.RandomFiringDuration;
    }

    private void Update()
    {
        firingIntervalTimer -= Time.deltaTime;

        if (firingIntervalTimer < 0)
        {
            if (firingDurationTimer >= 0)
            {
                FireWeapon();
                firingDurationTimer -= Time.deltaTime;
            }
            else
            {
                firingIntervalTimer = enemyDetails.RandomFiringInterval;
                firingDurationTimer = enemyDetails.RandomFiringDuration;
            }
        }
    }

    private void FireWeapon()
    {
        var playerDirectionVector = GameManager.Instance.PlayerPosition - transform.position;
        var weaponDirectionVector = GameManager.Instance.PlayerPosition - weaponShootPosition.position;

        var playerAngle = HelperUtilities.GetAngleFromVector(playerDirectionVector);
        var weaponAngle = HelperUtilities.GetAngleFromVector(weaponDirectionVector);

        var aimDirection = HelperUtilities.GetAimDirection(playerAngle);

        enemy.aimWeaponEvent.CallWeaponAimEvent(
            aimDirection, playerAngle, weaponAngle, weaponDirectionVector
        );

        if (HasWeapon() && PlayerInRange())
        {
            if (enemyDetails.firingLineOfSightRequire && !PlayerInLineOfSight(weaponDirectionVector))
            {
                return;
            }

            enemy.fireWeaponEvent.CallFireWeaponEvent(
                true, true, aimDirection, playerAngle, weaponAngle, weaponDirectionVector);

        }

    }

    private bool PlayerInRange()
    {
        var playerDirectionVector = GameManager.Instance.PlayerPosition - transform.position;
        return (playerDirectionVector.magnitude <= enemyDetails.weaponDetails.ammo.range);
    }

    private bool HasWeapon()
    {
        return enemyDetails.weaponDetails != null;
    }

    private bool PlayerInLineOfSight(Vector3 weaponDirectionVector)
    {
        var raycastHit = Physics2D.Raycast(weaponShootPosition.position, (Vector2)weaponDirectionVector, enemyDetails.weaponDetails.ammo.range, lineOfSightLayerMask);

        if (raycastHit && raycastHit.transform.CompareTag(Settings.playerTag))
        {
            return true;
        }

        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPattern : MonoBehaviour, IFireable
{
    [SerializeField] private Ammo[] ammoArray;

    private float chargingTimer = 0;
    private float speed = 0;
    private float rotationSpeed = 360f;
    private Vector3 fireDirection = new Vector3();

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void InitialAmmo(AmmoDetailsSO ammoDetails, float aimAngel, float weaponAngle, float speed, Vector3 weaponAimDirection, bool overrideAmmoMovement = false)
    {
        this.speed = speed;
        this.fireDirection = weaponAimDirection;

        chargingTimer = ammoDetails.chargeTime;
        rotationSpeed = ammoDetails.rotationSpeed;

        foreach (var ammo in ammoArray)
        {
            ammo.InitialAmmo(ammoDetails, aimAngel, weaponAngle, speed, weaponAimDirection, true);
        }

        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (chargingTimer > 0)
        {
            chargingTimer -= Time.deltaTime;
            return;
        }

        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

        transform.position += fireDirection.normalized * speed * Time.deltaTime;
    }

}

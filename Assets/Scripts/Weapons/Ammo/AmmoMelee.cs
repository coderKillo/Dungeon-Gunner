using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoMelee : MonoBehaviour, IFireable
{
    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void InitialAmmo(AmmoDetailsSO ammoDetails, float aimAngel, float weaponAngle, float speed, Vector3 weaponAimDirection, bool overrideAmmoMovement = false)
    {
        // Debug.DrawLine()
        // TODO: Continue here
    }
}

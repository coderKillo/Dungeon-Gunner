using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFireable
{
    void InitialAmmo(AmmoDetailsSO ammoDetails, float aimAngel, float weaponAngle, float speed, Vector3 weaponAimDirection, bool overrideAmmoMovement = false);

    GameObject GetGameObject();

}

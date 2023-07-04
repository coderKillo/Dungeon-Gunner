using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnHit
{
    GameObject GetGameObject();
    void Hit();
    void SetDamage(int damage);
    void SetRadius(float radius);
}

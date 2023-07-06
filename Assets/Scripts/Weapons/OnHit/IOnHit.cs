using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnHit
{
    GameObject GetGameObject();
    void Hit(Collider2D collider);
    void SetDamage(int damage);
    void SetRadius(float radius);
}

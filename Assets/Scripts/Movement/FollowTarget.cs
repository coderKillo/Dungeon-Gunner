using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    private Transform target;
    public Transform Target { set { target = value; } }

    void Update()
    {
        transform.position = target.position;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTarget : MonoBehaviour
{
    private CinemachineTargetGroup cinemachineTargetGroup;

    private void Awake()
    {
        cinemachineTargetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        SetCinemachineTargetGroup();
    }

    private void SetCinemachineTargetGroup()
    {
        var player_target = new CinemachineTargetGroup.Target { weight = 1f, radius = 1f, target = GameManager.Instance.Player.transform };

        cinemachineTargetGroup.m_Targets = new CinemachineTargetGroup.Target[] { player_target };
    }
}

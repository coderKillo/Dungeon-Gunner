using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTarget : MonoBehaviour
{
    private CinemachineTargetGroup cinemachineTargetGroup;

    [SerializeField] private Transform cursorTarget;

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
        var player_target = new CinemachineTargetGroup.Target { weight = 1f, radius = 2.5f, target = GameManager.Instance.Player.transform };
        var cursor_target = new CinemachineTargetGroup.Target { weight = 1f, radius = 1f, target = cursorTarget };

        cinemachineTargetGroup.m_Targets = new CinemachineTargetGroup.Target[] { player_target, cursor_target };
    }

    private void Update()
    {
        cursorTarget.position = HelperUtilities.GetWorldMousePosition();
    }
}

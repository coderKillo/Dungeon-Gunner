using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class DungeonMap : SingletonAbstract<DungeonMap>
{
    [SerializeField] private GameObject minimapUI;

    private Camera dungeonMapCamera;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        dungeonMapCamera = GetComponentInChildren<Camera>();
        dungeonMapCamera.gameObject.SetActive(false);

        var cinemachineCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        cinemachineCamera.Follow = GameManager.Instance.Player.transform;
    }

    public void DisplayDungeonMap()
    {
        dungeonMapCamera.gameObject.SetActive(true);

        mainCamera.gameObject.SetActive(false);
        minimapUI.SetActive(false);

        GameManager.Instance.Player.EnablePlayer(false);

        ActivateAllRooms();
    }

    private void ActivateAllRooms()
    {
        foreach (var room in DungeonBuilder.Instance.roomDictionary.Values)
        {
            room.instantiatedRoom.gameObject.SetActive(true);
        }
    }

    public void ClearDungeonMap()
    {
        dungeonMapCamera.gameObject.SetActive(false);

        mainCamera.gameObject.SetActive(true);
        minimapUI.SetActive(true);

        GameManager.Instance.Player.EnablePlayer(true);
    }
}

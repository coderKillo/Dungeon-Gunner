using System.Runtime.InteropServices;
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

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && GameManager.Instance.GameState == GameState.dungeonOverviewMap)
        {
            TeleportToRoomOnMousePosition();
        }
    }

    private void TeleportToRoomOnMousePosition()
    {
        var worldPosition = dungeonMapCamera.ScreenToWorldPoint(Input.mousePosition);
        var colliders = Physics2D.OverlapCircleAll((Vector2)worldPosition, 1f);
        foreach (var collider in colliders)
        {
            var instantiatedRoom = collider.GetComponent<InstantiatedRoom>();
            if (instantiatedRoom == null)
            {
                Debug.Log("instantiatedRoom == null");
                continue;
            }

            if (!instantiatedRoom.room.isClearedOfEnemies || !instantiatedRoom.room.isPreviouslyVisited)
            {
                Debug.Log(instantiatedRoom.room.isPreviouslyVisited);
                Debug.Log(instantiatedRoom.room.isClearedOfEnemies);
                continue;
            }

            StaticEventHandler.CallRoomChangedEvent(instantiatedRoom.room);

            ClearDungeonMap();

            GameManager.Instance.DisplayMessage.DisplayText("", "", 1f);
            GameManager.Instance.Player.transform.position = HelperUtilities.GetNearestSpawnPointFromRoom(worldPosition, instantiatedRoom.room);
            GameManager.Instance.SetGameState(GameState.playingLevel);
        }
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

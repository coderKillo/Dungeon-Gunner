using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleRoomBuilder : MonoBehaviour
{
    [SerializeField] private RoomTemplateSO _roomTemplate;

    private void Start()
    {
        var roomNode = new RoomNodeSO();
        roomNode.id = Guid.NewGuid().ToString();

        var room = DungeonBuilder.CreateRoomFromTemplate(_roomTemplate, roomNode);
        DungeonBuilder.InstantiatedRoom(room, transform);

        GameManager.Instance.SetCurrentRoom(room);
        GameManager.Instance.PlacePlayerInCurrentRoom();

        StaticEventHandler.CallRoomChangedEvent(room);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSystemTestController : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            var room = new Room();
            room.nodeType = new RoomNodeTypeSO();
            room.nodeType.isBossRoom = false;
            StaticEventHandler.CallRoomEnemiesDefeated(room);
        }
    }
}

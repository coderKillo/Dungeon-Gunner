using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ActivateRooms : MonoBehaviour
{
    [SerializeField] private Camera minimapCamera;

    private void Start()
    {
        InvokeRepeating(nameof(EnableRooms), 0.5f, 0.5f);
    }

    private void EnableRooms()
    {
        if (GameManager.Instance.GameState == GameState.dungeonOverviewMap)
        {
            return;
        }

        var cameraLowerBounds = minimapCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        var cameraUpperBounds = minimapCamera.ViewportToWorldPoint(new Vector3(1f, 1f, 1f));

        var cameraViewportLowerBounds = new Vector2Int((int)cameraLowerBounds.x, (int)cameraLowerBounds.y);
        var cameraViewportUpperBounds = new Vector2Int((int)cameraUpperBounds.x, (int)cameraUpperBounds.y);

        var cameraViewportRect = new RectInt(cameraViewportLowerBounds, cameraViewportUpperBounds - cameraViewportLowerBounds);

        foreach (var room in DungeonBuilder.Instance.roomDictionary.Values)
        {
            var roomRect = new RectInt(room.lowerBound, room.Size);

            room.instantiatedRoom.gameObject.SetActive(cameraViewportRect.Overlaps(roomRect));
        }
    }
}

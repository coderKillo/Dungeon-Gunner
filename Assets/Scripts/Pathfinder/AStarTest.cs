using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarTest : MonoBehaviour
{
    private InstantiatedRoom room;
    private Grid grid;
    private Tilemap frontTilemap;
    private Tilemap pathTilemap;
    private Vector3Int startPosition;
    private Vector3Int endPosition;
    private TileBase startTile;
    private TileBase endTile;

    private Vector3Int noValue = new Vector3Int(9999, 9999, 9999);
    private Stack<Vector3> path = new Stack<Vector3>();


    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void Start()
    {
        startTile = GameResources.Instance.preferredEnemyPath;
        endTile = GameResources.Instance.enemyUnwalkableCollisionTilesArray[0];
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs obj)
    {
        InitRoom(obj.room);
    }

    private void InitRoom(Room room)
    {
        this.path.Clear();
        this.room = room.instantiatedRoom;
        this.grid = room.instantiatedRoom.grid;
        this.frontTilemap = room.instantiatedRoom.frontTilemap;
        this.startPosition = noValue;
        this.endPosition = noValue;

        var frontTilemapClone = room.instantiatedRoom.transform.Find("Grid/Tilemap4_Front(Clone)");

        if (frontTilemapClone == null)
        {
            pathTilemap = Instantiate(frontTilemap, grid.transform);
            pathTilemap.GetComponent<TilemapRenderer>().material = GameResources.Instance.litMaterial;
            pathTilemap.GetComponent<TilemapRenderer>().sortingOrder = 2;
            pathTilemap.gameObject.tag = "Untagged";
        }
        else
        {
            pathTilemap = frontTilemapClone.GetComponent<Tilemap>();
            pathTilemap.ClearAllTiles();
        }
    }

    private void Update()
    {
        if (room == null || pathTilemap == null)
        {
            Debug.Log("room or pathTile null");
            return;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            ClearPath();

            if (startPosition != noValue)
            {
                pathTilemap.SetTile(startPosition, null);
            }

            startPosition = grid.WorldToCell(HelperUtilities.GetWorldMousePosition());

            if (!IsPositionWithinBound(startPosition))
            {
                startPosition = noValue;
                return;
            }

            pathTilemap.SetTile(startPosition, startTile);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            ClearPath();

            if (endPosition != noValue)
            {
                pathTilemap.SetTile(endPosition, null);
            }

            endPosition = grid.WorldToCell(HelperUtilities.GetWorldMousePosition());

            if (!IsPositionWithinBound(endPosition))
            {
                endPosition = noValue;
                return;
            }

            pathTilemap.SetTile(endPosition, endTile);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            ClearPath();

            DisplayPath();
        }
    }

    private void DisplayPath()
    {
        if (startPosition == noValue || endPosition == noValue)
        {
            return;
        }

        path = AStar.BuildPath(room.room, startPosition, endPosition);

        if (path == null)
        {
            return;
        }

        foreach (var pos in path)
        {
            pathTilemap.SetTile(grid.WorldToCell(pos), startTile);
        }
    }

    private bool IsPositionWithinBound(Vector3Int position)
    {
        if (position.x < room.room.templateLowerBound.x)
        {
            return false;
        }

        if (position.x >= room.room.templateUpperBound.x)
        {
            return false;
        }

        if (position.y < room.room.templateLowerBound.y)
        {
            return false;
        }

        if (position.y >= room.room.templateUpperBound.y)
        {
            return false;
        }

        return true;
    }

    private void ClearPath()
    {
        if (path == null)
        {
            return;
        }

        foreach (var pos in path)
        {
            pathTilemap.SetTile(grid.WorldToCell(pos), null);
        }

        path.Clear();
    }
}

using System.Diagnostics;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider2D))]
public class InstantiatedRoom : MonoBehaviour
{
    [HideInInspector] public Room room;
    [HideInInspector] public Grid grid;
    [HideInInspector] public Tilemap groundTilemap;
    [HideInInspector] public Tilemap decorator1Tilemap;
    [HideInInspector] public Tilemap decorator2Tilemap;
    [HideInInspector] public Tilemap frontTilemap;
    [HideInInspector] public Tilemap collisionTilemap;
    [HideInInspector] public Tilemap minimapTilemap;
    [HideInInspector] public Bounds roomColliderBounds;
    [HideInInspector] public int[,] pathfinderMovementPenaltyMatrix;

    private BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        roomColliderBounds = boxCollider.bounds;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == Settings.playerTag && room != GameManager.Instance.CurrentRoom)
        {
            room.isPreviouslyVisited = true;
            GameManager.Instance.SetCurrentRoom(room);
            StaticEventHandler.CallRoomChangedEvent(room);
        }
    }

    public void Initialize(GameObject roomGameObject)
    {
        grid = roomGameObject.GetComponentInChildren<Grid>();

        var tilemaps = roomGameObject.GetComponentsInChildren<Tilemap>();

        PopulateTilemaps(tilemaps);

        BlockOffUnusedDoorways(tilemaps);

        AddObstacles();

        AddDoorsToRoom();

        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }

    private void PopulateTilemaps(Tilemap[] tilemaps)
    {
        foreach (var tilemap in tilemaps)
        {
            if (tilemap.gameObject.tag == "groundTilemap")
            {
                groundTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "decoration1Tilemap")
            {
                decorator1Tilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "decoration2Tilemap")
            {
                decorator2Tilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "frontTilemap")
            {
                frontTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "collisionTilemap")
            {
                collisionTilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "minimapTilemap")
            {
                minimapTilemap = tilemap;
            }
        }
    }


    private void BlockOffUnusedDoorways(Tilemap[] tilemaps)
    {
        foreach (var doorway in room.doorwayList)
        {
            if (doorway.isConnected)
            {
                continue;
            }

            foreach (var tilemap in tilemaps)
            {
                if (tilemap == null)
                {
                    continue;
                }

                BlockDoorwayOnTilemap(doorway, tilemap);
            }
        }
    }

    private void AddObstacles()
    {
        pathfinderMovementPenaltyMatrix = new int[room.Size.x, room.Size.y];

        for (int x = 0; x < room.Size.x; x++)
        {
            for (int y = 0; y < room.Size.y; y++)
            {
                pathfinderMovementPenaltyMatrix[x, y] = Settings.defaultAstarMovementPenalty;

                var worldPosition = new Vector3Int(x + room.templateLowerBound.x, y + room.templateLowerBound.y);
                var currentTile = collisionTilemap.GetTile(worldPosition);

                if (GameResources.Instance.enemyUnwalkableCollisionTilesArray.Contains(currentTile))
                {
                    pathfinderMovementPenaltyMatrix[x, y] = 0;
                }

                if (GameResources.Instance.preferredEnemyPath == currentTile)
                {
                    pathfinderMovementPenaltyMatrix[x, y] = Settings.preferredPathMovementPenalty;
                }
            }
        }

    }

    private void BlockDoorwayOnTilemap(Doorway doorway, Tilemap tilemap)
    {
        Vector2Int startPosition = doorway.doorwayStartCopyPosition;
        Vector3Int direction = Vector3Int.zero;

        switch (doorway.orientation)
        {
            case Orientation.north:
            case Orientation.south:
                direction = Vector3Int.right;
                break;

            case Orientation.west:
            case Orientation.east:
                direction = Vector3Int.down;
                break;

            default:
                return;
        }

        for (int xPos = 0; xPos < doorway.doorwayCopyTileWidth; xPos++)
        {
            for (int yPos = 0; yPos < doorway.doorwayCopyTileHeight; yPos++)
            {
                var sourceTile = new Vector3Int(startPosition.x + xPos, startPosition.y - yPos);
                var targetTile = sourceTile + direction;

                var transformMatrix = tilemap.GetTransformMatrix(sourceTile);

                tilemap.SetTile(targetTile, tilemap.GetTile(sourceTile));

                tilemap.SetTransformMatrix(targetTile, transformMatrix);
            }
        }
    }

    private void AddDoorsToRoom()
    {
        if (room.nodeType.isCorridorEW || room.nodeType.isCorridorNS)
        {
            return;
        }

        foreach (var doorway in room.doorwayList)
        {
            if (!doorway.isConnected || doorway.doorPrefab == null)
            {
                continue;
            }

            var tileDistance = Settings.tileSizePixels / Settings.pixelPerUnit;

            var door = GameObject.Instantiate(doorway.doorPrefab, gameObject.transform);
            door.transform.localPosition = new Vector3(doorway.position.x, doorway.position.y, 0f);

            switch (doorway.orientation)
            {
                case Orientation.north:
                    door.transform.localPosition += new Vector3(tileDistance / 2f, tileDistance, 0f);
                    break;

                case Orientation.south:
                    door.transform.localPosition += new Vector3(tileDistance / 2f, 0f, 0f);
                    break;

                case Orientation.west:
                    door.transform.localPosition += new Vector3(tileDistance, tileDistance * 1.25f, 0f);
                    break;

                case Orientation.east:
                    door.transform.localPosition += new Vector3(0f, tileDistance * 1.25f, 0f);
                    break;
            }

            if (room.nodeType.isBossRoom)
            {
                var doorComponent = door.GetComponent<Door>();

                doorComponent.isBossRoom = true;
                doorComponent.Lock();
            }
        }
    }

    public void LockDoors()
    {
        foreach (var door in GetComponentsInChildren<Door>())
        {
            door.Lock();
        }

        DisableRoomCollider();
    }

    public void DisableRoomCollider()
    {
        boxCollider.enabled = false;
    }
}

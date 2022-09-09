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

    private BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        roomColliderBounds = boxCollider.bounds;
    }

    public void Initialize(GameObject roomGameObject)
    {
        grid = roomGameObject.GetComponentInChildren<Grid>();

        var tilemaps = roomGameObject.GetComponentsInChildren<Tilemap>();

        PopulateTilemaps(tilemaps);

        BlockOffUnusedDoorways(tilemaps);

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
            else if (tilemap.gameObject.tag == "decorator1Tilemap")
            {
                decorator1Tilemap = tilemap;
            }
            else if (tilemap.gameObject.tag == "decorator2Tilemap")
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
}
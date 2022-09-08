using System.Collections;
using System.Collections.Generic;
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

        foreach (var tilemap in roomGameObject.GetComponentsInChildren<Tilemap>())
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

        collisionTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
    }
}

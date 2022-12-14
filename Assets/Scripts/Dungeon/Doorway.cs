using UnityEngine;

[System.Serializable]
public class Doorway
{
    public Vector2Int position;
    public Orientation orientation;
    public GameObject doorPrefab;

    [Header("INFORMATION FOR OVERRIDE UNUSED DOORWAYS")]
    [Tooltip("The Upper Left Position To Start Copying From")]
    public Vector2Int doorwayStartCopyPosition;
    [Tooltip("The width of tiles in the doorway to copy over")]
    public int doorwayCopyTileWidth;
    [Tooltip("The height of tiles in the doorway to copy over")]
    public int doorwayCopyTileHeight;

    public bool isConnected = false;
    public bool isUnavailable = false;
}

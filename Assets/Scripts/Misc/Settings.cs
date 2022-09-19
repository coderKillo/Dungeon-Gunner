using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class Settings
{
    #region UNITS
    public const float pixelPerUnit = 16f;
    public const float tileSizePixels = 16f;
    #endregion

    #region TAGS
    public const string playerTag = "Player";
    public const string playerWeaponTag = "PlayerWeapon";
    #endregion

    #region ROOM SETTINGS
    public const int maxChildCorridors = 3;
    #endregion

    #region DUNGEON BUILD SETTINGS
    public const int maxDungeonRebuildAttemptsForRoomGraph = 1000;
    public const int maxDungeonBuildAttempts = 10;
    #endregion
}

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
    public const float fateInTime = 0.5f;
    public const int maxChildCorridors = 3;
    public const float doorUnlockDelay = 1f;
    #endregion

    #region DUNGEON BUILD SETTINGS
    public const int maxDungeonRebuildAttemptsForRoomGraph = 1000;
    public const int maxDungeonBuildAttempts = 10;
    #endregion

    #region FIRING CONTROL 
    public const float useAimAngleDistance = 3.5f;
    #endregion

    #region UI PARAMETERS
    public const float uiAmmoIconSpacing = 4f;
    #endregion

    #region ASTAR PATHFINDER PARAMETERS
    public const int defaultAstarMovementPenalty = 40;
    public const int preferredPathMovementPenalty = 1;
    public const float playerMoveDistanceToRebuildPath = 3f;
    public const float enemyPathRebuildCooldown = 2f;
    public const int targetFrameRateToSpreadRebuildPath = 60;
    #endregion

    #region HEALTH PARAMETERS
    public const int defaultEnemyHealth = 20;
    #endregion

    #region CONTACT DAMAGE PARAMETERS
    public const float contactDamageCollisionResetDelay = 0.5f;
    #endregion
}

using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLevel_", menuName = "Scriptable Object/Dungeon/Dungeon Level")]
public class DungeonLevelSO : ScriptableObject
{
    [Space(10)]
    [Header("BASIC LEVEL DETAILS")]
    public string levelName;

    [Space(10)]
    [Header("ROOM TEMPLATES FOR LEVEL")]
    public List<RoomTemplateSO> roomTemplateList;

    [Space(10)]
    [Header("ROOM GRAPHS FOR LEVEL")]
    public List<RoomNodeGraphSO> roomNodeGraphList;

    [Space(10)]
    [Header("ENEMY PARAMETER")]
    public EnemyPoolSO enemyPool;
    public EnemyPoolSO bossPool;
    public int bossCount = 1;

#if UNITY_EDITOR

    #region VALIDATION
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(levelName), levelName);

        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomTemplateList), roomTemplateList))
        {
            return;
        }
        if (HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomNodeGraphList), roomNodeGraphList))
        {
            return;
        }

        bool hasEWCorridor = false;
        bool hasNSCorridor = false;
        bool hasEntrance = false;

        foreach (var template in roomTemplateList)
        {
            if (template == null)
            {
                return;
            }

            if (template.roomNodeType.isCorridorNS)
            {
                hasNSCorridor = true;
            }

            if (template.roomNodeType.isCorridorEW)
            {
                hasEWCorridor = true;
            }

            if (template.roomNodeType.isEntrance)
            {
                hasEntrance = true;
            }
        }

        if (!hasNSCorridor)
        {
            Debug.Log("In " + this.name.ToString() + " No N/S Corridor Room Type Specified");
        }

        if (!hasEWCorridor)
        {
            Debug.Log("In " + this.name.ToString() + " No E/W Corridor Room Type Specified");
        }

        if (!hasEntrance)
        {
            Debug.Log("In " + this.name.ToString() + " No Entrance Room Type Specified");
        }

        foreach (var graph in roomNodeGraphList)
        {
            if (graph == null)
            {
                return;
            }

            foreach (var node in graph.nodeList)
            {
                if (node == null)
                {
                    continue;
                }

                if (
                    node.type.isEntrance ||
                    node.type.isCorridor ||
                    node.type.isCorridorEW ||
                    node.type.isCorridorNS ||
                    node.type.isNone
                )
                {
                    continue;
                }

                bool isRoomNodeTypeFound = false;

                foreach (var template in roomTemplateList)
                {
                    if (template == null)
                    {
                        continue;
                    }

                    if (template.roomNodeType == node.type)
                    {
                        isRoomNodeTypeFound = true;
                    }
                }

                if (!isRoomNodeTypeFound)
                {
                    Debug.Log("In " + this.name.ToString() + " No room template " + node.type.ToString());
                }
            }
        }
    }
    #endregion

#endif
}

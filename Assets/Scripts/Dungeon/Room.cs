using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public string id;
    public string templateId;

    public GameObject prefab;

    public RoomNodeTypeSO nodeType;

    public Vector2Int lowerBound;
    public Vector2Int upperBound;
    public Vector2Int templateLowerBound;
    public Vector2Int templateUpperBound;

    public Vector2Int[] spawnPositions;
    public List<SpawnableObjectsByLevel<EnemyDetailsSO>> enemiesByLevelList;
    public List<RoomEnemySpawnParameters> enemySpawnParameterList;

    public string parentId;
    public List<string> childIdList;

    public List<Doorway> doorwayList;

    public InstantiatedRoom instantiatedRoom;

    public bool isLit = false;
    public bool isPositioned = false;
    public bool isClearedOfEnemies = false;
    public bool isPreviouslyVisited = false;

    public Vector2Int Size { get { return templateUpperBound - templateLowerBound; } }

    public Vector2Int Position
    {
        get { return lowerBound - templateLowerBound; }
        set
        {
            lowerBound = value + templateLowerBound;
            upperBound = lowerBound + Size;
        }
    }

    public Room()
    {
        childIdList = new List<string>();
        doorwayList = new List<Doorway>();
    }

    public int GetNumbersOfEnemiesToSpawn(DungeonLevelSO level)
    {
        foreach (var enemySpawnParameter in enemySpawnParameterList)
        {
            if (enemySpawnParameter.dungeonLevel == level)
            {
                return enemySpawnParameter.TotalEnemies;
            }
        }

        return 0;
    }

    public RoomEnemySpawnParameters GetEnemySpawnParameter(DungeonLevelSO level)
    {
        foreach (var enemySpawnParameter in enemySpawnParameterList)
        {
            if (enemySpawnParameter.dungeonLevel == level)
            {
                return enemySpawnParameter;
            }
        }

        return null;
    }

    public Vector2Int RandomSpawnPosition()
    {
        return spawnPositions[Random.Range(0, spawnPositions.Length)];
    }
}

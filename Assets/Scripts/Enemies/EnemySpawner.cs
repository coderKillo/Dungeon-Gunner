using System;
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemySpawner : SingletonAbstract<EnemySpawner>
{
    private int totalEnemies;
    private int concurrentEnemies;
    private int currentEnemies;
    private Room room;
    private RoomEnemySpawnParameters spawnParameters;

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs obj)
    {
        totalEnemies = 0;
        currentEnemies = 0;
        room = obj.room;

        if (room.nodeType.isCorridorEW || room.nodeType.isCorridorNS || room.nodeType.isEntrance)
        {
            return;
        }

        if (room.isClearedOfEnemies)
        {
            return;
        }

        spawnParameters = room.GetEnemySpawnParameter(GameManager.Instance.CurrentLevel);
        totalEnemies = spawnParameters.TotalEnemies;
        concurrentEnemies = spawnParameters.ConcurrentEnemies;

        if (totalEnemies == 0)
        {
            room.isClearedOfEnemies = true;
            return;
        }

        SpawnEnemies();

        var roomTemplate = DungeonBuilder.Instance.GetRoomTemplate(obj.room.templateId);
        if (roomTemplate == null)
        {
            return;
        }
    }

    private void SpawnEnemies()
    {
        if (GameManager.Instance.GameState == GameState.playingLevel)
        {
            GameManager.Instance.SetGameState(GameState.engagingEnemies);
        }

        if (room.spawnPositions.Length > 0)
        {
            StartCoroutine(SpawnEnemiesRoutine());
        }
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        var grid = room.instantiatedRoom.grid;

        var randomSpawnableObject = new RandomSpawnableObject<EnemyDetailsSO>(room.enemiesByLevelList);

        for (int spawnNumber = 0; spawnNumber < totalEnemies; spawnNumber++)
        {
            while (currentEnemies >= concurrentEnemies)
            {
                yield return null;
            }

            var cellPosition = (Vector3Int)room.RandomSpawnPosition();

            CreateEnemy(randomSpawnableObject.GetItem(), grid.CellToWorld(cellPosition), spawnNumber);

            yield return new WaitForSeconds(spawnParameters.SpawnInterval);
        }
    }

    private void CreateEnemy(EnemyDetailsSO enemyDetails, Vector3 position, int spawnNumber)
    {
        currentEnemies++;

        var enemy = GameObject.Instantiate(enemyDetails.prefab, position, Quaternion.identity);
        enemy.GetComponent<Enemy>().Initialize(enemyDetails, spawnNumber, GameManager.Instance.CurrentLevel);
    }
}

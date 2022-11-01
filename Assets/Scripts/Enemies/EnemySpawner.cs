using System;
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemySpawner : SingletonAbstract<EnemySpawner>
{
    private int totalEnemies;
    private int spawnedEnemies;
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
        spawnedEnemies = 0;
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

        room.instantiatedRoom.LockDoors();

        StaticEventHandler.CallRoomEnemiesEngaging(room);

        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
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
        spawnedEnemies++;

        var enemyGameObject = GameObject.Instantiate(enemyDetails.prefab, position, Quaternion.identity);
        enemyGameObject.GetComponent<DestroyedEvent>().OnDestroyed += DestroyedEvent_OnDestroyed;

        var enemy = enemyGameObject.GetComponent<Enemy>();
        enemy.Initialize(enemyDetails, spawnNumber, GameManager.Instance.CurrentLevel);

        StaticEventHandler.CallEnemySpawned(enemy);
    }

    private void DestroyedEvent_OnDestroyed(DestroyedEvent obj, DestroyedEventArgs args)
    {
        obj.OnDestroyed -= DestroyedEvent_OnDestroyed;

        currentEnemies--;

        StaticEventHandler.CallPointScoredEvent(args.points);

        if (currentEnemies <= 0 && spawnedEnemies >= totalEnemies)
        {
            room.isClearedOfEnemies = true;
        }

        if (room.isClearedOfEnemies)
        {
            StaticEventHandler.CallRoomEnemiesDefeated(room);

            room.instantiatedRoom.UnlockDoors(Settings.doorUnlockDelay);
        }
    }
}

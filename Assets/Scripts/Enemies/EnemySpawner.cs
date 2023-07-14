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

    private float enemyHealthDifficultlyFactor = 1f;
    private float enemyDamageDifficultlyFactor = 1f;
    private float enemyWaveSizeDifficultlyFactor = 1f;

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
        StaticEventHandler.OnDifficultyChange += StaticEventHandler_OnDifficultyChange;
        GameManager.OnGameStateChange += GameManager_OnGameStateChange;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
        StaticEventHandler.OnDifficultyChange -= StaticEventHandler_OnDifficultyChange;
        GameManager.OnGameStateChange -= GameManager_OnGameStateChange;
    }

    private void StaticEventHandler_OnDifficultyChange(DifficultyChangedEventArgs args)
    {
        enemyDamageDifficultlyFactor = args.difficulty;
        enemyHealthDifficultlyFactor = args.difficulty;
        enemyWaveSizeDifficultlyFactor = args.difficulty;
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
        totalEnemies = Mathf.RoundToInt(spawnParameters.TotalEnemies * enemyWaveSizeDifficultlyFactor);
        concurrentEnemies = Mathf.RoundToInt(spawnParameters.ConcurrentEnemies * enemyWaveSizeDifficultlyFactor);

        if (totalEnemies == 0)
        {
            room.isClearedOfEnemies = true;

            StaticEventHandler.CallRoomEnemiesDefeated(room);

            return;
        }

        room.instantiatedRoom.LockDoors();

        StaticEventHandler.CallRoomEnemiesEngaging(room);

        SpawnEnemies();
    }

    private void GameManager_OnGameStateChange(GameState state)
    {
        if (state == GameState.levelCompleted)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
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

        for (int spawnNumber = 0; spawnNumber < totalEnemies; spawnNumber += concurrentEnemies)
        {
            int waveSize = Math.Min((totalEnemies - spawnNumber), concurrentEnemies);
            for (int i = 0; i < waveSize; i++)
            {
                var cellPosition = (Vector3Int)room.RandomSpawnPosition();

                CreateEnemy(randomSpawnableObject.GetItem(), grid.CellToWorld(cellPosition) + UnityEngine.Random.insideUnitSphere, spawnNumber);
            }

            while (currentEnemies > 0)
            {
                yield return new WaitForSeconds(spawnParameters.SpawnInterval);
            }
        }
    }

    private void CreateEnemy(EnemyDetailsSO enemyDetails, Vector3 position, int spawnNumber)
    {
        currentEnemies++;
        spawnedEnemies++;

        var enemyGameObject = GameObject.Instantiate(enemyDetails.prefab, position, Quaternion.identity, transform);
        enemyGameObject.GetComponent<DestroyedEvent>().OnDestroyed += DestroyedEvent_OnDestroyed;

        var enemy = enemyGameObject.GetComponent<Enemy>();
        enemy.Initialize(enemyDetails, spawnNumber, GameManager.Instance.CurrentLevel);
        enemy.fireWeapon.WeaponDamageFactor = enemyDamageDifficultlyFactor;
        enemy.dealContactDamage.Damage = Mathf.RoundToInt(Settings.defaultContactDamage * enemyDamageDifficultlyFactor);
        enemy.health.StartingHealth = Mathf.RoundToInt(enemy.health.StartingHealth * enemyHealthDifficultlyFactor);

        StaticEventHandler.CallEnemySpawned(enemy);
    }

    private void DestroyedEvent_OnDestroyed(DestroyedEvent obj, DestroyedEventArgs args)
    {
        obj.OnDestroyed -= DestroyedEvent_OnDestroyed;

        currentEnemies--;

        StaticEventHandler.CallEnemyDied(obj.gameObject.GetComponent<Enemy>());
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

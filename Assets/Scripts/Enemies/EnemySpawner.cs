using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

[DisallowMultipleComponent]
public class EnemySpawner : SingletonAbstract<EnemySpawner>
{
    [Space(10)]
    [Header("Credits")]
    [SerializeField] private int baseCredits = 50;
    [SerializeField] private float creditCoefficientFactor = 0.25f;
    [ShowInInspector][ReadOnly] private int currentRoomCredits = 0;

    [Space(10)]
    [Header("Level")]
    [SerializeField] private float enemyLevelCoefficientFactor = 3f;
    [SerializeField] private float enemyLevelDamageFactor = 0.03f;
    [SerializeField] private float enemyLevelHealthFactor = 0.2f;
    [ShowInInspector][ReadOnly] private int enemyLevel = 1;

    [Space(10)]
    [Header("Spawn")]
    [SerializeField] private Vector2 spawnIntervalDuringWave = new Vector3(0.1f, 1f);
    private float RandomSpawnIntervalDuringWave { get { return Random.Range(spawnIntervalDuringWave.x, spawnIntervalDuringWave.y); } }
    [SerializeField] private Vector2 spawnIntervalBetweenWave = new Vector3(4.5f, 9f);
    private float RandomSpawnIntervalBetweenWave { get { return Random.Range(spawnIntervalBetweenWave.x, spawnIntervalBetweenWave.y); } }

    [Space(10)]
    [Header("Elite")]
    [SerializeField] private int eliteCostFactor = 6;
    [SerializeField] private float eliteDamageFactor = 2f;
    [SerializeField] private float eliteHealthFactor = 4f;
    [SerializeField] private float eliteScaling = 1.5f;

    [Space(10)]
    [Header("Difficulty")]
    [ShowInInspector][ReadOnly] private float difficulty = 1f;
    [ShowInInspector][ReadOnly] private int minutesPlayed = 0;
    [ShowInInspector][ReadOnly] private float coefficient = 1.1f;

    private int spawnedEnemies;
    private int currentEnemies;

    private Room room;

    private void Start()
    {
        OnMinutePassed();
        InvokeRepeating(nameof(OnMinutePassed), 60f, 60f);
    }

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
        difficulty = args.difficulty;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs obj)
    {
        room = obj.room;

        if (room.nodeType.isCorridorEW || room.nodeType.isCorridorNS || room.nodeType.isEntrance || room.nodeType.isChestRoom)
        {
            return;
        }

        if (room.isClearedOfEnemies)
        {
            return;
        }

        if (room.spawnPositions.Length <= 0)
        {
            Debug.LogWarning($"no spawn positions are set in room: {room.instantiatedRoom.name}");
            return;
        }

        StartSpawnEnemies();
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

    [Button]
    private void OnMinutePassed()
    {
        minutesPlayed++;
        CalculateCoefficient();
        CalculateLevel();
    }

    [Button]
    private void StartSpawnEnemies()
    {
        spawnedEnemies = 0;
        currentEnemies = 0;
        currentRoomCredits = Mathf.RoundToInt(coefficient * baseCredits);

        room.instantiatedRoom.LockDoors();

        StaticEventHandler.CallRoomEnemiesEngaging(room);

        var currentLevel = GameManager.Instance.CurrentLevel;
        if (room.nodeType.isBossRoom)
        {
            SpawnBosses(currentLevel.bossPool, currentLevel.bossCount);
        }
        else
        {
            SpawnEnemies(currentLevel.enemyPool);
        }
    }

    private void SpawnEnemies(EnemyPoolSO pool)
    {
        StartCoroutine(SpawnEnemiesRoutine(pool));
    }

    private void SpawnBosses(EnemyPoolSO pool, int count)
    {
        for (int i = 0; i < count; i++)
        {
            var enemyDetails = pool.enemyList[Random.Range(0, pool.enemyList.Count)];
            CreateEnemy(enemyDetails, i);
        }

        currentRoomCredits = 0;
    }

    private IEnumerator SpawnEnemiesRoutine(EnemyPoolSO pool)
    {
        var credits = Mathf.RoundToInt(currentRoomCredits * 0.3f);
        var time = 0f;

        while (true)
        {
            credits += Mathf.RoundToInt(((1 + creditCoefficientFactor * coefficient)) * time);
            time = 0f;

            if (currentEnemies < room.maxConcurrentEnemies)
            {
                var enemyDetails = FindEnemyWithinBudget(credits, pool);
                while (enemyDetails != null)
                {
                    var cost = enemyDetails.value;

                    var enemy = CreateEnemy(enemyDetails, spawnedEnemies);

                    if (credits > cost * eliteCostFactor)
                    {
                        enemy.transform.localScale = new Vector3(eliteScaling, eliteScaling, 1f);
                        enemy.fireWeapon.WeaponDamageFactor *= eliteDamageFactor;
                        enemy.health.StartingHealth = Mathf.RoundToInt(eliteHealthFactor * enemy.health.StartingHealth);

                        cost *= eliteCostFactor;
                    }

                    credits -= cost;
                    currentRoomCredits -= cost;

                    if (FindEnemyWithinBudget(currentRoomCredits, pool) == null)
                    {
                        goto endSpawn;
                    }

                    enemyDetails = FindEnemyWithinBudget(credits, pool);

                    var waitDuringWaves = RandomSpawnIntervalDuringWave;
                    time += waitDuringWaves;
                    yield return new WaitForSeconds(waitDuringWaves);
                }
            }

            var waitBetweenWaves = RandomSpawnIntervalBetweenWave;
            time += waitBetweenWaves;
            yield return new WaitForSeconds(waitBetweenWaves);
        }

    endSpawn:
        currentRoomCredits = 0;
    }

    private EnemyDetailsSO FindEnemyWithinBudget(int credits, EnemyPoolSO pool)
    {
        var affordableEnemies = pool.enemyList.FindAll((x) => x.value < credits);
        if (affordableEnemies.Count == 0)
        {
            return null;
        }

        return affordableEnemies[Random.Range(0, affordableEnemies.Count)];
    }

    private Enemy CreateEnemy(EnemyDetailsSO enemyDetails, int spawnNumber)
    {
        var grid = room.instantiatedRoom.grid;
        var cellPosition = (Vector3Int)room.RandomSpawnPosition();
        var worldPosition = grid.CellToWorld(cellPosition) + UnityEngine.Random.insideUnitSphere;

        currentEnemies++;
        spawnedEnemies++;

        var enemyGameObject = GameObject.Instantiate(enemyDetails.prefab, worldPosition, Quaternion.identity, transform);
        enemyGameObject.GetComponent<DestroyedEvent>().OnDestroyed += DestroyedEvent_OnDestroyed;

        var enemy = enemyGameObject.GetComponent<Enemy>();
        enemy.Initialize(enemyDetails, spawnNumber, GameManager.Instance.CurrentLevel);
        enemy.fireWeapon.WeaponDamageFactor = GetEnemyDamageFactor();
        enemy.health.StartingHealth = GetEnemyHealth(enemyDetails.baseHealth);

        StaticEventHandler.CallEnemySpawned(enemy);

        return enemy;
    }

    private void CalculateCoefficient()
    {
        coefficient = (1f + minutesPlayed * 0.05f * difficulty) * Mathf.Pow(1.15f, (float)GameManager.Instance.LevelCompleted);
    }

    private void CalculateLevel()
    {
        enemyLevel = Mathf.RoundToInt(1f + (coefficient - 1f) * enemyLevelCoefficientFactor);
    }

    private int GetEnemyHealth(int baseHealth)
    {
        return baseHealth + Mathf.RoundToInt(enemyLevel * (float)baseHealth * enemyLevelHealthFactor);
    }

    private float GetEnemyDamageFactor()
    {
        return 1f + (float)enemyLevel * enemyLevelDamageFactor;
    }

    private void DestroyedEvent_OnDestroyed(DestroyedEvent obj, DestroyedEventArgs args)
    {
        obj.OnDestroyed -= DestroyedEvent_OnDestroyed;

        currentEnemies--;

        StaticEventHandler.CallEnemyDied(obj.gameObject.GetComponent<Enemy>());
        StaticEventHandler.CallPointScoredEvent(Mathf.RoundToInt(args.points * Settings.enemyPointScaling));

        if (currentEnemies <= 0 && currentRoomCredits <= 0)
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

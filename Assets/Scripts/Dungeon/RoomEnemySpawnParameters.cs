using UnityEngine;

[System.Serializable]
public class RoomEnemySpawnParameters
{
    public DungeonLevelSO dungeonLevel;

    public int minTotalEnemies;
    public int maxTotalEnemies;
    public int TotalEnemies { get { return Random.Range(minTotalEnemies, maxTotalEnemies); } }

    public int minConcurrentEnemies;
    public int maxConcurrentEnemies;
    public int ConcurrentEnemies { get { return Random.Range(minConcurrentEnemies, maxConcurrentEnemies); } }

    public float minSpawnInterval;
    public float maxSpawnInterval;
    public float SpawnInterval { get { return Random.Range(minSpawnInterval, maxSpawnInterval); } }
}

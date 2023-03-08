using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyDetailsSO _enemyDetails;
    [SerializeField] private Transform[] _spawnLocation;
    [SerializeField] private DungeonLevelSO _level;

    void Start()
    {
        SpawnEnemy();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.J))
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        foreach (var location in _spawnLocation)
        {
            var enemyGameObject = GameObject.Instantiate(_enemyDetails.prefab, location.position, Quaternion.identity);
            enemyGameObject.GetComponent<DestroyedEvent>().OnDestroyed += DestroyedEvent_OnDestroyed;

            var enemy = enemyGameObject.GetComponent<Enemy>();
            enemy.Initialize(_enemyDetails, 1, _level);
        }
    }

    private void DestroyedEvent_OnDestroyed(DestroyedEvent arg1, DestroyedEventArgs arg2)
    {
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyDetailsSO _enemyDetails;
    [SerializeField] private Transform _spawnLocation;
    [SerializeField] private DungeonLevelSO _level;

    bool _enemyAlive = false;

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
        if (_enemyAlive)
        {
            return;
        }

        var enemyGameObject = GameObject.Instantiate(_enemyDetails.prefab, _spawnLocation.position, Quaternion.identity);
        enemyGameObject.GetComponent<DestroyedEvent>().OnDestroyed += DestroyedEvent_OnDestroyed;

        var enemy = enemyGameObject.GetComponent<Enemy>();
        enemy.Initialize(_enemyDetails, 1, _level);

        _enemyAlive = true;
    }

    private void DestroyedEvent_OnDestroyed(DestroyedEvent arg1, DestroyedEventArgs arg2)
    {
        _enemyAlive = false;
    }
}

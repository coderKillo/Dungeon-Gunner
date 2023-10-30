using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class BossHealthUI : MonoBehaviour
{
    [SerializeField] private GameObject bossHealthBarPrefab;
    [SerializeField] private float disableDelay = 1f;

    private void OnEnable()
    {
        StaticEventHandler.OnRoomEnemiesEngaging += StaticEventHandler_OnRoomEnemiesEngaging;
        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnEnemiesDefeated;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomEnemiesEngaging -= StaticEventHandler_OnRoomEnemiesEngaging;
        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnEnemiesDefeated;
        StaticEventHandler.OnEnemySpawned -= StaticEventHandler_OnEnemySpawned;
    }

    private void StaticEventHandler_OnRoomEnemiesEngaging(RoomEnemiesEngagingEventArgs obj)
    {
        if (!obj.room.nodeType.isBossRoom)
        {
            return;
        }

        StaticEventHandler.OnEnemySpawned += StaticEventHandler_OnEnemySpawned;
    }

    private void StaticEventHandler_OnEnemiesDefeated(RoomEnemiesDefeatedEventArgs obj)
    {
        if (!obj.room.nodeType.isBossRoom)
        {
            return;
        }

        StaticEventHandler.OnEnemySpawned -= StaticEventHandler_OnEnemySpawned;

        StartCoroutine(ClearHealthBars());
    }

    private void StaticEventHandler_OnEnemySpawned(EnemySpawnedEventArgs obj)
    {
        var bossHealthBar = GameObject.Instantiate(bossHealthBarPrefab, Vector3.zero, Quaternion.identity, transform);
        bossHealthBar.GetComponent<BossHealthBarUI>().Initialize(obj.enemy);
    }

    private IEnumerator ClearHealthBars()
    {
        yield return new WaitForSeconds(disableDelay);

        for (int index = 0; index < transform.childCount; index++)
        {
            Destroy(transform.GetChild(index).gameObject);
        }
    }
}

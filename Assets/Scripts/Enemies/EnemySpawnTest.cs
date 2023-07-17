using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnTest : MonoBehaviour
{
    private List<SpawnableObjectsByLevel<EnemyDetailsSO>> spawnableObjectsByLevelList;
    private RandomSpawnableObject<EnemyDetailsSO> randomSpawnableObject;
    private List<GameObject> enemyList = new List<GameObject>();


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
        foreach (var enemy in enemyList)
        {
            Destroy(enemy);
        }

        enemyList.Clear();

        var roomTemplate = DungeonBuilder.Instance.GetRoomTemplate(obj.room.templateId);
        if (roomTemplate == null)
        {
            return;
        }

        // spawnableObjectsByLevelList = roomTemplate.enemiesByLevelList;
        // randomSpawnableObject = new RandomSpawnableObject<EnemyDetailsSO>(spawnableObjectsByLevelList);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (randomSpawnableObject == null)
            {
                return;
            }

            var enemyDetail = randomSpawnableObject.GetItem();

            if (enemyDetail == null)
            {
                return;
            }

            enemyList.Add(GameObject.Instantiate(enemyDetail.prefab, HelperUtilities.GetNearestSpawnPoint(HelperUtilities.GetWorldMousePosition()), Quaternion.identity));
        }
    }
}

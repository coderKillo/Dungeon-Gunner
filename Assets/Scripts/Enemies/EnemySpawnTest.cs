using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnTest : MonoBehaviour
{
    [SerializeField] private RoomTemplateSO roomTemplate;

    private List<SpawnableObjectsByLevel<EnemyDetailsSO>> spawnableObjectsByLevelList;
    private RandomSpawnableObject<EnemyDetailsSO> randomSpawnableObject;
    private GameObject enemy;

    void Start()
    {
        spawnableObjectsByLevelList = roomTemplate.enemiesByLevelList;

        randomSpawnableObject = new RandomSpawnableObject<EnemyDetailsSO>(spawnableObjectsByLevelList);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }

            var enemyDetail = randomSpawnableObject.GetItem();
            if (enemyDetail != null)
            {
                enemy = GameObject.Instantiate(enemyDetail.prefab, HelperUtilities.GetNearestSpawnPoint(HelperUtilities.GetWorldMousePosition()), Quaternion.identity);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestSpawnerTest : MonoBehaviour
{
    [SerializeField] private RoomChestSpawnParameters parameters;

    private ChestSpawner spawner;

    private void Awake()
    {
        spawner = GetComponent<ChestSpawner>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            spawner.SpawnChest(parameters);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChestSpawner : MonoBehaviour
{
    private void OnEnable()
    {
        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;
    }

    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedEventArgs obj)
    {
        var room = obj.room;
        var grid = room.instantiatedRoom.grid;

        if (room.isAlreadyLooted)
        {
            return;
        }

        room.isAlreadyLooted = true;

        var parameters = room.GetChestSpawnParameters(GameManager.Instance.CurrentLevel);

        if (RandomSpawnChest(parameters.SpawnChance))
        {
            SpawnChest(parameters);
        }
    }

    private bool RandomSpawnChest(int spawnChance)
    {
        var roll = Random.Range(1, 101);

        return (roll < spawnChance);
    }

    public void SpawnChest(RoomChestSpawnParameters parameters)
    {
        var chestGameObject = GameObject.Instantiate(GameResources.Instance.chestPrefab, transform);
        chestGameObject.transform.position = HelperUtilities.GetNearestSpawnPoint(GameManager.Instance.PlayerPosition);

        var chest = chestGameObject.GetComponent<Chest>();

        GetItemsToSpawn(parameters.RandomItemAmount, out bool hasHealth, out bool hasAmmo, out bool hasWeapon);

        chest.Initialize(
            hasHealth ? parameters.RandomHealthAmount : 0,
            hasAmmo ? parameters.RandomAmmoAmount : 0,
            hasWeapon ? parameters.RandomWeapon : null
        );
    }

    private void GetItemsToSpawn(int itemAmount, out bool hasHealth, out bool hasAmmo, out bool hasWeapon)
    {
        hasHealth = false;
        hasAmmo = false;
        hasWeapon = false;

        var choice = Random.Range(1, 4);

        switch (itemAmount)
        {
            case 1:
                switch (choice)
                {
                    case 1:
                        hasHealth = true;
                        break;
                    case 2:
                        hasAmmo = true;
                        break;
                    case 3:
                        hasWeapon = true;
                        break;
                    default:
                        break;
                }
                break;
            case 2:
                switch (choice)
                {
                    case 1:
                        hasHealth = true;
                        hasAmmo = true;
                        break;
                    case 2:
                        hasHealth = true;
                        hasWeapon = true;
                        break;
                    case 3:
                        hasAmmo = true;
                        hasWeapon = true;
                        break;
                    default:
                        break;
                }
                break;

            case 3:
                hasHealth = true;
                hasAmmo = true;
                hasWeapon = true;
                break;

            default:
                break;
        }
    }
}

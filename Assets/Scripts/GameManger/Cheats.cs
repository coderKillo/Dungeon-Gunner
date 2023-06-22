using System.Runtime.InteropServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheats : MonoBehaviour
{
    [SerializeField] private KeyCode _shieldKey;
    [SerializeField] private KeyCode _healKey;
    [SerializeField] private KeyCode _teleportToBoss;
    [SerializeField] private KeyCode _instagibGunKey;
    [SerializeField] private KeyCode _scoreKey;
    [SerializeField] private KeyCode _ammoKey;

    const int SCORE_AMOUNT = 200;
    const int HEAL_AMOUND = 300;
    const int SHIELD_AMOUND = 300;

    void Start()
    {
        Debug.LogWarning("cheats are available");
    }

    void Update()
    {
        if (Input.GetKeyDown(_shieldKey))
        {
            FillShield();
        }
        if (Input.GetKeyDown(_healKey))
        {
            FillHealth();
        }
        if (Input.GetKeyDown(_teleportToBoss))
        {
            TeleportPlayerToBoss();
        }
        if (Input.GetKeyDown(_instagibGunKey))
        {
            AddInstagramGunToPlayer();
        }
        if (Input.GetKeyDown(_scoreKey))
        {
            AddScore();
        }
        if (Input.GetKeyDown(_ammoKey))
        {
            AddAmmo();
        }
    }

    private void AddAmmo()
    {
        foreach (var weapon in GameManager.Instance.Player.GetAllWeapons())
        {
            weapon.totalAmmo = weapon.weaponDetails.ammoCapacity;
        }
    }

    private void AddInstagramGunToPlayer()
    {
        CardSystem.Instance.Hand.Add(GameResources.Instance.instagibCard, Settings.maxCardLevel);
    }

    private void AddScore()
    {
        StaticEventHandler.CallPointScoredEvent(SCORE_AMOUNT);
    }

    private void TeleportPlayerToBoss()
    {
        if (DungeonBuilder.Instance == null)
        {
            return;
        }

        foreach (var room in DungeonBuilder.Instance.GetAllRooms())
        {
            if (!room.nodeType.isBossRoom)
            {
                continue;
            }

            var player = GameManager.Instance.Player;

            player.gameObject.transform.position = new Vector3(
                (room.lowerBound.x + room.upperBound.x) / 2f,
                (room.lowerBound.y + room.upperBound.y) / 2f,
                0f
            );

            player.gameObject.transform.position = HelperUtilities.GetNearestSpawnPointFromRoom(player.gameObject.transform.position, room);
        }
    }

    private void FillHealth()
    {
        GameManager.Instance.Player.health.Heal(HEAL_AMOUND);
    }

    private void FillShield()
    {
        GameManager.Instance.Player.health.AddArmor(SHIELD_AMOUND);
    }
}

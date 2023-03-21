using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CardSystemSettings))]
public class CardPickUpSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _cardPickUpPrefab;
    [SerializeField] private int _cardSpawnPointThreshold = 200;

    private CardSystemSettings _settings;
    private int _points = 0;
    private Vector3 _lastEnemyDiedPosition = new Vector3();

    private void Awake()
    {
        _settings = GetComponent<CardSystemSettings>();
    }

    private void OnEnable()
    {
        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;
        StaticEventHandler.OnPointsScored += StaticEventHandler_OnPointsScored;
        StaticEventHandler.OnEnemyDied += StaticEventHandler_OnEnemyDied;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;
        StaticEventHandler.OnPointsScored -= StaticEventHandler_OnPointsScored;
        StaticEventHandler.OnEnemyDied -= StaticEventHandler_OnEnemyDied;
    }

    private void StaticEventHandler_OnPointsScored(PointsScoredArgs obj)
    {
        _points += obj.points;

        if (_points > _cardSpawnPointThreshold)
        {
            _points = 0;
            SpawnCard(CardRarity.Common);
        }
    }

    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedEventArgs obj)
    {
        if (obj.room.nodeType.isBossRoom)
        {
            SpawnCard(CardRarity.Epic);
        }
        else
        {
            SpawnCard(CardRarity.Rare);
        }
    }

    private void StaticEventHandler_OnEnemyDied(EnemyDiedEventArgs obj)
    {
        _lastEnemyDiedPosition = obj.enemy.transform.position;
    }

    private void SpawnCard(CardRarity rarity)
    {
        var card = (CardPickUp)PoolManager.Instance.ReuseComponent(_cardPickUpPrefab, _lastEnemyDiedPosition, Quaternion.identity);
        card.SetColor(_settings.GetColor(rarity));
        card.EnsuredCardRarity = rarity;
        card.gameObject.SetActive(true);
    }
}

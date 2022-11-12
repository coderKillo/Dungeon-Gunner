using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MusicManager))]
[DisallowMultipleComponent]
public class MusicSelector : MonoBehaviour
{
    [SerializeField] private MusicTrackSO _ambienceMusic;
    [SerializeField] private MusicTrackSO _chestRoomMusic;
    [SerializeField] private MusicTrackSO _bossMusic;
    [SerializeField] private MusicTrackSO _battleMusic;

    private MusicManager _musicManager;


    private void Awake()
    {
        _musicManager = GetComponent<MusicManager>();
    }

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
        StaticEventHandler.OnRoomEnemiesEngaging += StaticEventHandler_OnRoomEnemiesEngaging;
        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
        StaticEventHandler.OnRoomEnemiesEngaging -= StaticEventHandler_OnRoomEnemiesEngaging;
        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;
    }

    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedEventArgs obj)
    {
        if (obj.room.nodeType.isChestRoom)
        {
            _musicManager.PlayMusic(_chestRoomMusic, 0.2f, 0.5f);
        }
        else
        {
            _musicManager.PlayMusic(_ambienceMusic, 0.2f, 0.5f);
        }
    }

    private void StaticEventHandler_OnRoomEnemiesEngaging(RoomEnemiesEngagingEventArgs obj)
    {
        if (obj.room.nodeType.isBossRoom)
        {
            _musicManager.PlayMusic(_bossMusic, 0.2f, 0.5f);
        }
        else
        {
            _musicManager.PlayMusic(_battleMusic, 0.2f, 0.5f);
        }
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs obj)
    {
        if (obj.room.nodeType.isChestRoom)
        {
            _musicManager.PlayMusic(_chestRoomMusic, 0.2f, 0.5f);
        }
        else
        {
            _musicManager.PlayMusic(_ambienceMusic, 0.2f, 0.5f);
        }
    }
}

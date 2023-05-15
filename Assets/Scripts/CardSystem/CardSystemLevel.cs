using System;
using System.Collections.Generic;
using UnityEngine;

public class CardSystemLevel : MonoBehaviour
{
    [SerializeField] private int _pointsPerLevel = 1000;
    [SerializeField] private int _epicCardSpawnLevelInterval = 5;

    static public Action<int> OnLevelChange;
    static public Action<float> OnLevelPercentageChange;

    private CardSystem _cardSystem;

    private float _levelPercentage = 0f;
    private int _level = 1;

    public int GetRandomCardSpawnLevel()
    {
        int lowerLevelBoundary = _level / 10;
        int upperLevelBoundary = lowerLevelBoundary + 1;
        float chancesToUpperBoundary = (_level % 10) / 10;

        if (UnityEngine.Random.Range(0f, 1f) >= chancesToUpperBoundary)
        {
            return Math.Clamp(lowerLevelBoundary, 1, Settings.maxCardLevel);
        }
        else
        {
            return Math.Clamp(upperLevelBoundary, 1, Settings.maxCardLevel);
        }
    }

    private void Awake()
    {
        _cardSystem = GetComponent<CardSystem>();
    }

    private void OnEnable()
    {
        StaticEventHandler.OnPointsScored += StaticEventHandler_OnPointsScored;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnPointsScored -= StaticEventHandler_OnPointsScored;
    }

    private void StaticEventHandler_OnPointsScored(PointsScoredArgs obj)
    {
        _levelPercentage += (float)obj.points / (float)_pointsPerLevel;

        if (_levelPercentage >= 1)
        {
            _levelPercentage -= 1;
            LevelUp();
        }

        OnLevelPercentageChange?.Invoke(_levelPercentage);
    }

    private void LevelUp()
    {
        _level++;
        OnLevelChange?.Invoke(_level);

        if (_level % _epicCardSpawnLevelInterval == 0)
        {
            _cardSystem.Draw(CardRarity.Epic);
        }
        else
        {
            _cardSystem.Draw(CardRarity.Common);
        }
    }
}

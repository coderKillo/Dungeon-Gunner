using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class CardSystemLevel : MonoBehaviour
{
    [SerializeField] private int _pointsPerLevel = 1000;
    [SerializeField] private int _epicCardSpawnLevelInterval = 5;

    [Header("SOUNDS EFFECTS")]
    [SerializeField] private SoundEffectSO _levelUpSound;

    static public Action<int> OnLevelChange;
    static public Action<float> OnLevelPercentageChange;

    private CardSystem _cardSystem;

    private int _currentPoints = 0;
    private int _level = 1;

    public int GetRandomCardSpawnLevel()
    {
        int lowerLevelBoundary = _level / 10;
        int upperLevelBoundary = lowerLevelBoundary + 1;
        float chancesToUpperBoundary = (_level % 10) / 10f;

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

    private void Start()
    {
        OnLevelChange?.Invoke(_level);
        OnLevelPercentageChange?.Invoke(0f);
    }

    private void StaticEventHandler_OnPointsScored(PointsScoredArgs obj)
    {
        _currentPoints += obj.points;

        while (_currentPoints >= _pointsPerLevel)
        {
            _currentPoints -= _pointsPerLevel;
            LevelUp();
        }

        OnLevelPercentageChange?.Invoke((float)_currentPoints / (float)_pointsPerLevel);
    }

    private void LevelUp()
    {
        SoundEffectManager.Instance.PlaySoundEffect(_levelUpSound);

        _level++;
        OnLevelChange?.Invoke(_level);

        UpdatePointsPerLevel();

        if (_level % _epicCardSpawnLevelInterval == 0)
        {
            _cardSystem.Draw(CardRarity.Epic);
        }
        else
        {
            _cardSystem.Draw(CardRarity.Common);
        }
    }

    private void UpdatePointsPerLevel()
    {
        _pointsPerLevel = Mathf.RoundToInt(1000 * (1 + Mathf.Log(_level, 3)));
    }
}

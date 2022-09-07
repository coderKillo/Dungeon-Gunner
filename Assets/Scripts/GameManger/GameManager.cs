using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : SingletonAbstract<GameManager>
{
    #region HEADER
    [Space(10)]
    [Header("DUNGEON LEVEL")]

    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;
    [SerializeField] private int currentLevelIndex = 0;
    [HideInInspector] public GameState gameState;
    #endregion

    private void Start()
    {
        gameState = GameState.gameStarted;
    }

    private void Update()
    {
        HandleGameState();

        #region RESTART FOR TESTING
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameState = GameState.gameStarted;
        }
        #endregion
    }

    private void HandleGameState()
    {
        switch (gameState)
        {
            case GameState.gameStarted:

                PlayDungeonLevel(currentLevelIndex);

                gameState = GameState.levelCompleted;

                break;

            case GameState.levelCompleted:
                break;
        }

    }

    private void PlayDungeonLevel(int levelIndex)
    {
    }

    #region UNITY EDITOR
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
    #endregion
}

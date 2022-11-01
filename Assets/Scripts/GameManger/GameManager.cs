using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class GameManager : SingletonAbstract<GameManager>
{
    #region HEADER
    [Space(10)]
    [Header("DUNGEON LEVEL")]

    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;
    [SerializeField] private int currentLevelIndex = 0;

    private GameState previousGameState = GameState.none;
    private GameState gameState = GameState.none;
    public GameState GameState { get { return gameState; } }
    public void SetGameState(GameState state)
    {
        if (state == gameState)
        {
            return;
        }

        previousGameState = gameState;
        gameState = state;

        HandleGameStateChange();
    }

    private Room currentRoom;
    public Room CurrentRoom { get { return currentRoom; } }
    public void SetCurrentRoom(Room room)
    {
        previousRoom = currentRoom;
        currentRoom = room;
    }

    private Room previousRoom;

    public DungeonLevelSO CurrentLevel { get { return dungeonLevelList[currentLevelIndex]; } }

    private PlayerDetailsSO playerDetails;
    private Player player;
    public Player Player { get { return player; } }
    public Sprite PlayerIcon { get { return player.playerDetails.minimapIcon; } }
    public Vector3 PlayerPosition { get { return player.transform.position; } }

    private long score = 0;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        playerDetails = GameResources.Instance.currentPlayer.playerDetails;

        var playerGameObject = Instantiate(playerDetails.prefab);

        player = playerGameObject.GetComponent<Player>();
        player.Initialize(playerDetails);
    }

    private void Start()
    {
        SetGameState(GameState.gameStarted);

        SetScore(0);
    }

    private void OnEnable()
    {
        StaticEventHandler.OnPointsScored += StaticEventHandler_OnPointsScored;
        StaticEventHandler.OnRoomEnemiesEngaging += StaticEventHandOnRoomEnemiesEngaging;
        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnEnemiesDefeated;
        player.destroyedEvent.OnDestroyed += PlayerDestroyedEvent_OnDestroyed;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnPointsScored -= StaticEventHandler_OnPointsScored;
        StaticEventHandler.OnRoomEnemiesEngaging -= StaticEventHandOnRoomEnemiesEngaging;
        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnEnemiesDefeated;
        player.destroyedEvent.OnDestroyed -= PlayerDestroyedEvent_OnDestroyed;
    }

    private void HandleGameStateChange()
    {
        switch (gameState)
        {
            case GameState.gameStarted:

                PlayDungeonLevel(currentLevelIndex);

                break;


            case GameState.playingLevel:

                if (previousGameState == GameState.engagingBoss)
                {
                    SetGameState(GameState.levelCompleted);
                }

                break;


            case GameState.engagingBoss:
                break;


            case GameState.engagingEnemies:
                break;


            case GameState.gameWon:
                Debug.Log("Game Won!");
                break;


            case GameState.gameLost:

                SetGameState(GameState.restartGame);

                break;


            case GameState.gamePaused:
                break;


            case GameState.restartGame:

                StartCoroutine(RestartLevel());

                break;


            case GameState.dungeonOverviewMap:
                break;


            case GameState.levelCompleted:

                currentLevelIndex++;

                if (currentLevelIndex >= dungeonLevelList.Count)
                {
                    SetGameState(GameState.gameWon);
                }

                StopAllCoroutines();
                StartCoroutine(LevelComplete());

                break;
        }

    }

    private IEnumerator LevelComplete()
    {
        Debug.Log("Level Completed! Next level starts in 2 seconds");

        yield return new WaitForSeconds(2f);

        PlayDungeonLevel(currentLevelIndex);
    }

    private IEnumerator RestartLevel()
    {
        Debug.Log("Restart level in 5 seconds");

        yield return new WaitForSeconds(5f);

        SceneManager.LoadScene("MainGameScene");
    }

    private void PlayDungeonLevel(int levelIndex)
    {
        bool buildSuccessful = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[levelIndex]);

        if (!buildSuccessful)
        {
            Debug.LogError("Couldn't build dungeon from specified dungeon level");
        }

        StaticEventHandler.CallRoomChangedEvent(currentRoom);

        player.gameObject.transform.position = new Vector3(
            (currentRoom.lowerBound.x + currentRoom.upperBound.x) / 2f,
            (currentRoom.lowerBound.y + currentRoom.upperBound.y) / 2f,
            0f
        );

        player.gameObject.transform.position = HelperUtilities.GetNearestSpawnPoint(player.gameObject.transform.position);

        SetGameState(GameState.playingLevel);
    }

    public void SetScore(long amount)
    {
        score = amount;
        StaticEventHandler.CallScoreChangedEvent(score);
    }

    private void StaticEventHandler_OnPointsScored(PointsScoredArgs args)
    {
        SetScore(score + args.points);
    }

    private void PlayerDestroyedEvent_OnDestroyed(DestroyedEvent arg1, DestroyedEventArgs arg2)
    {
        SetGameState(GameState.gameLost);
    }

    private void StaticEventHandler_OnEnemiesDefeated(RoomEnemiesDefeatedEventArgs obj)
    {
        SetGameState(GameState.playingLevel);
    }

    private void StaticEventHandOnRoomEnemiesEngaging(RoomEnemiesEngagingEventArgs obj)
    {
        if (obj.room.nodeType.isBossRoom)
        {
            SetGameState(GameState.engagingBoss);
        }
        else
        {
            SetGameState(GameState.engagingEnemies);
        }
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

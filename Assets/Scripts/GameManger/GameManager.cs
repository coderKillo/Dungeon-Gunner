using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

[DisallowMultipleComponent]
public class GameManager : SingletonAbstract<GameManager>
{
    #region HEADER
    [Space(10)]
    [Header("DUNGEON LEVEL")]

    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;
    [SerializeField] private int currentLevelIndex = 0;
    public DungeonLevelSO CurrentLevel { get { return dungeonLevelList[currentLevelIndex]; } }

    private GameState previousGameState = GameState.none;
    private GameState gameState = GameState.none;
    [ShowInInspector] public GameState GameState { get { return gameState; } }
    [ShowInInspector] public GameState PreviousGameState { get { return previousGameState; } }
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

    [Space(10)]
    [Header("PLAYER")]

    private PlayerDetailsSO playerDetails;
    private Player player;
    public Player Player { get { return player; } }
    public Sprite PlayerIcon { get { return player.playerDetails.minimapIcon; } }
    public Vector3 PlayerPosition { get { return player.transform.position; } }

    private long score = 0;
    #endregion

    private DisplayMessage displayMessage;
    private PauseMenu pauseMenu;

    protected override void Awake()
    {
        base.Awake();

        displayMessage = GetComponent<DisplayMessage>();
        pauseMenu = GetComponent<PauseMenu>();

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && gameState == GameState.playingLevel)
        {
            SetGameState(GameState.dungeonOverviewMap);
        }

        if (Input.GetKeyUp(KeyCode.Tab) && gameState == GameState.dungeonOverviewMap)
        {
            DungeonMap.Instance.ClearDungeonMap();
            SetGameState(GameState.playingLevel);
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (gameState == GameState.engagingBoss
            || gameState == GameState.engagingEnemies
            || gameState == GameState.bossDefeated
            || gameState == GameState.playingLevel)
        {
            SetGameState(GameState.gamePaused);
        }

        else if (gameState == GameState.gamePaused)
        {
            pauseMenu.ClearPauseMenu();
            SetGameState(previousGameState);
        }
    }

    private void HandleGameStateChange()
    {
        switch (gameState)
        {
            case GameState.gameStarted:

                PlayDungeonLevel(currentLevelIndex);

                break;


            case GameState.playingLevel:
                break;

            case GameState.bossDefeated:

                Portal.SpawnPortal(PlayerPosition);

                displayMessage.DisplayText("Boss Defeated! Use portal to enter the next level", 2f, Color.white, 0.5f, 1f);

                SetGameState(GameState.playingLevel);
                break;


            case GameState.engagingBoss:
                break;


            case GameState.engagingEnemies:
                break;


            case GameState.gameWon:
                displayMessage.DisplayText("Game Won!", 5f, Color.white);
                break;


            case GameState.gameLost:

                SetGameState(GameState.restartGame);

                break;


            case GameState.gamePaused:

                pauseMenu.DisplayPauseMenu();

                break;


            case GameState.restartGame:

                StartCoroutine(RestartLevel());

                break;


            case GameState.dungeonOverviewMap:

                DungeonMap.Instance.DisplayDungeonMap();

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
        displayMessage.DisplayText("Level Completed! Next level starts in 2 seconds", 2f, Color.white, 1f, 1f);

        yield return new WaitForSeconds(2f);

        PlayDungeonLevel(currentLevelIndex);
    }

    private IEnumerator RestartLevel()
    {
        displayMessage.DisplayText("Restart level in 5 seconds", 5f, Color.white, 0.5f, 1f);

        yield return new WaitForSeconds(5f);

        SceneManager.LoadScene("MainMenuScene");
    }

    private void PlayDungeonLevel(int levelIndex)
    {
        bool buildSuccessful = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[levelIndex]);

        if (!buildSuccessful)
        {
            Debug.LogError("Couldn't build dungeon from specified dungeon level");
        }

        displayMessage.DisplayText("Level " + (levelIndex + 1) + "\n\n" + dungeonLevelList[levelIndex].levelName, 1f, Color.white);

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
        if (obj.room.nodeType.isBossRoom)
        {
            SetGameState(GameState.bossDefeated);
        }
        else
        {
            SetGameState(GameState.playingLevel);
        }
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

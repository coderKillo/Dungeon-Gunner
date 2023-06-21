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
    [SerializeField] private int prestigeLevel = 0;
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
    public DisplayMessage DisplayMessage { get { return displayMessage; } }

    private PauseMenu pauseMenu;
    private DeathMenu deathMenu;

    private Tutorial tutorial;

    protected override void Awake()
    {
        base.Awake();

        displayMessage = GetComponent<DisplayMessage>();
        pauseMenu = GetComponent<PauseMenu>();
        deathMenu = GetComponent<DeathMenu>();

        tutorial = GetComponent<Tutorial>();
        tutorial.enabled = false;

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
        tutorial.OnTutorialDone += Tutorial_OnTutorialDone;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnPointsScored -= StaticEventHandler_OnPointsScored;
        StaticEventHandler.OnRoomEnemiesEngaging -= StaticEventHandOnRoomEnemiesEngaging;
        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnEnemiesDefeated;
        player.destroyedEvent.OnDestroyed -= PlayerDestroyedEvent_OnDestroyed;
        tutorial.OnTutorialDone -= Tutorial_OnTutorialDone;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && gameState == GameState.playingLevel)
        {
            SetGameState(GameState.dungeonOverviewMap);
        }

        if (Input.GetKeyUp(KeyCode.M) && gameState == GameState.dungeonOverviewMap)
        {
            DungeonMap.Instance.ClearDungeonMap();
            SetGameState(GameState.playingLevel);
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("MainGameScene");
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

                tutorial.enabled = true;
                break;


            case GameState.tutorialDone:
                Invoke(nameof(DrawStartCards), 1f);

                break;


            case GameState.playingLevel:
                break;


            case GameState.bossDefeated:

                Portal.SpawnPortal(PlayerPosition);

                displayMessage.DisplayText("Boss Slain!", "Enter the portal to continue to the next level.\n(Press 'E')", 2f, 0.5f, 1f);

                SetGameState(GameState.playingLevel);
                break;


            case GameState.engagingBoss:
                break;


            case GameState.engagingEnemies:
                break;


            case GameState.gameWon:
                displayMessage.DisplayText("Game Won!", "", 5f);
                break;


            case GameState.gameLost:

                StopAllCoroutines();
                StartCoroutine(GameLost());

                break;


            case GameState.gamePaused:

                pauseMenu.DisplayPauseMenu();

                break;


            case GameState.restartGame:
                break;


            case GameState.dungeonOverviewMap:

                DungeonMap.Instance.DisplayDungeonMap();

                break;


            case GameState.levelCompleted:

                currentLevelIndex++;

                if (currentLevelIndex >= dungeonLevelList.Count)
                {
                    NextPrestigeLevel();
                    currentLevelIndex = 0;
                }

                StopAllCoroutines();
                StartCoroutine(LevelComplete());

                break;
        }

    }

    private void NextPrestigeLevel()
    {
        prestigeLevel++;

        StaticEventHandler.CallDifficultyChange(prestigeLevel * Settings.difficultyFactor);
    }

    private IEnumerator LevelComplete()
    {
        yield return null;

        PlayDungeonLevel(currentLevelIndex);
    }

    private IEnumerator GameLost()
    {
        yield return new WaitForSeconds(2f);

        deathMenu.DisplayMenu();
    }

    private void PlayDungeonLevel(int levelIndex)
    {
        if (dungeonLevelList.Count <= 0 || !DungeonBuilder.Instance)
        {
            return;
        }

        bool buildSuccessful = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[levelIndex]);

        if (!buildSuccessful)
        {
            Debug.LogError("Couldn't build dungeon from specified dungeon level");
        }

        var levelText = $@" 
{dungeonLevelList[levelIndex].levelName}";

        if (prestigeLevel > 0)
        {
            levelText += $@" 
            
Prestige {prestigeLevel}
Monster Damage +{prestigeLevel * Settings.difficultyFactor * 100}%
Monster Health +{prestigeLevel * Settings.difficultyFactor * 100}%
Monster Spawn Amount +{prestigeLevel * Settings.difficultyFactor * 100}% ";
        }

        displayMessage.DisplayText($"Level {levelIndex + 1}", levelText, 1f);

        StaticEventHandler.CallRoomChangedEvent(currentRoom);

        player.gameObject.transform.position = new Vector3(
            (currentRoom.lowerBound.x + currentRoom.upperBound.x) / 2f,
            (currentRoom.lowerBound.y + currentRoom.upperBound.y) / 2f,
            0f
        );

        player.gameObject.transform.position = HelperUtilities.GetNearestSpawnPoint(player.gameObject.transform.position);

        SetGameState(GameState.playingLevel);
    }


    private void DrawStartCards()
    {
        for (int i = 0; i < Settings.cardDrawOnStart; i++)
        {
            CardSystem.Instance.Draw(CardRarity.Rare);
        }
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

    private void Tutorial_OnTutorialDone()
    {
        SetGameState(GameState.tutorialDone);
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

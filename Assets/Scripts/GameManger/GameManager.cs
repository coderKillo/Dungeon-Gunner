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

    private Room currentRoom;
    public Room CurrentRoom { get { return currentRoom; } }
    public void SetCurrentRoom(Room room)
    {
        previousRoom = currentRoom;
        currentRoom = room;
    }

    private Room previousRoom;
    private PlayerDetailsSO playerDetails;

    private Player player;
    public Player Player { get { return player; } }
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
        bool buildSuccessful = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[levelIndex]);

        if (!buildSuccessful)
        {
            Debug.LogError("Couldn't build dungeon from specified dungeon level");
        }

        player.gameObject.transform.position = new Vector3(
            (currentRoom.lowerBound.x + currentRoom.upperBound.x) / 2f,
            (currentRoom.lowerBound.y + currentRoom.upperBound.y) / 2f,
            0f
        );

        player.gameObject.transform.position = HelperUtilities.GetNearestSpawnPoint(player.gameObject.transform.position);
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

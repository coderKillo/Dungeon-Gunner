using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyMovementAI : MonoBehaviour
{
    [SerializeField] private MovementDetailsSO movementDetails;
    public bool debugPath = false;

    [HideInInspector] public float moveSpeed;
    [HideInInspector] public int updateFrameNumber = 1;

    private Enemy enemy;

    private Stack<Vector3> path = new Stack<Vector3>();
    private Vector3 prevPlayerPosition;
    private Coroutine moveEnemyCoroutine;
    private float rebuildCooldownTimer;
    private WaitForFixedUpdate waitForFixedUpdate;
    private bool chasePlayer;
    private List<Vector3> surroundingPositionList = new List<Vector3>();

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        moveSpeed = movementDetails.GetRandomMovementSpeed();
    }

    void Start()
    {
        waitForFixedUpdate = new WaitForFixedUpdate();
        prevPlayerPosition = GameManager.Instance.PlayerPosition;
    }

    void Update()
    {
        var playerPosition = GameManager.Instance.PlayerPosition;

        if (Vector3.Distance(playerPosition, enemy.transform.position) < enemy.enemyDetails.chaseDistance)
        {
            chasePlayer = true;
        }

        if (!chasePlayer)
        {
            return;
        }

        if (Time.frameCount % Settings.targetFrameRateToSpreadRebuildPath != updateFrameNumber)
        {
            return;
        }

        rebuildCooldownTimer -= Time.deltaTime;

        if (Vector3.Distance(prevPlayerPosition, playerPosition) >= Settings.playerMoveDistanceToRebuildPath || rebuildCooldownTimer < 0f)
        {
            rebuildCooldownTimer = Settings.enemyPathRebuildCooldown;
            prevPlayerPosition = playerPosition;

            CreatePath();
            MoveEnemy();
        }
    }

    private void CreatePath()
    {
        var room = GameManager.Instance.CurrentRoom;
        var grid = room.instantiatedRoom.grid;

        var startPosition = grid.WorldToCell(enemy.transform.position);
        var endPosition = GetPlayerPosition();

        path = AStar.BuildPath(room, startPosition, endPosition);

        if (path != null)
        {
            path.Pop(); // remove position enemy is already on
        }
        else
        {
            Idle();
        }
    }

    private Vector3Int GetPlayerPosition()
    {
        var room = GameManager.Instance.CurrentRoom;
        var grid = room.instantiatedRoom.grid;
        var playerAbsolutePos = grid.WorldToCell(GameManager.Instance.PlayerPosition);
        var playerRelativePos = (Vector2Int)playerAbsolutePos - room.templateLowerBound;

        if (!room.instantiatedRoom.IsObstacle(playerRelativePos))
        {
            return playerAbsolutePos;
        }

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                var position = playerRelativePos + new Vector2Int(i, j);
                try
                {
                    if (!room.instantiatedRoom.IsObstacle(position))
                    {
                        return playerAbsolutePos + new Vector3Int(i, j, 0);
                    }
                }
                catch // if is out of bounce
                {

                }
            }
        }

        return (Vector3Int)room.RandomSpawnPosition();
    }

    private void MoveEnemy()
    {
        if (path == null)
        {
            return;
        }

        if (moveEnemyCoroutine != null)
        {
            StopCoroutine(moveEnemyCoroutine);
            Idle();
        }


        if (debugPath)
        {
            DebugPath();
        }

        moveEnemyCoroutine = StartCoroutine(MoveEnemyCoroutine(path));
    }

    private void DebugPath()
    {
        var room = GameManager.Instance.CurrentRoom;
        var frontTilemapClone = room.instantiatedRoom.transform.Find("Grid/Tilemap4_Front(Clone)");

        if (frontTilemapClone != null)
        {
            var pathTilemap = frontTilemapClone.GetComponent<Tilemap>();
            var grid = room.instantiatedRoom.grid;
            var startTile = GameResources.Instance.preferredEnemyPath;

            pathTilemap.ClearAllTiles();

            foreach (var pos in path)
            {
                pathTilemap.SetTile(grid.WorldToCell(pos), startTile);
            }
        }

    }

    private IEnumerator MoveEnemyCoroutine(Stack<Vector3> steps)
    {
        while (steps.Count > 0)
        {
            var targetPosition = steps.Pop();
            var direction = (targetPosition - transform.position).normalized;

            while (Vector3.Distance(targetPosition, transform.position) > 0.2f)
            {
                enemy.movementToPositionEvent.CallMovementToPositionEvent(
                    transform.position, targetPosition, moveSpeed, direction, false);
                yield return waitForFixedUpdate;
            }

            yield return waitForFixedUpdate;
        }

        Idle();
    }

    private void Idle()
    {
        enemy.idleEvent.CallIdleEvent();
    }
}

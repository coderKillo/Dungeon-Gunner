using System.Security.AccessControl;
public enum Orientation
{
    north,
    east,
    south,
    west,
    none
}

public enum GameState
{
    none,
    gameStarted,
    playingLevel,
    engagingEnemies,
    engagingBoss,
    levelCompleted,
    gameWon,
    gameLost,
    gamePaused,
    dungeonOverviewMap,
    restartGame
}

public enum AimDirection
{
    Up,
    UpRight,
    UpLeft,
    Right,
    Left,
    Down
}

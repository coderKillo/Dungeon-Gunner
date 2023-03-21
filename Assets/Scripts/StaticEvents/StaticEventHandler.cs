using System;

public static class StaticEventHandler
{
    public static event Action<RoomChangedEventArgs> OnRoomChanged;

    public static void CallRoomChangedEvent(Room room)
    {
        OnRoomChanged?.Invoke(new RoomChangedEventArgs() { room = room });
    }

    public static event Action<RoomEnemiesEngagingEventArgs> OnRoomEnemiesEngaging;

    public static void CallRoomEnemiesEngaging(Room room)
    {
        OnRoomEnemiesEngaging?.Invoke(new RoomEnemiesEngagingEventArgs() { room = room });
    }

    public static event Action<RoomEnemiesDefeatedEventArgs> OnRoomEnemiesDefeated;

    public static void CallRoomEnemiesDefeated(Room room)
    {
        OnRoomEnemiesDefeated?.Invoke(new RoomEnemiesDefeatedEventArgs() { room = room });
    }

    public static event Action<PointsScoredArgs> OnPointsScored;

    public static void CallPointScoredEvent(int points)
    {
        OnPointsScored?.Invoke(new PointsScoredArgs() { points = points });
    }


    public static event Action<ScoreChangedArgs> OnScoreChanged;

    public static void CallScoreChangedEvent(long score)
    {
        OnScoreChanged?.Invoke(new ScoreChangedArgs() { score = score });
    }

    public static event Action<EnemySpawnedEventArgs> OnEnemySpawned;

    public static void CallEnemySpawned(Enemy enemy)
    {
        OnEnemySpawned?.Invoke(new EnemySpawnedEventArgs() { enemy = enemy });
    }

    public static event Action<EnemyDiedEventArgs> OnEnemyDied;

    public static void CallEnemyDied(Enemy enemy)
    {
        OnEnemyDied?.Invoke(new EnemyDiedEventArgs() { enemy = enemy });
    }
}

public class RoomChangedEventArgs : EventArgs
{
    public Room room;
}

public class RoomEnemiesEngagingEventArgs : EventArgs
{
    public Room room;
}

public class RoomEnemiesDefeatedEventArgs : EventArgs
{
    public Room room;
}

public class PointsScoredArgs : EventArgs
{
    public int points;
}

public class ScoreChangedArgs : EventArgs
{
    public long score;
}

public class EnemySpawnedEventArgs : EventArgs
{
    public Enemy enemy;
}

public class EnemyDiedEventArgs : EventArgs
{
    public Enemy enemy;
}


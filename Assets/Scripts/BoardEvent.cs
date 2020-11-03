using System;

[Serializable]
public enum BoardEvent
{
    BlockSpawned,
    CollapseRows,
    LockBlock,
    Move,
    UnvalidMove,
    PlayerAction,
    Scores,
    Level,
    GameOver,
}
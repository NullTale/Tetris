using System.Collections.Generic;
using Core.EventSystem;
using Tetris.Engine;

public class BoardEventHandler : IBoardEventHandler
{
    //////////////////////////////////////////////////////////////////////////
    public void OnBlockSpawned(Block block)
    {
        MessageSystem.Send(BoardEvent.BlockSpawned, block);
    }
        
    public void OnCollapseRows(List<int> rows)
    {
        MessageSystem.Send(BoardEvent.CollapseRows, rows);
    }
        
    public void OnLockBlock(Block block)
    {
        MessageSystem.Send(BoardEvent.LockBlock, block);
    }
        
    public void OnMove(Block block, Move move)
    {
        MessageSystem.Send(BoardEvent.Move, block, move);
    }

    public void OnGameOver()
    {
        MessageSystem.Send(BoardEvent.GameOver);
    }

    public void OnUnvalidMove(Move move)
    {
        MessageSystem.Send(BoardEvent.UnvalidMove, move);
    }
}
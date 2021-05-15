using System;
using System.Collections.Generic;
using Core.EventSystem;
using NaughtyAttributes;
using UnityEngine;

public class ScoresCounter : MessageListener<BoardEvent>
{
    /*
        1 Line = 50*(level + 1) points
        2 Lines = 150*(level + 1) points
        3 Lines = 350*(level + 1) points
        4 Lines = 1000*(level + 1) points (aka a Tetris)
        Clear the board = 2000*(level + 1)
        Every piece = 10*(level + 1) points
    */

    [Serializable]
    public enum ScoreEvent
    {
        Line1,
        Line2,
        Line3,
        Line4,
        Board,
        Piece,

        Force
    }

    //////////////////////////////////////////////////////////////////////////
    [SerializeField, ReadOnly]
    private int                 m_Scores;
    public int                  Scores
    {
        get => m_Scores;
        set
        {
            if (m_Scores == value)
                return;

            // save value & exchange
            var exchange = m_Scores - value;
            m_Scores = value;
            
            // send message
            MessageSystem.Send(BoardEvent.Scores, ScoreEvent.Force, exchange);
        }
    }

    //////////////////////////////////////////////////////////////////////////
    public override void ProcessMessage(IMessage<BoardEvent> e)
    {
        switch (e.Key)
        {
            case BoardEvent.CollapseRows:
            {
                var rows = e.GetData<List<int>>();

                // board cleared
                if (TetrisManager.Instance.GameManager.BoardManager.IsClear)
                {
                    addScores(ScoreEvent.Board, 2000 * (TetrisManager.Instance.LevelCounter.Level + 1));
                    break;
                }

                // rows cleared
                if (rows.Count != 0)
                {
                    switch (rows.Count)
                    {
                        case 1:
                            addScores(ScoreEvent.Line1, 50 * (TetrisManager.Instance.LevelCounter.Level + 1));
                            break;
                        case 2:
                            addScores(ScoreEvent.Line2, 150 * (TetrisManager.Instance.LevelCounter.Level + 1));
                            break;
                        case 3:
                            addScores(ScoreEvent.Line3, 350 * (TetrisManager.Instance.LevelCounter.Level + 1));
                            break;
                        case 4:
                            addScores(ScoreEvent.Line4, 1000 * (TetrisManager.Instance.LevelCounter.Level + 1));
                            break;
                    }
                    break;
                }
            } break;

            case BoardEvent.LockBlock:
            {
                // every piece
                addScores(ScoreEvent.Piece, 10 * (TetrisManager.Instance.LevelCounter.Level + 1));
            } break;
        }

        /////////////////////////////////////
        void addScores(ScoreEvent scoreEvent, int scores)
        {
            m_Scores += scores;
            MessageSystem.Send(BoardEvent.Scores, scoreEvent, scores);
        }
    }
}
using System;
using System.Linq;
using Core;
using Core.EventSystem;
using Tetris.Engine.AI;
using Tetris.Engine.AI.Algorithms;
using Tetris.Engine.AI.Algorithms.Weights;
using UnityEngine;

public class AIAgent : MessageListener<BoardEvent>
{
    [Serializable]
    public class MovePlan
    {
        public Tetris.Engine.Move[]     m_Moves;
        public int                      m_Current;
        public Tetris.Engine.Move       m_Move;

        //////////////////////////////////////////////////////////////////////////
        public MovePlan(Tetris.Engine.Move[] moves)
        {
            m_Moves = moves;
            m_Current = -1;
        }

        public Tetris.Engine.Move MakeMove()
        {
            m_Move = Tetris.Engine.Move.None;

            // get first appropriate move
            var current = m_Moves.Length - m_Moves.Skip(m_Current + 1).SkipWhile(n => n == Tetris.Engine.Move.None).Count();
            if (current < m_Moves.Length)
            {
                var move = m_Moves[current];

                // ignore fall if to close
                if (move == Tetris.Engine.Move.Fall 
                    && TetrisManager.Instance.GameManager.ActiveBlock != null
                    && TetrisManager.Instance.GameManager.BoardManager.GetCollisionHeight(TetrisManager.Instance.GameManager.ActiveBlock) <= Instance.m_IgnoreFallHeight)
                {
                    // save position
                    m_Current = current;
                    m_Move = move;
                }
                // implement move
                else if (TetrisManager.Instance.GameManager.MoveBlock(move))
                {
                    // save position
                    m_Current = current;
                    m_Move = move;
                }
            }

            return m_Move;
        }
    }

    //////////////////////////////////////////////////////////////////////////
    public static AIAgent           Instance;

    private TinyTimer               m_ActionTime;
    [SerializeField]
    private float                   m_ActionInterval;
    private Engine                  m_AI;

    [SerializeField]
    private float                   m_RotationCost = 0.2f;
    [SerializeField]
    private float                   m_MoveCost = 1.0f;
    [SerializeField]
    private float                   m_RepeatMoveMultiplier = 1.0f;
    [SerializeField]
    private float                   m_ConfusionDelay = 1.0f;
    [SerializeField]
    private int                     m_IgnoreFallHeight = 3;
    [SerializeField]
    private bool                    m_AutoFall;

    private Move                    m_Move;
    private MovePlan                m_MovePlan;
    private MovePlan                Plan
    {
        get => m_MovePlan ?? (m_MovePlan = _GetPlan());
        set
        {
            m_MovePlan = value;
            if (value == null)
                m_PrevMove = Tetris.Engine.Move.None;
        }
    }

    private Tetris.Engine.Move      m_PrevMove = Tetris.Engine.Move.None;

    [SerializeField]
    private TetrisAiWeights         m_AIWeights = new TetrisAiWeights 
    {
        ColumnTransitions   =  0.8024363520000000f,
        NumberOfHoles       =  5.0289489999999999f,
        RowTransitions      = -0.4794300500000000f,
        RowsCleared         = -2.0772042300000000f,
        WellSums            =  0.4410647000000000f
    };
    

    //////////////////////////////////////////////////////////////////////////
    public void Activate()
    {
        // set instance
        Instance = this;

        // set engine
        m_AI = new Engine(new TetrisAi(m_AIWeights));

        // first set action timer
        m_ActionTime = new TinyTimer(m_ActionInterval, true);

        // empty plan
        m_MovePlan = _GetEmptyPlan();

        // disable self
        enabled = false;

        // activate listener auto connection
        AutoConnect = true;
    }

    public override void OnEnable()
    {
        base.OnEnable();

        // reset plan require immediate action
        m_ActionTime?.Reset(0.0f, 0.0f);
        Plan = null;
    }

    public void Step(float deltaTime)
    {
        if (isActiveAndEnabled == false)
            return;

        // get & implement first move
        if (m_ActionTime.AddTime(deltaTime))
        {
            m_ActionTime.FinishTime = _getMoveCost(Plan.MakeMove()) * m_ActionInterval;
        }
            
    }

    public override void ProcessMessage(IMessage<BoardEvent> e)
    {
        switch (e.Key)
        {
            case BoardEvent.PlayerAction:
                m_ActionTime.Reset(0.0f, m_ConfusionDelay);
                Plan = null;
                break;
            case BoardEvent.BlockSpawned:
                Plan = null;
                break;
            case BoardEvent.LockBlock:
                Plan = null;
                break;
            case BoardEvent.UnvalidMove:
                m_ActionTime.Reset(0.0f, m_MoveCost);
                Plan = null;
                break;
        }
    }

    //////////////////////////////////////////////////////////////////////////
    private MovePlan _GetPlan()
    {
        m_PrevMove = Tetris.Engine.Move.None;

        var move = m_AI.GetMoves(TetrisManager.Instance.GameManager.BoardManager).FirstOrDefault();
        if (move == null)
            return _GetEmptyPlan();

        var movePlan = m_AutoFall ? move.Moves.Append(Tetris.Engine.Move.Fall).ToArray() : move.Moves;
        return new MovePlan(movePlan);
    }

    private static MovePlan _GetEmptyPlan()
    {
        return new MovePlan(new[]{Tetris.Engine.Move.None});
    }

    private float _getMoveCost(Tetris.Engine.Move move)
    {
        var mul = move == m_PrevMove ? m_RepeatMoveMultiplier : 1.0f;
        m_PrevMove = move;
        
        switch (move)
        {
            case Tetris.Engine.Move.None:
                return 0.2f;

            // move cost
            case Tetris.Engine.Move.Fall:
            case Tetris.Engine.Move.Down:
            case Tetris.Engine.Move.Left:
            case Tetris.Engine.Move.Right:
                return m_MoveCost * mul;

            // rotation cost
            case Tetris.Engine.Move.RotateRight:
            case Tetris.Engine.Move.RotateLeft:
                return m_RotationCost * mul;

            default:
                throw new ArgumentOutOfRangeException(nameof(move), move, null);
        }

    }
}
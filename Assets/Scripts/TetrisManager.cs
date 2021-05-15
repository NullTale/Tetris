using System.Collections.Generic;
using Core;
using Core.CommandSystem;
using NaughtyAttributes;
using Tetris.Engine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;
using UnityEngine.Tilemaps;


[assembly: Preserve]

[RequireComponent(typeof(CommandSequencer))]
public class TetrisManager : MonoBehaviour
{
    public static TetrisManager     Instance;

    private GameManager             m_GameManager;
    public GameManager              GameManager => m_GameManager;
    private TinyTimer               m_GameStepTimer;

    [SerializeField, ReadOnly]
    private float                   m_StepInterval;
    public float                    StepInterval
    {
        get => m_StepInterval;
        set
        {
            m_StepInterval = value;
            m_GameStepTimer.FinishTime = value;
        }
    }

    [SerializeField, ReadOnly]
    private bool                    m_Implementation;
    public bool                     Implementation
    {
        get => m_Implementation;
        set => m_Implementation = value;
    }

    private IBoardVisualizer        m_BoardVisualizer;
    public IBoardVisualizer         BoardVisualizer => m_BoardVisualizer;

    [SerializeField]
    private BlockProvider           m_BlockProvider;
    public BlockProvider            BlockProvider => m_BlockProvider;

    [SerializeField]
    private ScoresCounter           m_ScoresCounter;
    public ScoresCounter            ScoreCounter => m_ScoresCounter;
    [SerializeField]
    private LevelCounter            m_LevelCounter;
    public LevelCounter             LevelCounter => m_LevelCounter;

    public CommandSequencer         CommandSequencer { get; private set; }
    public PlayerController         PlayerController { get; private set; }

    public float                    SoftDropTimeLeft => m_GameStepTimer.Remainder * (1.0f / m_SoftDropTimeScale);

    [SerializeField]
    private bool                    m_ImplementationOnStart;
    [SerializeField]
    private bool                    m_Pause;
    public bool                     Pause
    {
        get => m_Pause;
        set => m_Pause = value;
    }

    [SerializeField]
    [Tooltip("Time multiplier for soft drop peaces")]
    private float                   m_SoftDropTimeScale;
    public float                    SoftDropTimeScale
    {
        get => m_SoftDropTimeScale;
        set => m_SoftDropTimeScale = value;
    }

    private float                   m_DeltaTime;
    [SerializeField]
    private bool                    m_PlayerControl;
    public bool                     PlayerControl
    {
        get => m_PlayerControl;
        set => m_PlayerControl = value;
    }

    public bool                     AI
    {
        get => AIAgent.Instance.enabled;
        set => AIAgent.Instance.enabled = value;
    }

    [SerializeField]
    private UnityEvent              m_OnGameOver;
    private bool                    m_OnGameOverTriggered;

    //////////////////////////////////////////////////////////////////////////
    public interface IBoardVisualizer
    {
        void Activate();
        void Deactivate();
        void Clear();
    }

    //////////////////////////////////////////////////////////////////////////
    private void Awake()
    {
        // set links
        Instance = this;
        CommandSequencer = GetComponent<CommandSequencer>();
        PlayerController = GetComponentInChildren<PlayerController>();
        m_BoardVisualizer = GetComponentInChildren<IBoardVisualizer>();
        m_BlockProvider = GetComponentInChildren<BlockProvider>();
    }

    private void Start()
    {
        // activate attached board visualizer
        BoardVisualizer?.Activate();

        // responsible for game execution
        m_GameManager = new GameManager(20, 10, new BoardEventHandler(), m_BlockProvider);

        // game step timer
        m_GameStepTimer = new TinyTimer(m_StepInterval, true);

        // activate AI
        GetComponentInChildren<AIAgent>()?.Activate();

        if (m_ImplementationOnStart)
            StartImplementation();
    }

    private void Update()
    {
        if (m_Pause)
            return;

        // update input
        PlayerController.UpdateCheckers();

        // do not increment delta time if animations playing
        if (Implementation)
            m_DeltaTime += Time.deltaTime;

        // plan game & ai step
        if (Implementation && CommandSequencer.IsRunning == false)
        {
            var deltaTime = m_DeltaTime;
            if (m_GameManager.IsSoftDrop)
                deltaTime *= 1.0f / m_SoftDropTimeScale;

            CommandSequencer.Push(() =>
            {
                // invoke player input
                if (PlayerControl)
                    PlayerController.InvokeCheckers();
                
                // invoke agent & game manager update
                AIAgent.Instance.Step(deltaTime);
                if (m_GameStepTimer.AddTime(deltaTime))
                    GameManager.OnGameLoopStep();
            });

            m_DeltaTime = 0;
        }

        if (GameManager.IsGameOver && m_OnGameOverTriggered == false)
        {
            m_OnGameOverTriggered = true;
            m_OnGameOver.Invoke();
        }
    }

    public static int GetCollisionHeight(List<Vector2Int> shape)
    {
        var collisionHeight = int.MaxValue;
        foreach (var coord in shape)
        {
            var steps = 0;
            for (var y = coord.y - 1; y >= 0; y--)
            {
                if (TetrisManager.Instance.GameManager.BoardManager.GameBoard[y][coord.x] == false)
                    steps ++;
                else
                    break;
            }

            collisionHeight = Mathf.Min(steps, collisionHeight);
        }

        return collisionHeight;
    }

    //////////////////////////////////////////////////////////////////////////
    [Button]
    public void StartImplementation()
    {
        Implementation = true;
    }

    [Button]
    public void PauseImplementation()
    {
        Implementation = false;
    }

    public void ClearBoard(int level)
    {
        CommandSequencer.Push(() =>
        {
            // remove next commands
            CommandSequencer.Clear();

            // create new board
            m_GameManager = new GameManager(20, 10, new BoardEventHandler(), m_BlockProvider);

            // clear visualizer
            BoardVisualizer.Clear();

            // discard scores
            m_ScoresCounter.Scores = 0;
            m_LevelCounter.Level = level;

            // activate game over trigger
            m_OnGameOverTriggered = false;
        });
    }
}
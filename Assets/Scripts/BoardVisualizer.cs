using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RotaryHeart.Lib.SerializableDictionary;
using Tetris.Engine;
using UnityEngine;
using Core;
using Core.CommandSystem;
using Core.Effect;
using Core.EventSystem;
using NaughtyAttributes;

[Serializable]
public class BoardVisualizer : MessageListener<BoardEvent>, TetrisManager.IBoardVisualizer
{
    private static readonly int                                 LevelUp = Animator.StringToHash("LevelUp");

    //////////////////////////////////////////////////////////////////////////
    [SerializeField]
    private SerializableDictionaryBase<BlockType, ShapeData>    m_ShapesData;
    public SerializableDictionaryBase<BlockType, ShapeData>     ShapesData => m_ShapesData;

    private ShapeVisualizer                                     m_ActiveShape;
    public ShapeVisualizer                                      ActiveShape => m_ActiveShape;

    private List<ShapeVisualizer>                               m_ShapeList = new List<ShapeVisualizer>();
    public List<ShapeVisualizer>                                ShapeList => m_ShapeList;

    [SerializeField]
    private float                                               m_UnvalidMoveOffset;

    [SerializeField, MinMaxSlider(0.5f, 10f)]
    private Vector2                                             m_BoardWeightRange;

    [SerializeField]
    private ShapePositionVisualizer                             m_ShapePositionVisualizer;

    private CommandSequencer                                    m_CommandSequencer;
    [SerializeField] [Foldout("Effects")]
    private Effect                                              m_Fall;
    [SerializeField] [Foldout("Effects")]
    private Effect                                              m_Collapse;
    [SerializeField] [Foldout("Effects")]
    private Effect                                              m_GameOver;
    [SerializeField] [Foldout("Effects")]
    private Effect                                              m_Flash;
    [SerializeField] [Foldout("Effects")] [SoundKey]
    private string                                              m_MoveSound;
    [SerializeField] [Foldout("Effects")] [SoundKey]
    private string                                              m_LockBlockSound;
    [SerializeField] [Foldout("Effects")] [SoundKey]
    private string                                              m_LevelUpSound;
    [SerializeField] [Foldout("Effects")] [SoundKey]
    private string                                              m_RowClearedSound;
    [SerializeField] [Foldout("Effects")] [SoundKey]
    private string                                              m_GameOverSound;
    [SerializeField] [Foldout("Effects")] [SoundKey]
    private string                                              m_UnvalidMoveSound;
    [SerializeField] [Foldout("Effects")]
    private GameObject                                          m_HillRoot;
    [SerializeField] [Foldout("Effects")]
    private bool                                                m_WaitFallAnimation;
    [SerializeField] [Foldout("Effects")]
    private bool                                                m_WaitCollapseAnimation;

    public bool WaitCollapseAnimation
    {
        get => m_WaitCollapseAnimation;
        set => m_WaitCollapseAnimation = value;
    }

    public bool WaitFallAnimation
    {
        get => m_WaitFallAnimation;
        set => m_WaitFallAnimation = value;
    }

    [SerializeField]
    private GameObject                                          m_Playfield;
    [SerializeField]
    private GameObject                                          m_PlayfieldRoot;
    public GameObject                                           PlayfieldRoot => m_PlayfieldRoot;


    [SerializeField]
    private ScoreVisualizer                                     m_ScoreVisualizer;
    [SerializeField]
    private LevelVisualizer                                     m_LevelVisualizer;
    [SerializeField]
    private NextShapeVisualizer                                 m_NextShapeVisualizer;

    [SerializeField]
    private bool                                                m_EnableShapePosition;
    [SerializeField] [DrawIf(nameof(m_EnableShapePosition), true)]
    private Transform                                           m_ActiveShapePosition;

    private bool            m_EnableGUI;
    public bool             EnableGUI
    {
        get => m_EnableGUI;
        set
        {
            if (m_EnableGUI == value)
                return;

            m_EnableGUI = value;

            m_ScoreVisualizer.gameObject.SetActive(m_EnableGUI);
            m_LevelVisualizer.gameObject.SetActive(m_EnableGUI);
            m_NextShapeVisualizer.gameObject.SetActive(m_EnableGUI);
        }
    }

    public bool             FixedPLayfield
    {
        get => m_HillRoot.GetComponent<Rigidbody2D>().isKinematic;
        set
        {
            var rb= m_HillRoot.GetComponent<Rigidbody2D>();
            rb.isKinematic = value;
            rb.velocity = Vector2.zero;
        }
    }


    //////////////////////////////////////////////////////////////////////////
    [Serializable]
    public class ShapeData
    {
        public GameObject   m_BlockPrefab;
        public Sprite       m_BlockPreview;
        
        //////////////////////////////////////////////////////////////////////////
        public ShapeVisualizer GetShapeVisualizer(Block block)
        {
            // instantiate & init block prefab
            var result = Instantiate(m_BlockPrefab, (TetrisManager.Instance.BoardVisualizer as BoardVisualizer).PlayfieldRoot.transform).GetComponent<ShapeVisualizer>();
            result.Init(block);
            
            // 
            return result;
        }
    }

    //////////////////////////////////////////////////////////////////////////
    public void Activate()
    {
        ConnectListener();
        m_CommandSequencer = TetrisManager.Instance.CommandSequencer;
        m_ShapePositionVisualizer.Init(this);

        m_ScoreVisualizer.Scores = TetrisManager.Instance.ScoreCounter.Scores;
        m_LevelVisualizer.Level = TetrisManager.Instance.LevelCounter.Level;
    }

    public void Deactivate()
    {
        DisconnectListener();
    }

    public void Clear()
    {
        foreach (var shape in ShapeList.ToList())
            foreach (var block in shape.Blocks.ToList())
                block.Destroy();
    }

    private void FixedUpdate()
    {
        // update shape position
        if (m_EnableShapePosition)
            // ReSharper disable once Unity.PerformanceCriticalCodeNullComparison
            if (m_ActiveShape != null)
            {
                m_ActiveShapePosition.position =
                    m_ActiveShape.Blocks.Aggregate(Vector3.zero, (seed, block) => seed + block.View.transform.position) /
                    m_ActiveShape.Blocks.Count;
            }
    }

    public override void ProcessMessage(IMessage<BoardEvent> e)
    {
        switch (e.Key)
        {
            case BoardEvent.BlockSpawned:
            {
                // spawn new active block
                var block = e.GetData<Block>();
                
                m_CommandSequencer.Push(() =>
                {
                    m_ActiveShape = m_ShapesData[block.BlockType].GetShapeVisualizer(block);

                    ShapeList.Add(m_ActiveShape);
                    
                    m_ShapePositionVisualizer.Set(m_ActiveShape?.Coords);
                });
            } break;
            case BoardEvent.CollapseRows:
            {
                var rows = e.GetData<List<int>>();

                m_CommandSequencer.Push(_Collapse(rows));

            } break;
            case BoardEvent.LockBlock:
            {
                m_CommandSequencer.Push(() =>
                {
                    m_ShapePositionVisualizer.Set(null);

                    // lock current active block
                    m_ActiveShape.OnLock(e.GetData<Block>());
            
                    // play sound
                    SoundManager.Sound.Play(m_LockBlockSound);

                    m_ActiveShape = null;

                    _UpdateBoardWeight();
                });
            } break;
            case BoardEvent.Move:
            {
                // move active block
                var (block, move) = e.GetData<Block, Move>();
                if (move == Move.Fall)
                    m_CommandSequencer.Push(_Fall(block, move));
                else
                    m_CommandSequencer.Push(() =>
                    {
                        m_ActiveShape.OnMove(block, move);
                        m_ShapePositionVisualizer.Set(m_ActiveShape?.Coords);
                    });

            } break;
            case BoardEvent.UnvalidMove:
            {
                m_CommandSequencer.Push(() =>
                {
                    SoundManager.Sound.Play(m_UnvalidMoveSound);

                    var move = e.GetData<Move>();
                    // ~move playfield
                    switch (move)
                    {
                        case Move.Left:
                            m_HillRoot.GetComponent<ApplyForce>().Apply(Vector2.left);
                            break;
                        case Move.Right:
                            m_HillRoot.GetComponent<ApplyForce>().Apply(Vector2.right);
                            break;
                    }

                    if (m_ActiveShape == null)
                        return;

                    // move active shape view follow to the direction
                    switch (move)
                    {
                        case Move.Left:
                            foreach (var block in m_ActiveShape.Blocks)
                                block.View.transform.localPosition += Vector3.left * m_UnvalidMoveOffset;
                            break;
                        case Move.RotateRight:
                            foreach (var block in m_ActiveShape.Blocks)
                                block.View.transform.localPosition += Vector3.up.normalized * m_UnvalidMoveOffset;
                            break;
                        case Move.RotateLeft:
                            foreach (var block in m_ActiveShape.Blocks)
                                block.View.transform.localPosition += Vector3.down * m_UnvalidMoveOffset;
                            break;
                        case Move.Right:
                            foreach (var block in m_ActiveShape.Blocks)
                                block.View.transform.localPosition += Vector3.right * m_UnvalidMoveOffset;
                            break;
                    }
                });
            } break;
            case BoardEvent.PlayerAction:
                SoundManager.Sound.Play(m_MoveSound);
                break;
            case BoardEvent.Scores:
            {
                var (se, exchange) = e.GetData<ScoresCounter.ScoreEvent, int>();
                m_ScoreVisualizer.Scores = TetrisManager.Instance.ScoreCounter.Scores;
            } break;
            case BoardEvent.Level:
            {
                var exchange = e.GetData<int>();
                m_LevelVisualizer.Level = TetrisManager.Instance.LevelCounter.Level;

                // do not play sound & animation with negative exchange
                if (exchange > 0)
                {
                    m_Playfield.GetComponent<Animator>().SetTrigger(LevelUp);
                    SoundManager.Sound.Play(m_LevelUpSound);
                }

                // update progress
                m_LevelVisualizer.Progress = TetrisManager.Instance.LevelCounter.Progress;

            } break;
            case BoardEvent.GameOver:
            {
                m_CommandSequencer.Push(_GameOver());
            } break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // update gizmo
        //m_CommandSequencer.Push(() => m_ShapePositionVisualizer?.Set(m_ActiveShape?.Coords));
    }

    public IEnumerable<BlockVisualizer> GetBlocks()
    {
        // collect blocks from all shapes
        foreach (var shapeVisualizer in ShapeList)
        foreach (var blockVisualizer in shapeVisualizer.Blocks)
            yield return blockVisualizer;
    }

    public IEnumerable<BlockVisualizer> GetBlocks(Func<BlockVisualizer, bool> predicat)
    {
        // collect blocks from all shapes
        foreach (var shapeVisualizer in ShapeList)
            foreach (var blockVisualizer in shapeVisualizer.Blocks)
                if (predicat(blockVisualizer))
                    yield return blockVisualizer;
    }

    private void _UpdateBoardWeight()
    {
        var boardManager = TetrisManager.Instance.GameManager.BoardManager;
        var maxBlocks = boardManager.NumberOfRows * boardManager.NumberOfColumns;
        var blocksCount = boardManager.GameBoard.Sum(n => n.Count(c => c));
        var scale = blocksCount / (float)maxBlocks;

        // lerp
        var weight = Mathf.LerpUnclamped(m_BoardWeightRange.x, m_BoardWeightRange.y, scale);

        // set mass
        m_HillRoot.GetComponent<Rigidbody2D>().mass = weight;
        
    }

    //////////////////////////////////////////////////////////////////////////
    private void _RemoveEmptyShapes()
    {
        // remove & destroy empty shapes
        foreach (var shapeVisualizer in ShapeList.ToList().Where(shapeVisualizer => shapeVisualizer.Blocks.IsEmpty()))
        {
            ShapeList.Remove(shapeVisualizer);
            Destroy(shapeVisualizer.gameObject);
        }
    }

    private IEnumerator _GameOver()
    {
        SoundManager.Sound.Play(m_GameOverSound);

        var effect = Instantiate(m_GameOver)
            .GetModule<ColorizeEffectModule>().Set(GetBlocks().ToList())
            .Run();

        yield return effect;
    }

    private IEnumerator _Collapse(List<int> rows)
    {
        // disallow multiply collapses
        /*if (m_CollapseRunning)
        {
            TetrisManager.Instance.Implementation = false;
            yield return new WaitWhile(() => m_CollapseRunning);
            TetrisManager.Instance.Implementation = true;
        }*/

        m_CollapseRunning = true;

        var blocks   = new HashSet<BlockVisualizer>(GetBlocks());
        var collapse = new HashSet<BlockVisualizer>(blocks.Where(n => rows.Contains(n.Position.y)));;

        foreach (var block in collapse)
            block.Seize();
        
        // remove empty shapes
        _RemoveEmptyShapes();
        // upd board weight
        _UpdateBoardWeight();


        // update progress
        m_LevelVisualizer.Progress = TetrisManager.Instance.LevelCounter.Progress;

        // select collapse method
        var method = CollapseEffectModule.Method.Immediate;

        switch (rows.Count)
        {
            case 1:
            case 2:
                method = UnityRandom.RandomFromArray(CollapseEffectModule.Method.FromLeft, CollapseEffectModule.Method.FromRight, CollapseEffectModule.Method.Random);
                SoundManager.Sound.Play(m_RowClearedSound);
                break;

            case 3:
                method = UnityRandom.RandomFromArray(CollapseEffectModule.Method.FromLeft, CollapseEffectModule.Method.FromRight, CollapseEffectModule.Method.FromCenter, CollapseEffectModule.Method.FromSides);
                SoundManager.Sound.Play(m_RowClearedSound);
                break;

            case 4:
                method = UnityRandom.RandomFromArray(CollapseEffectModule.Method.FromCenter, CollapseEffectModule.Method.FromSides, CollapseEffectModule.Method.FromTop, CollapseEffectModule.Method.FromButtom);

                // play flash first
                TetrisManager.Instance.Implementation = false;
                yield return Instantiate(m_Flash).Run();
                TetrisManager.Instance.Implementation = true;
                break;
        }

        var effect = Instantiate(m_Collapse)
                     .GetModule<CollapseEffectModule>().Set(collapse, blocks, method)
                     .GetModule<ColorizeEffectModule>().Set(collapse.ToList())
                     .Run(() =>
                     {
                     });

        if (WaitCollapseAnimation)
        {
            // animation mod on
            TetrisManager.Instance.Implementation = false;

            // instantiate & init & run effect
            yield return effect;

            // animation mod off
            TetrisManager.Instance.Implementation = true;
        }
        else
        {
            StartCoroutine(effect);
        }
        m_CollapseRunning = false;
    }
    private bool    m_CollapseRunning;
    
    private IEnumerator _Fall(Block block, Move move)
    {
        // disallow fall when collapse happens
        /*if (m_CollapseRunning)
        {
            TetrisManager.Instance.Implementation = false;
            yield return new WaitWhile(() => m_CollapseRunning);
            TetrisManager.Instance.Implementation = true;
        }*/

        var shape = ActiveShape;
        var fallAnimation = m_WaitFallAnimation;

        // enable shape trail
        shape.Trail = true;

        var from = shape.Position;

        // move shape
        shape.PositionOffset = from - block.Position.ToVector2Int();
        shape.OnMove(block, move);

        // instantiate & init & run effect
        var effect =  Instantiate(m_Fall)
            //.SetSpeed(1.0f / TetrisManager.Instance.StepInterval)
            .GetModule<ShapeRouteEffectModule>().Set(shape, from, block.Position.ToVector2Int())
            .GetModule<ImpulseEffectModule>().Set(shape)
            .Run(() =>
            {
                // add hill impulse
                //m_HillRoot.GetComponent<ApplyForce>().Apply(new Vector2((shape.Position.x - 5.0f) / 20.0f, -1.0f).normalized);
                m_HillRoot.GetComponent<ApplyForce>().Apply(Vector2.down);
            });


        if (fallAnimation)
        {
            // animation mod on
            TetrisManager.Instance.Implementation = false;

            yield return effect;

            // animation mod off
            TetrisManager.Instance.Implementation = true;
        }
        else
        {
            StartCoroutine(effect);
        }
    }
}

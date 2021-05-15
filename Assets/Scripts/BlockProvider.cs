using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Tetris.Engine;
using UnityEngine;

[Serializable]
public class BlockProvider : MonoBehaviour, IBlockProvider
{
    [SerializeField]
    private ReorderableArrayT<BlockType>    m_FigureSequence;
    [SerializeField]
    private int                             m_RandomSequenceLenght = 3;
    [SerializeField]
    private bool                            m_Popup;

    public BlockType                        NextBlock
    {
        get
        {
            _fillSequence();
            return m_FigureSequence.FirstOrDefault();
        }
        set
        {
            _fillSequence();
            m_FigureSequence[0] = value;
        }
    }

    public bool Popup
    {
        get => m_Popup;
        set => m_Popup = value;
    }

    public  ReorderableArrayT<BlockType> FigureSequence => m_FigureSequence;

    public RandomMode Mode
    {
        get => m_Mode;
        set => m_Mode = value;
    }

    [SerializeField]
    private RandomMode                         m_Mode = RandomMode.Random;
    [SerializeField] [Range(0.0f, 1.0f)]
    private float                        m_AverageVsMaxRatio;

    //////////////////////////////////////////////////////////////////////////
    [Serializable]
    public enum RandomMode
    {
        Love = 0,
        Random = 1,
        Hate = 2,
    }

    //////////////////////////////////////////////////////////////////////////
    public Block SpawnBlock()
    {
        Block result = null;

        switch (m_Mode)
        {
            case RandomMode.Random:
            { 
                // fill sequence
                _fillSequence();

                // popup block
                result = new Block(m_FigureSequence.First());

                // remove from sequence if required
                if (m_Popup)
                {
                    m_FigureSequence.RemoveAt(0);
                    m_FigureSequence.Add(_getRandomBlockType());
                }
            } break;
            case RandomMode.Hate:
            {
                var candidates = _candidates();
                result = new Block(candidates.MaxBy(n => n.Item2).Item1);
            } break;
            case RandomMode.Love:
            {
                var candidates = _candidates();
                result = new Block(candidates.MinBy(n => n.Item2).Item1);
            } break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // return block
        return result;

        List<(BlockType, float)> _candidates()
        {
            var candidates = new List<(BlockType, float)>(10);
            foreach (BlockType type in Enum.GetValues(typeof(BlockType)))
            {
                var block = new Block(type);
                TetrisManager.Instance.GameManager.BoardManager.SetSpawnPosition(block);

                var moves   = AIAgent.Instance.AI.GetMoves(TetrisManager.Instance.GameManager.BoardManager, block).ToArray();
                var average = moves.Average(n => n.Fitness);
                var max     = moves.First().Fitness;
                candidates.Add((type, Mathf.Lerp(average, max, m_AverageVsMaxRatio)));
            }
            
            UnityRandom.RandomizeList(candidates);

            return candidates;
        }
    }

    private void _fillSequence()
    {
        while (m_FigureSequence.Count < m_RandomSequenceLenght)
            m_FigureSequence.Add(_getRandomBlockType());
    }

    private BlockType _getRandomBlockType()
    {
        return Block.GetRandomBlockType();
    }
}
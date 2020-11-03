using System;
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

    public ReorderableArrayT<BlockType> FigureSequence => m_FigureSequence;

    //////////////////////////////////////////////////////////////////////////
    public Block SpawnBlock()
    {
        // fill sequence
        _fillSequence();

        // popup block
        var result = new Block(m_FigureSequence.First());

        // remove from sequence if required
        if (m_Popup)
        {
            m_FigureSequence.RemoveAt(0);
            m_FigureSequence.Add(_getRandomBlockType());
        }

        // return block
        return result;
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